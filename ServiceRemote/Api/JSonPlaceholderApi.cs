using System.Net.Http.Json;
using System.Text.Json;
using ServiceRemote.Dto;
using ServiceRemote.Models;

namespace ServiceRemote.Api;

/// <summary>
/// Implementacion HTTP del cliente para JSONPlaceholder.
/// </summary>
public class JSonPlaceholderApi : IJsonPlaceholderApi
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;

    public JSonPlaceholderApi(HttpClient httpClient)
    {
        _httpClient = httpClient;

        if (_httpClient.BaseAddress is null)
        {
            _httpClient.BaseAddress = new Uri("https://jsonplaceholder.typicode.com");
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<User>> GetUsers()
    {
        var users = await _httpClient.GetFromJsonAsync<IEnumerable<User>>("/users", JsonOptions);
        return users ?? Enumerable.Empty<User>();
    }

    /// <inheritdoc/>
    public async Task<User> GetUserById(int id)
    {
        var user = await _httpClient.GetFromJsonAsync<User>($"/users/{id}", JsonOptions);

        return user ?? throw new InvalidOperationException(
            $"No se ha podido deserializar el usuario con identificador {id}.");
    }

    /// <inheritdoc/>
    public async Task<User> CreateUser(UserCreateDto user)
    {
        var response = await _httpClient.PostAsJsonAsync("/users", user, JsonOptions);
        response.EnsureSuccessStatusCode();

        var createdUser = await response.Content.ReadFromJsonAsync<User>(JsonOptions);

        return createdUser ?? throw new InvalidOperationException(
            "No se ha podido deserializar el usuario creado.");
    }

    /// <inheritdoc/>
    public async Task<User> UpdateUser(int id, UserUpdateDto user)
    {
        var response = await _httpClient.PutAsJsonAsync($"/users/{id}", user, JsonOptions);
        response.EnsureSuccessStatusCode();

        var updatedUser = await response.Content.ReadFromJsonAsync<User>(JsonOptions);

        return updatedUser ?? throw new InvalidOperationException(
            $"No se ha podido deserializar el usuario actualizado con identificador {id}.");
    }

    /// <inheritdoc/>
    public async Task<User> PatchUser(int id, UserUpdateDto user)
    {
        var response = await _httpClient.PatchAsJsonAsync($"/users/{id}", user, JsonOptions);
        response.EnsureSuccessStatusCode();

        var patchedUser = await response.Content.ReadFromJsonAsync<User>(JsonOptions);

        return patchedUser ?? throw new InvalidOperationException(
            $"No se ha podido deserializar el usuario actualizado con identificador {id}.");
    }

    /// <inheritdoc/>
    public async Task DeleteUser(int id)
    {
        var response = await _httpClient.DeleteAsync($"/users/{id}");
        response.EnsureSuccessStatusCode();
    }
}
