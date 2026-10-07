using NUnit.Framework;
using System.Reactive.Subjects;
using FluentAssertions;
using ServiceRemote.Dto;
using ServiceRemote.Errors;
using ServiceRemote.Notifications.Emiter;

namespace ServiceRemote.Test.Notifications.Test;

[TestFixture]
public class NotificationServiceTest
{
    private Subject<string> _subject = null!;
    private NotificacionService _service = null!;
    private List<string> _receivedMessages = null!;
    private bool _completed;

    [SetUp]
    public void Setup()
    {
        _subject = new Subject<string>();
        _service = new NotificacionService(_subject);

        _receivedMessages = new List<string>();
        _completed = false;

        _subject.Subscribe(
            msg => _receivedMessages.Add(msg),
            () => _completed = true);
    }

    [Test]
    public async Task NotifyCreated_ShouldPublishCreatedMessage()
    {
        // Arrange
        var user = new UserResponseDto
        {
            // Completar propiedades necesarias
        };

        // Act
        await _service.NotifyCreated(user);

        // Assert
        _receivedMessages.Should().HaveCount(1);
        _receivedMessages[0]
            .Should()
            .Be($"Usuario creado: {user.ToJson()}");
    }

    [Test]
    public async Task NotifyUpdated_ShouldPublishUpdatedMessage()
    {
        // Arrange
        var user = new UserResponseDto
        {
            // Completar propiedades necesarias
        };

        // Act
        await _service.NotifyUpdated(user);

        // Assert
        _receivedMessages.Should().ContainSingle();
        _receivedMessages[0]
            .Should()
            .Be($"Usuario actualizado: {user.ToJson()}");
    }

    [Test]
    public async Task NotifyDeleted_ShouldPublishDeletedMessage()
    {
        // Arrange
        var user = new UserResponseDto
        {
            // Completar propiedades necesarias
        };

        // Act
        await _service.NotifyDeleted(user);

        // Assert
        _receivedMessages.Should().ContainSingle();
        _receivedMessages[0]
            .Should()
            .Be($"Usuario eliminado: {user.ToJson()}");
    }

    [Test]
    public async Task NotifyError_ShouldPublishErrorMessage()
    {
        // Arrange
        var error = DomainErrors.NotFound(1);

        // Act
        await _service.NotifyError(error);

        // Assert
        _receivedMessages.Should().ContainSingle();
        _receivedMessages[0]
            .Should()
            .Be($"Error en el servicio: {error.Message}");
    }

    [Test]
    public async Task NotifyEnd_ShouldPublishEndMessage_AndCompleteSubject()
    {
        // Act
        await _service.NotifyEnd();

        // Assert
        _receivedMessages.Should().ContainSingle();
        _receivedMessages[0]
            .Should()
            .Be("Fin de la comunicacion, Cerrando servicio de notificaciones");

        _completed.Should().BeTrue();
    }

    [Test]
    public async Task NotifyCreated_ShouldReturnCompletedTask()
    {
        // Arrange
        var user = new UserResponseDto();

        // Act
        var task = _service.NotifyCreated(user);

        // Assert
        task.IsCompleted.Should().BeTrue();
    }
}