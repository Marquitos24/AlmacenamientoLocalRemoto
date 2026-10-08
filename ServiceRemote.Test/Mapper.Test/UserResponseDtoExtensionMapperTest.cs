using FluentAssertions;
using NUnit.Framework;
using ServiceRemote.Dto;
using ServiceRemote.Models;

namespace ServiceRemote.Test.Mapper.Test;

[TestFixture]
public class UserDtoExtensionMapperTests
{
    [Test]
    public void ToDto_Mapea_User_A_UserResponseDto()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Name = "Marcos",
            UserName = "marcosMotosierra",
            Email = "marcos@email.com"
        };

        // Act
        var dto = UserDtoExtensionMapper.ToDto(user);

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(user.Id);
        dto.Name.Should().Be(user.Name);
        dto.UserName.Should().Be(user.UserName);
        dto.Email.Should().Be(user.Email);
    }

    [Test]
    public void ToModel_Mapea_UserResponseDto_A_User()
    {
        // Arrange
        var dto = new UserResponseDto
        {
            Id = 1,
            Name = "Marcos",
            UserName = "marcosElMejor",
            Email = "marcos@email.com"
        };

        // Act
        var user = dto.ToModel();

        // Assert
        user.Should().NotBeNull();
        user.Id.Should().Be(dto.Id);
        user.Name.Should().Be(dto.Name);
        user.UserName.Should().Be(dto.UserName);
        user.Email.Should().Be(dto.Email);
    }

    [Test]
    public void ToJson_Serializa_UserResponseDto()
    {
        // Arrange
        var dto = new UserResponseDto
        {
            Id = 1,
            Name = "Marcos",
            UserName = "marcos1234",
            Email = "marcos@email.com"
        };

        // Act
        var json = dto.ToJson();

        // Assert
        json.Should().NotBeNullOrEmpty();
        json.Should().Contain("\"Id\":1");
        json.Should().Contain("\"Name\":\"Marcos\"");
        json.Should().Contain("\"UserName\":\"marcos1234\"");
        json.Should().Contain("\"Email\":\"marcos@email.com\"");
    }
}

[TestFixture]
public class UserCreateDtoExtensionMapperTests
{
    /*
     * [Test]
    public void ToDto_Mapea_User_A_UserCreateDto()
    {
        // Arrange
        var user = new User
        {
            Name = "Marcos",
            UserName = "marcos123",
            Email = "marcos@email.com"
        };

        // Act
        var dto = UserCreateDtoExtensionMapper.ToDto(user);

        // Assert
        dto.Should().NotBeNull();
        dto.Name.Should().Be(user.Name);
        dto.UserName.Should().Be(user.UserName);
        dto.Email.Should().Be(user.Email);
    }
     */

    [Test]
    public void ToModel_Mapea_UserCreateDto_A_User()
    {
        // Arrange
        var dto = new UserCreateDto
        {
            Name = "Marcos",
            UserName = "marcos123",
            Email = "marcos@email.com"
        };

        // Act
        var user = dto.ToModel();

        // Assert
        user.Should().NotBeNull();
        user.Name.Should().Be(dto.Name);
        user.UserName.Should().Be(dto.UserName);
        user.Email.Should().Be(dto.Email);
    }

    [Test]
    public void ToJson_Serializa_UserCreateDto()
    {
        // Arrange
        var dto = new UserCreateDto
        {
            Name = "Marcos",
            UserName = "marcos123",
            Email = "marcos@email.com"
        };

        // Act
        var json = dto.ToJson();

        // Assert
        json.Should().NotBeNullOrEmpty();
        json.Should().Contain("\"Name\":\"Marcos\"");
        json.Should().Contain("\"UserName\":\"marcos123\"");
        json.Should().Contain("\"Email\":\"marcos@email.com\"");
    }
}

[TestFixture]
public class UserUpdateDtoExtensionMapperTests
{
    /*
     * [Test]
    public void ToDto_Mapea_User_A_UserUpdateDto()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Name = "Marcos",
            UserName = "marcos123",
            Email = "marcos@email.com"
        };

        // Act
        var dto = UserUpdateDtoExtensionMapper.ToDto(user);

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(user.Id);
        dto.Name.Should().Be(user.Name);
        dto.UserName.Should().Be(user.UserName);
        dto.Email.Should().Be(user.Email);
    }

     */
    [Test]
    public void ToModel_Mapea_UserUpdateDto_A_User()
    {
        // Arrange
        var dto = new UserUpdateDto
        {
            Id = 1,
            Name = "Marcos",
            UserName = "marcos123",
            Email = "marcos@email.com"
        };

        // Act
        var user = dto.ToModel();

        // Assert
        user.Should().NotBeNull();
        user.Id.Should().Be(dto.Id);
        user.Name.Should().Be(dto.Name);
        user.UserName.Should().Be(dto.UserName);
        user.Email.Should().Be(dto.Email);
    }

    [Test]
    public void ToJson_Serializa_UserUpdateDto()
    {
        // Arrange
        var dto = new UserUpdateDto
        {
            Id = 1,
            Name = "Marcos",
            UserName = "marcos123",
            Email = "marcos@email.com"
        };

        // Act
        var json = dto.ToJson();

        // Assert
        json.Should().NotBeNullOrEmpty();
        json.Should().Contain("\"Id\":1");
        json.Should().Contain("\"Name\":\"Marcos\"");
        json.Should().Contain("\"UserName\":\"marcos123\"");
        json.Should().Contain("\"Email\":\"marcos@email.com\"");
    }
}