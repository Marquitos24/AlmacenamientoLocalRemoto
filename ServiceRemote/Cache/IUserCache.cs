using Microsoft.Extensions.Caching.Distributed;
using ServiceRemote.Models;

namespace ServiceRemote.Cache;


/// <summary>
/// Interfaz de caché para funciones simplificadas de cacheo distribuido
/// </summary>
public interface IUserCache
{
    /// <summary>
    /// Obtiene un usuario de la caché por su id
    /// </summary>
    /// <param name="id">El, id del usuario a obtener</param>
    /// <returns>El usuario si existe, de lo contrario null</returns>
    public Task<User?> GetAsync(string id);

    /// <summary>
    /// Establece un valor en la caché distribuida con la clave proporcionada y las opciones de entrada.
    /// </summary>
    /// <param name="user"></param>
    public Task SetAsync(User user);
    
    /// <summary>
    /// Elimina un valor de la caché distribuida basado en la clave proporcionada.
    /// </summary>
    /// <param name="key">La clave del valor a eliminar</param>
    public Task RemoveAsync(string key);
    
    /// <summary>
    /// Elimina todos los valores de la caché distribuida.
    /// </summary>
    public Task RemoveAllAsync();
}