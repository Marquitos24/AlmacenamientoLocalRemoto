using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace ServiceRemote.Cache;

/// <summary>
/// Interfaz de caché genérica para almacenar y recuperar objetos de tipo T.
/// </summary>
public interface IDistributedCache
{
    /// <summary>
    /// Obtiene un valor de la caché distribuida basado en la clave proporcionada.
    /// </summary>
    /// <param name="key">Clave del valor a obtener.</param>
    /// <returns>El valor asociado a la clave, o null si no se encuentra.</returns>
    Task<byte[]?> GetAsync(string key);
    /// <summary>
    /// Establece un valor en la caché distribuida con la clave proporcionada y las opciones de entrada.
    /// </summary>
    /// <param name="key">Clave del valor a establecer.</param>
    /// <param name="value">Valor a establecer.</param>
    /// <param name="options">Opciones de entrada para el valor.</param>
    /// <returns></returns>
    Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options);
    
    /// <summary>
    /// Elimina un valor de la caché distribuida basado en la clave proporcionada.
    /// </summary>
    /// <param name="key">Clave del valor a eliminar.</param>
    /// <returns></returns>
    Task RemoveAsync(string key);
}
