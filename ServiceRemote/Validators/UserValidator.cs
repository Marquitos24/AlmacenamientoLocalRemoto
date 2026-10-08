using CSharpFunctionalExtensions;
using ServiceRemote.Errors;
using ServiceRemote.Models;

namespace ServiceRemote.Validators;
/// <summary>
/// Clase de validacion de usuarios 
/// </summary>
public class UserValidator: IUserValidator
{
    /// <inheritdoc/>
    public Task<Result<User, UserDomainError>> Validate(User obj)
    {
        if (obj == null)
        {
            return Task.FromResult(Result.Failure<User, UserDomainError>(new UserDomainError.Validation(new List<string> { "El usuario es obligatorio." })));
        }

        var errors = new List<string>();

        if (obj.Id <= 0)
            errors.Add("El Id del usuario debe ser mayor a cero.");

        if (string.IsNullOrWhiteSpace(obj.Name))
            errors.Add("El nombre del usuario es obligatorio.");

        if (string.IsNullOrWhiteSpace(obj.UserName))
            errors.Add("El apellido del usuario es obligatorio.");

        if (string.IsNullOrWhiteSpace(obj.Email) || !obj.Email.Contains("@"))
            errors.Add("El correo electrónico del usuario es inválido.");

        if (errors.Any())
            return Task.FromResult(Result.Failure<User, UserDomainError>(new UserDomainError.Validation(errors)));

        return Task.FromResult(Result.Success<User, UserDomainError>(obj));
    }
}