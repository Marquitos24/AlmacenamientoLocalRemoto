using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using ServiceRemote.Cache;
using ServiceRemote.Dto;
using ServiceRemote.Errors;
using ServiceRemote.Models;
using ServiceRemote.Notifications.Emiter;
using ServiceRemote.Repositories;
using ServiceRemote.Services;

namespace ServiceRemote.Test.Services.Test;

[TestFixture]
public class UserServiceTest
{
    private Mock<IUserCache> _cache = null!;
    private Mock<IUserRepository> _repository = null!;
    private Mock<ILogger> _logger = null!;
    private Mock<INotificationService<UserResponseDto, UserDomainError>> _notificador = null!;
    private UserService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _cache = new Mock<IUserCache>();
        _repository = new Mock<IUserRepository>();
        _logger = new Mock<ILogger>();
        _notificador = new Mock<INotificationService<UserResponseDto, UserDomainError>>();

        _service = new UserService(
            _cache.Object,
            _repository.Object,
            _logger.Object,
            _notificador.Object);
    }

    [Test]
    public async Task GetAll_Should_Return_All_Users_From_Repository()
    {
        var users = new[]
        {
            CreateUser(1),
            CreateUser(2)
        };

        _repository.Setup(repository => repository.GetAll()).ReturnsAsync(users);

        var result = await _service.GetAll();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(users, options => options.ExcludingMissingMembers());
        _repository.Verify(repository => repository.GetAll(), Times.Once);
    }

    [Test]
    public async Task GetById_Should_Return_Cached_User_When_User_Exists_In_Cache()
    {
        var cachedUser = CreateUser(1);

        _cache.Setup(cache => cache.GetAsync("1")).ReturnsAsync(cachedUser);

        var result = await _service.GetById(1);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(cachedUser, options => options.ExcludingMissingMembers());

        _cache.Verify(cache => cache.GetAsync("1"), Times.Once);
        _repository.Verify(repository => repository.GetById(It.IsAny<int>()), Times.Never);
        _cache.Verify(cache => cache.SetAsync(It.IsAny<User>()), Times.Never);
    }

    [Test]
    public async Task GetById_Should_Get_User_From_Repository_And_Store_In_Cache_When_Cache_Misses()
    {
        var user = CreateUser(2);

        _cache.Setup(cache => cache.GetAsync("2")).ReturnsAsync((User?)null);
        _repository.Setup(repository => repository.GetById(2)).ReturnsAsync(user);

        var result = await _service.GetById(2);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(user, options => options.ExcludingMissingMembers());

        _cache.Verify(cache => cache.GetAsync("2"), Times.Once);
        _repository.Verify(repository => repository.GetById(2), Times.Once);
        _cache.Verify(cache => cache.SetAsync(user), Times.Once);
    }

    [Test]
    public async Task GetById_Should_Return_NotFound_And_Notify_Error_When_User_Does_Not_Exist()
    {
        _cache.Setup(cache => cache.GetAsync("99")).ReturnsAsync((User?)null);
        _repository.Setup(repository => repository.GetById(99)).ReturnsAsync((User?)null);

        var result = await _service.GetById(99);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<UserDomainError.NotFound>();

        _notificador.Verify(
            notificador => notificador.NotifyError(It.Is<UserDomainError.NotFound>(error => error.Id == 99)),
            Times.Once);
    }

    [Test]
    public async Task Create_Should_Create_User_Cache_It_And_Notify_Created()
    {
        var createDto = new UserCreateDto
        {
            Id = 3,
            Name = "Usuario 3",
            UserName = "usuario3",
            Email = "usuario3@example.com"
        };

        var createdUser = createDto.ToModel();

        _repository
            .Setup(repository => repository.Create(It.Is<User>(user => user.Id == createDto.Id)))
            .ReturnsAsync(createdUser);

        var result = await _service.Create(createDto);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(createdUser, options => options.ExcludingMissingMembers());

        _repository.Verify(repository => repository.Create(It.Is<User>(user => user.Id == createDto.Id)), Times.Once);
        _cache.Verify(cache => cache.SetAsync(createdUser), Times.Once);
        _notificador.Verify(
            notificador => notificador.NotifyCreated(It.Is<UserResponseDto>(dto => dto.Id == createdUser.Id)),
            Times.Once);
    }

    [Test]
    public async Task Update_Should_Return_NotFound_When_User_Does_Not_Exist()
    {
        var updateDto = new UserUpdateDto
        {
            Id = 44,
            Name = "No existe",
            UserName = "noexiste",
            Email = "noexiste@example.com"
        };

        _repository.Setup(repository => repository.GetById(updateDto.Id)).ReturnsAsync((User?)null);

        var result = await _service.Update(updateDto);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<UserDomainError.NotFound>();

        _repository.Verify(repository => repository.Update(It.IsAny<User>()), Times.Never);
        _notificador.Verify(
            notificador => notificador.NotifyError(It.Is<UserDomainError.NotFound>(error => error.Id == updateDto.Id)),
            Times.Once);
    }

    [Test]
    public async Task Update_Should_Update_User_Cache_It_And_Notify_Updated()
    {
        var existingUser = CreateUser(4);
        var updateDto = new UserUpdateDto
        {
            Id = 4,
            Name = "Usuario actualizado",
            UserName = "actualizado",
            Email = "actualizado@example.com"
        };
        var updatedUser = updateDto.ToModel();

        _repository.Setup(repository => repository.GetById(updateDto.Id)).ReturnsAsync(existingUser);
        _repository.Setup(repository => repository.Update(It.Is<User>(user => user.Id == updateDto.Id)))
            .ReturnsAsync(updatedUser);

        var result = await _service.Update(updateDto);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(updatedUser, options => options.ExcludingMissingMembers());

        _cache.Verify(cache => cache.SetAsync(updatedUser), Times.Once);
        _notificador.Verify(
            notificador => notificador.NotifyUpdated(It.Is<UserResponseDto>(dto => dto.Id == updatedUser.Id)),
            Times.Once);
    }

    [Test]
    public async Task Delete_Should_Delete_User_Remove_Cache_Notify_Deleted_And_Return_Remaining_Users()
    {
        var deletedUser = CreateUser(5);
        var remainingUsers = new[]
        {
            CreateUser(1),
            CreateUser(2)
        };

        _repository.Setup(repository => repository.GetById(5)).ReturnsAsync(deletedUser);
        _repository.Setup(repository => repository.Delete(5)).Returns(Task.CompletedTask);
        _repository.Setup(repository => repository.GetAll()).ReturnsAsync(remainingUsers);

        var result = await _service.Delete(5);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(remainingUsers, options => options.ExcludingMissingMembers());

        _repository.Verify(repository => repository.Delete(5), Times.Once);
        _cache.Verify(cache => cache.RemoveAsync("5"), Times.Once);
        _notificador.Verify(
            notificador => notificador.NotifyDeleted(It.Is<UserResponseDto>(dto => dto.Id == deletedUser.Id)),
            Times.Once);
    }

    [Test]
    public async Task GetAll_Should_Return_Storage_Error_And_Notify_Error_When_Repository_Fails()
    {
        var exception = new InvalidOperationException("Fallo remoto");

        _repository.Setup(repository => repository.GetAll()).ThrowsAsync(exception);

        var result = await _service.GetAll();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<UserDomainError.Storage>();

        var error = (UserDomainError.Storage)result.Error;
        error.Exception.Should().Be(exception);

        _notificador.Verify(
            notificador => notificador.NotifyError(It.Is<UserDomainError.Storage>(storage => storage.Exception == exception)),
            Times.Once);
    }

    private static User CreateUser(int id)
    {
        return new User
        {
            Id = id,
            Name = $"Usuario {id}",
            UserName = $"usuario{id}",
            Email = $"usuario{id}@example.com"
        };
    }
}
