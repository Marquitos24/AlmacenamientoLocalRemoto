using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using ServiceRemote.Errors;

namespace ServiceRemote.Test.Errors.Test;

[TestFixture]
public class DomainErrorExtensionsTests
{
    private sealed record UnknownError()
        : UserDomainError("Error desconocido");
    
    [Test]
    public void NotFound_Retorna_404_ErrorIdentificador()
    {
        // Arrange
        var error = DomainErrors.NotFound(10);

        // Act
        var resultado = error.ToHttpResult<object>();

        // Assert
        resultado.Result.Should().BeOfType<NotFoundObjectResult>();

        var objectResult = (NotFoundObjectResult)resultado.Result!;

        objectResult.StatusCode.Should().Be(404);
        objectResult.Value.Should().NotBeNull();
    }

    [Test]
    public void Validation_Retorna_400_ErroresValidaciones()
    {
        // Arrange
        var errors = new[]
        {
            "Nombre obligatorio",
            "El nombre de usuario no puede tener menos de 1 caracter",
            "Email incorrecto"
        };

        var error = DomainErrors.Validation(errors);

        // Act
        var result = error.ToHttpResult<object>();

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();

        var objectResult = (BadRequestObjectResult)result.Result!;

        objectResult.StatusCode.Should().Be(400);
        objectResult.Value.Should().NotBeNull();
    }

    [Test]
    public void Storage_Retorna_500_ErrorBD()
    {
        // Arrange
        var exception = new Exception("Error de base de datos");
        var error = DomainErrors.Storage(exception);

        // Act
        var result = error.ToHttpResult<object>();

        // Assert
        result.Result.Should().BeOfType<ObjectResult>();

        var objectResult = (ObjectResult)result.Result!;

        objectResult.StatusCode.Should().Be(500);
        objectResult.Value.Should().NotBeNull();
    }

    [Test]
    public void ErrorDesconocido_Retorna_500()
    {
        // Arrange
        var error = new UnknownError();

        // Act
        var result = error.ToHttpResult<object>();

        // Assert
        result.Result.Should().BeOfType<ObjectResult>();

        var objectResult = (ObjectResult)result.Result!;

        objectResult.StatusCode.Should().Be(500);
        objectResult.Value.Should().NotBeNull();
    }

    
}