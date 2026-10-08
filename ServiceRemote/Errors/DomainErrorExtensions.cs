using Microsoft.AspNetCore.Mvc;

namespace ServiceRemote.Errors;

public static class DomainErrorExtensions
{
    public static ActionResult<T> ToHttpResult<T>(this UserDomainError error) => error switch
    {
        UserDomainError.NotFound => new NotFoundObjectResult(new { message = error.Message }),
        UserDomainError.Validation ve => new BadRequestObjectResult(new { message = ve.Message, errors = ve.Errors }),
        UserDomainError.Storage => new ObjectResult(new { message = error.Message }) { StatusCode = 500 },
        _ => new ObjectResult(new { message = "Error interno del servidor" }) { StatusCode = 500 }
    };
}