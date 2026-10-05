namespace ServiceRemote.Errors;

public abstract record UserDomainError(string Message)
{
    public sealed record NotFound(int Id)
        : UserDomainError($"No se ha encontrado ningun usuario con el identificador: {Id}");

    public sealed record Validation(IEnumerable<string> Errors)
        : UserDomainError("Se han detectado errores de validación en la entidad.");
    
    public sealed record Storage(Exception Exception)
        : UserDomainError($"Error de almacenamiento: {Exception.Message}");
}

public static class DomainErrors
{
    public static UserDomainError NotFound(int id) => new UserDomainError.NotFound(id);
    public static UserDomainError Validation(IEnumerable<string> errors) => new UserDomainError.Validation(errors);
    public static UserDomainError Storage(Exception ex) => new UserDomainError.Storage(ex);
}