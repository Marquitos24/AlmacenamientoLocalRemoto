using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using NUnit.Framework;
using ServiceRemote.Cache;
using ServiceRemote.Models;
using IDistributedCache = Microsoft.Extensions.Caching.Distributed.IDistributedCache;

namespace ServiceRemote.Test.Cache.Test;

[TestFixture]
public class UserCacheService_Test
{
    private Mock<IDistributedCache> _cacheMock = null!;
    private UserCacheService _service = null!;

    [SetUp]
    public void SetUp()
    {
            _cacheMock = new Mock<IDistributedCache>();
            _service = new UserCacheService(_cacheMock.Object);
    }

    [Test]
    public async Task GetAsync_WhenUserIsNotCached_ReturnsNull()
    {
        _cacheMock
            .Setup(cache => cache.GetAsync("user:1"))
            .ReturnsAsync((byte[]?)null);

        var result = await _service.GetAsync("1");

        Assert.That(result, Is.Null);
        _cacheMock.Verify(cache => cache.GetAsync("user:1"), Times.Once);
        _cacheMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetAsync_WhenUserIsCached_ReturnsDeserializedUser()
    {
        var cachedUser = CreateUser();
        var cachedJson = JsonSerializer.Serialize(cachedUser);
        var cachedBytes = Encoding.UTF8.GetBytes(cachedJson);

        _cacheMock
            .Setup(cache => cache.GetAsync("user:1"))
            .ReturnsAsync(cachedBytes);

        var result = await _service.GetAsync("1");

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result!.Id, Is.EqualTo(cachedUser.Id));
            Assert.That(result.Name, Is.EqualTo(cachedUser.Name));
            Assert.That(result.UserName, Is.EqualTo(cachedUser.UserName));
            Assert.That(result.Email, Is.EqualTo(cachedUser.Email));
        });
        _cacheMock.Verify(cache => cache.GetAsync("user:1"), Times.Once);
        _cacheMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task SetAsync_SerializesUserAndStoresItWithUserKey()
    {
        var user = CreateUser();
        string? capturedKey = null;
        byte[]? capturedValue = null;
        DistributedCacheEntryOptions? capturedOptions = null;

        _cacheMock
            .Setup(cache => cache.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()))
            .Callback<string, byte[], DistributedCacheEntryOptions, CancellationToken>(
                (key, value, options, token) =>
                {
                    capturedKey = key;
                    capturedValue = value;
                    capturedOptions = options;
                })
            .Returns(Task.CompletedTask);

        await _service.SetAsync(user);

        Assert.That(capturedKey, Is.EqualTo("user:1"));
        Assert.That(capturedOptions, Is.Not.Null);

        var storedUser = JsonSerializer.Deserialize<User>(capturedValue!);
        Assert.That(storedUser, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(storedUser!.Id, Is.EqualTo(user.Id));
            Assert.That(storedUser.Name, Is.EqualTo(user.Name));
            Assert.That(storedUser.UserName, Is.EqualTo(user.UserName));
            Assert.That(storedUser.Email, Is.EqualTo(user.Email));
        });
        _cacheMock.Verify(cache => cache.SetAsync(
                "user:1",
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
        _cacheMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task RemoveAsync_RemovesUserKeyFromCache()
    {
        _cacheMock
            .Setup(cache => cache.RemoveAsync("user:1"))
            .Returns(Task.CompletedTask);

        await _service.RemoveAsync("1");

        _cacheMock.Verify(cache => cache.RemoveAsync("user:1"), Times.Once);
        _cacheMock.VerifyNoOtherCalls();
    }

    private static User CreateUser()
    {
        return new User
        {
            Id = 1,
            Name = "Nizar",
            UserName = "nizar",
            Email = "nizar@example.com"
        };
    }
}
