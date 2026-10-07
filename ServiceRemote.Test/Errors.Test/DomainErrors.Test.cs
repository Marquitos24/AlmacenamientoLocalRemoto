using FluentAssertions;
using NUnit.Framework;
using ServiceRemote.Errors;

namespace ServiceRemote.Test.Errors.Test;

[TestFixture]
public class DomainErrorsTests
{
    [Test]
    public void NotFound_Should_Return_NotFound_Error()
    {
        // Act
        var error = DomainErrors.NotFound(10);

        // Assert
        error.Should().BeOfType<UserDomainError.NotFound>();

        error.Message.Should()
            .Be("No se ha encontrado ningun usuario con el identificador: 10");
    }

    [Test]
    public void Validation_Should_Return_Validation_Error()
    {
        // Arrange
        var errors = new[] { "Error 1", "Error 2" };

        // Act
        var error = DomainErrors.Validation(errors);

        // Assert
        error.Should().BeOfType<UserDomainError.Validation>();

        var validationError = (UserDomainError.Validation)error;

        validationError.Errors.Should()
            .BeEquivalentTo(errors);
    }

    [Test]
    public void Storage_Should_Return_Storage_Error()
    {
        // Arrange
        var exception = new Exception("Fallo almacenamiento");

        // Act
        var error = DomainErrors.Storage(exception);

        // Assert
        error.Should().BeOfType<UserDomainError.Storage>();

        var storageError = (UserDomainError.Storage)error;

        storageError.Exception.Should().Be(exception);

        storageError.Message.Should()
            .Be("Error de almacenamiento: Fallo almacenamiento");
    }
    [Test]
    public void NotFound_With_Same_Id_Should_Be_Equal()
    {
        var error1 = new UserDomainError.NotFound(1);
        var error2 = new UserDomainError.NotFound(1);
        
        error1.Should().Be(error2);
    }
}