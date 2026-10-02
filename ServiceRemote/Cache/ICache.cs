namespace ServiceRemote.Cache;

/// <summary>
/// Interfaz de cache genérica para almacenar y recuperar objetos de tipo T.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ICache<T>
{
    /// <summary>
    /// Obtiene un objeto T en funcion de su identificador en su usencia devuelve null
    /// </summary>
    /// <param name="id"> Identificador de T</param>
    /// <returns></returns>
    public T? Get(int id);
    
    /// <summary>
    /// Introduce un objeto T en la caché
    /// </summary>
    /// <param name="obt">Objeto a obtener</param>
    /// <returns></returns>
    public void Set(T obt);
    
    /// <summary>
    /// Elimina de la caché el objeto T en funcion del identificador
    /// </summary>
    /// <param name="id">Identificador del objeto a eliminar</param>
    /// <returns></returns>
    public void Delete(int id);

    /// <summary>
    /// Elimina de la caché todos los datos 
    /// </summary>
    /// <returns></returns>
    public void DeleteAll();
}