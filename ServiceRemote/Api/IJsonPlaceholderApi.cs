using Refit;
using ServiceRemote.Dto;
using ServiceRemote.Models;

namespace ServiceRemote.Api;

/// <summary>
/// Cliente REST para usar JSONPlaceholder como almacenamiento remoto.
/// Base URL: https://jsonplaceholder.typicode.com
/// </summary>
public interface IJsonPlaceholderApi
{
    /// <summary>
    /// Obtiene todos los usuarios remotos.
    /// </summary>
    [Get("/users")]
    Task<IEnumerable<User>> GetUsers();

    /// <summary>
    /// Obtiene un usuario remoto por identificador.
    /// </summary>
    [Get("/users/{id}")]
    Task<User> GetUserById(int id);

    /// <summary>
    /// Crea un usuario remoto.
    /// JSONPlaceholder devuelve una respuesta simulada; no persiste realmente el recurso.
    /// </summary>
    [Post("/users")]
    Task<User> CreateUser([Body] UserCreateDto user);

    /// <summary>
    /// Reemplaza un usuario remoto.
    /// JSONPlaceholder devuelve una respuesta simulada; no persiste realmente el cambio.
    /// </summary>
    [Put("/users/{id}")]
    Task<User> UpdateUser(int id, [Body] UserUpdateDto user);

    /// <summary>
    /// Actualiza parcialmente un usuario remoto.
    /// JSONPlaceholder devuelve una respuesta simulada; no persiste realmente el cambio.
    /// </summary>
    [Patch("/users/{id}")]
    Task<User> PatchUser(int id, [Body] UserUpdateDto user);

    /// <summary>
    /// Elimina un usuario remoto.
    /// JSONPlaceholder devuelve una respuesta simulada; no persiste realmente el cambio.
    /// </summary>
    [Delete("/users/{id}")]
    Task DeleteUser(int id);
}
