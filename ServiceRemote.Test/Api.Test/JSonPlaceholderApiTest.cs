using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using ServiceRemote.Api;
using ServiceRemote.Dto;
using ServiceRemote.Models;

namespace ServiceRemote.Test.Api.Test;

[TestFixture]
public class JSonPlaceholderApiTest
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private Mock<HttpMessageHandler> _handler = null!;
    private JSonPlaceholderApi _api = null!;

    [SetUp]
    public void SetUp()
    {
        _handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        var httpClient = new HttpClient(_handler.Object)
        {
            BaseAddress = new Uri("https://jsonplaceholder.typicode.com")
        };

        _api = new JSonPlaceholderApi(httpClient);
    }

    [Test]
    public async Task GetUsers_Should_Send_Get_Request_And_Return_Users()
    {
        var expectedUsers = new[]
        {
            new User { Id = 1, Name = "Leanne Graham", UserName = "Bret", Email = "leanne@example.com" },
            new User { Id = 2, Name = "Ervin Howell", UserName = "Antonette", Email = "ervin@example.com" }
        };

        SetupRequest(HttpMethod.Get, "/users", JsonResponse(expectedUsers));

        var users = await _api.GetUsers();

        users.Should().BeEquivalentTo(expectedUsers);
        VerifyRequest();
    }

    [Test]
    public async Task GetUserById_Should_Send_Get_Request_And_Return_User()
    {
        var expectedUser = new User
        {
            Id = 5,
            Name = "Chelsey Dietrich",
            UserName = "Kamren",
            Email = "chelsey@example.com"
        };

        SetupRequest(HttpMethod.Get, "/users/5", JsonResponse(expectedUser));

        var user = await _api.GetUserById(5);

        user.Should().BeEquivalentTo(expectedUser);
        VerifyRequest();
    }

    [Test]
    public async Task CreateUser_Should_Send_Post_Request_With_Body_And_Return_Created_User()
    {
        var createDto = new UserCreateDto
        {
            Name = "Nuevo usuario",
            UserName = "nuevo",
            Email = "nuevo@example.com"
        };

        var createdUser = new User
        {
            Id = 11,
            Name = createDto.Name,
            UserName = createDto.UserName,
            Email = createDto.Email
        };

        SetupRequest(
            HttpMethod.Post,
            "/users",
            JsonResponse(createdUser, HttpStatusCode.Created),
            request => AssertJsonBody(request, createDto));

        var user = await _api.CreateUser(createDto);

        user.Should().BeEquivalentTo(createdUser);
        VerifyRequest();
    }

    [Test]
    public async Task UpdateUser_Should_Send_Put_Request_With_Body_And_Return_Updated_User()
    {
        var updateDto = new UserUpdateDto
        {
            Id = 4,
            Name = "Usuario actualizado",
            UserName = "actualizado",
            Email = "actualizado@example.com"
        };

        var updatedUser = new User
        {
            Id = 4,
            Name = updateDto.Name!,
            UserName = updateDto.UserName!,
            Email = updateDto.Email!
        };

        SetupRequest(
            HttpMethod.Put,
            "/users/4",
            JsonResponse(updatedUser),
            request => AssertJsonBody(request, updateDto));

        var user = await _api.UpdateUser(4, updateDto);

        user.Should().BeEquivalentTo(updatedUser);
        VerifyRequest();
    }

    [Test]
    public async Task PatchUser_Should_Send_Patch_Request_With_Body_And_Return_Patched_User()
    {
        var updateDto = new UserUpdateDto
        {
            Name = "Nombre parcial"
        };

        var patchedUser = new User
        {
            Id = 7,
            Name = "Nombre parcial",
            UserName = "usuario",
            Email = "usuario@example.com"
        };

        SetupRequest(
            HttpMethod.Patch,
            "/users/7",
            JsonResponse(patchedUser),
            request => AssertJsonBody(request, updateDto));

        var user = await _api.PatchUser(7, updateDto);

        user.Should().BeEquivalentTo(patchedUser);
        VerifyRequest();
    }

    [Test]
    public async Task DeleteUser_Should_Send_Delete_Request()
    {
        SetupRequest(HttpMethod.Delete, "/users/3", new HttpResponseMessage(HttpStatusCode.OK));

        await _api.DeleteUser(3);

        VerifyRequest();
    }

    [Test]
    public async Task CreateUser_Should_Throw_When_Response_Is_Not_Success()
    {
        var createDto = new UserCreateDto
        {
            Name = "Usuario",
            UserName = "usuario",
            Email = "usuario@example.com"
        };

        SetupRequest(
            HttpMethod.Post,
            "/users",
            new HttpResponseMessage(HttpStatusCode.InternalServerError));

        var action = async () => await _api.CreateUser(createDto);

        await action.Should().ThrowAsync<HttpRequestException>();
        VerifyRequest();
    }

    private void SetupRequest(
        HttpMethod method,
        string path,
        HttpResponseMessage response,
        Action<HttpRequestMessage>? assertRequest = null)
    {
        _handler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(request =>
                    request.Method == method &&
                    request.RequestUri != null &&
                    request.RequestUri.PathAndQuery == path),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((request, _) => assertRequest?.Invoke(request))
            .ReturnsAsync(response)
            .Verifiable();
    }

    private void VerifyRequest()
    {
        _handler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>());
    }

    private static HttpResponseMessage JsonResponse<T>(T body, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new HttpResponseMessage(statusCode)
        {
            Content = JsonContent.Create(body, options: JsonOptions)
        };
    }

    private static void AssertJsonBody<T>(HttpRequestMessage request, T expectedBody)
    {
        request.Content.Should().NotBeNull();

        var requestBody = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
        var expectedJson = JsonSerializer.Serialize(expectedBody, JsonOptions);

        using var requestDocument = JsonDocument.Parse(requestBody);
        using var expectedDocument = JsonDocument.Parse(expectedJson);

        requestDocument.RootElement.ToString()
            .Should()
            .Be(expectedDocument.RootElement.ToString());

        request.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }
}
