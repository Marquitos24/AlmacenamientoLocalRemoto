using CSharpFunctionalExtensions;
using ServiceRemote.Errors;
using ServiceRemote.Models;

namespace ServiceRemote.Validators;

/// <summary>
/// Contrato de validacion de entidades
/// </summary>
public interface IUserValidator
{
    /// <summary>
    /// Metodo de validacion de entidades, devuelve un resultado que puede ser un error o el usuario mismo
    /// </summary>
    /// <param name="obj"> Entidad a validar </param>
    /// <returns> Result de error o entidad</returns>
    Task<Result<User, UserDomainError>> Validate(User? obj);
}
