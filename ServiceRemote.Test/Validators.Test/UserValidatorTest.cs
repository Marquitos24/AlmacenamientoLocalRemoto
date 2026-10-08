using FluentAssertions;
using NUnit.Framework;
using ServiceRemote.Errors;
using ServiceRemote.Models;
using ServiceRemote.Validators;

namespace ServiceRemote.Test.Validators.Test;

[TestFixture]
public class UserValidatorTest
{
    private UserValidator _validator = null!;

    [SetUp]
    public void SetUp()
    {
        _validator = new UserValidator();
    }

    [Test]
    public async Task Validate_Should_Return_Success_When_User_Is_Valid()
    {
        var user = new User
        {
            Id = 1,
            Name = "Nizar",
            UserName = "nizar",
            Email = "nizar@example.com"
        };

        var result = await _validator.Validate(user);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeSameAs(user);
    }

    [Test]
    public async Task Validate_Should_Return_Domain_Error_When_User_Is_Null()
    {
        var result = await _validator.Validate(null);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<UserDomainError.Validation>();

        var error = (UserDomainError.Validation)result.Error;
        error.Errors.Should().ContainSingle("El usuario es obligatorio.");
    }

    [Test]
    public async Task Validate_Should_Return_Domain_Error_With_All_Validation_Errors()
    {
        var user = new User
        {
            Id = 0,
            Name = " ",
            UserName = string.Empty,
            Email = "email-invalido"
        };

        var result = await _validator.Validate(user);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<UserDomainError.Validation>();

        var error = (UserDomainError.Validation)result.Error;
        error.Errors.Should().BeEquivalentTo(
            "El Id del usuario debe ser mayor a cero.",
            "El nombre del usuario es obligatorio.",
            "El apellido del usuario es obligatorio.",
            "El correo electrónico del usuario es inválido.");
    }

    [Test]
    public async Task Validate_Should_Return_Email_Required_When_Email_Is_Empty()
    {
        var user = new User
        {
            Id = 1,
            Name = "Nizar",
            UserName = "nizar",
            Email = " "
        };

        var result = await _validator.Validate(user);

        result.IsFailure.Should().BeTrue();

        var error = (UserDomainError.Validation)result.Error;
        error.Errors.Should().ContainSingle("El email del usuario es obligatorio.");
    }
}
