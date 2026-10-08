using CSharpFunctionalExtensions;
using ServiceRemote.Errors;

namespace ServiceRemote.Services;

/// <summary>
/// Interfaz genérica para servicios que manejan operaciones CRUD (Crear, Leer, Actualizar, Eliminar)
/// para entidades de tipo TDto con identificadores de tipo TId, utilizando DTOs de creación
/// y actualización, y devolviendo errores de dominio de tipo TDomainError.
/// </summary>
/// <typeparam name="TDto"> Clase devuelta</typeparam>
/// <typeparam name="TId"> Identificador</typeparam>
/// <typeparam name="TCreateDto"> Clase de creacion</typeparam>
/// <typeparam name="TUpdateDto"> Clase de Modificacion</typeparam>
/// <typeparam name="TDomainError"> Errores de dominio</typeparam>
public interface IService<TDto, TId, TCreateDto, TUpdateDto,TDomainError>
{
    /// <summary>
    /// Obtiene todos los usuarios activos de la base de datos.
    /// Si no hay usuarios activos, devuelve un error de dominio.
    /// </summary>
    /// <returns> Lista de todos los usuarios o un error de dominio</returns>
    Task<Result <IEnumerable<TDto>, TDomainError>> GetAll();
        
    /// <summary>
    /// Obtiene un usuario activo por su id de la base de datos.
    /// Si no hay usuarios activos, devuelve un error de dominio.
    /// </summary>
    /// <param name="id">El id del usuario a obtener</param>
    /// <returns>usuario encontrado o un error de dominio</returns>
    Task<Result <TDto, TDomainError>> GetById(TId id);
    
    /// <summary>
    /// Crea un nuevo usuario en la base de datos.
    /// </summary>
    /// <param name="data">nuevo usuario a registrar</param>
    /// <returns></returns>
    Task<Result <TDto, TDomainError>> Create(TCreateDto obj);

    /// <summary>
    /// Actualiza un usuario existente en la base de datos.
    /// en caso de que no exista el usuario, devuelve un error de dominio.
    /// </summary>
    /// <param name="obj"> Objeto con los datos actualizados </param>
    /// <returns> Devuelve el usuario actualizado o un error de dominio </returns>
    Task<Result <TDto, TDomainError>> Update(TUpdateDto obj);

    /// <summary>
    /// Elimina un usuario existente en la base de datos.
    /// En caso de que no exista el usuario, devuelve un error de dominio.
    /// </summary>
    /// <param name="id">El id del usuario a eliminar</param>
    /// <returns> Devuelve la lista de usuarios actualizada o un error de dominio </returns>
    Task<UnitResult <TDomainError>> Delete(TId id);
}
