using FluentAssertions;
using NUnit.Framework;
using ServiceRemote.Errors;

namespace ServiceRemote.Test.Errors.Test;

[TestFixture]
public class UserDomainErrorTests
{
    [Test]
    public void NotFound_Should_Create_Error_With_Correct_Message()
    {
        // Arrange
        const int id = 15;

        // Act
        var error = new UserDomainError.NotFound(id);

        // Assert
        error.Message.Should()
            .Be($"No se ha encontrado ningun usuario con el identificador: {id}");
    }

    [Test]
    public void NotFound_Should_Store_Id()
    {
        // Arrange
        const int id = 15;

        // Act
        var error = new UserDomainError.NotFound(id);

        // Assert
        error.Id.Should().Be(id);
    }

    [Test]
    public void Validation_Should_Create_Error_With_Default_Message()
    {
        // Arrange
        var errors = new[]
        {
            "Nombre obligatorio",
            "Email incorrecto"
        };

        // Act
        var error = new UserDomainError.Validation(errors);

        // Assert
        error.Message.Should()
            .Be("Se han detectado errores de validación en la entidad.");
    }

    [Test]
    public void Validation_Should_Store_Errors()
    {
        // Arrange
        var errors = new[]
        {
            "Nombre obligatorio",
            "Email incorrecto"
        };

        // Act
        var error = new UserDomainError.Validation(errors);

        // Assert
        error.Errors.Should().BeEquivalentTo(errors);
    }

    [Test]
    public void Storage_Should_Create_Error_With_Exception_Message()
    {
        // Arrange
        var exception = new InvalidOperationException("Error de base de datos");

        // Act
        var error = new UserDomainError.Storage(exception);

        // Assert
        error.Message.Should()
            .Be("Error de almacenamiento: Error de base de datos");
    }

    [Test]
    public void Storage_Should_Store_Exception()
    {
        // Arrange
        var exception = new InvalidOperationException("Error de base de datos");

        // Act
        var error = new UserDomainError.Storage(exception);

        // Assert
        error.Exception.Should().Be(exception);
    }
}