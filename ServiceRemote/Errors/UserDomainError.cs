namespace ServiceRemote.Errors;

/// <summary>
/// Representa un error de dominio relacionado con la entidad de usuario.
/// </summary>
/// <param name="Message">El mensaje de error.</param>
public abstract record UserDomainError(string Message)
{
    /// <summary>
    /// Representa un error de dominio cuando no se encuentra un usuario con el identificador especificado.
    /// </summary>
    /// <param name="Id">El identificador del usuario no encontrado.</param>
    public sealed record NotFound(int Id)
        : UserDomainError($"No se ha encontrado ningun usuario con el identificador: {Id}");

    /// <summary>
    /// Representa un error de dominio cuando se detectan errores de validación en la entidad de usuario.
    /// </summary>
    /// <param name="Errors">La lista de errores de validación.</param>
    public sealed record Validation(IEnumerable<string> Errors)
        : UserDomainError("Se han detectado errores de validación en la entidad.");
    
    /// <summary>
    /// Representa un error de dominio relacionado con problemas de almacenamiento de datos.
    /// </summary>
    /// <param name="Exception">La excepción que representa el error de almacenamiento.</param>
    public sealed record Storage(Exception Exception)
        : UserDomainError($"Error de almacenamiento: {Exception.Message}");
}

/// <summary>
/// Proporciona métodos de ayuda para crear instancias de errores de dominio relacionados con la entidad de usuario.
/// </summary>
public static class DomainErrors
{
    public static UserDomainError NotFound(int id) => new UserDomainError.NotFound(id);
    public static UserDomainError Validation(IEnumerable<string> errors) => new UserDomainError.Validation(errors);
    public static UserDomainError Storage(Exception ex) => new UserDomainError.Storage(ex);
}