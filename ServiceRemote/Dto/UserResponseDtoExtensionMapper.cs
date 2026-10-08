using System;
using ServiceRemote.Entity;
using ServiceRemote.Models;

namespace ServiceRemote.Dto;

public static class UserDtoExtensionMapper
{
    public static UserResponseDto ToDto(this User user)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            UserName = user.UserName,
            Email = user.Email,
            CreateAt = DateTime.Now,
            UpdateAt = DateTime.Now
        };
    }
    
    public static User ToModel(this Dto.UserResponseDto dto)
    {
        return new User
        {
            Id = dto.Id,
            Name = dto.Name,
            UserName = dto.UserName,
            Email = dto.Email,
        };
    }
    public static string ToJson(this UserResponseDto dto)
    {
        return System.Text.Json.JsonSerializer.Serialize(dto);
    }
}

public static class UserCreateDtoExtensionMapper
{
    public static User ToModel(this Dto.UserCreateDto dto)
    {
        return new User
        {
            Name = dto.Name,
            UserName = dto.UserName,
            Email = dto.Email,
        };
    }
    public static string ToJson(this UserCreateDto dto)
    {
        return System.Text.Json.JsonSerializer.Serialize(dto);
    }
}

public static class UserUpdateDtoExtensionMapper
{
    public static User ToModel(this UserUpdateDto dto)
    {
        return new User
        {
            Id = dto.Id,
            Name = dto.Name,
            UserName = dto.UserName,
            Email = dto.Email,
        };
    }
    public static string ToJson(this UserUpdateDto dto)
    {
        return System.Text.Json.JsonSerializer.Serialize(dto);
    }
}

/// <summary>
/// Mapeos entre el modelo de dominio User y la entidad de EF Core.
/// </summary>
public static class UserEntityMapper
{
    /// <summary>
    /// Convierte una entidad EF Core en un modelo de dominio.
    /// </summary>
    /// <param name="entity">Entidad de usuario de EF Core.</param>
    /// <returns>Modelo de dominio.</returns>
    public static User ToModel(this UserEntity entity)
    {
        return new User
        {
            Id = entity.Id,
            Name = entity.Name,
            UserName = entity.UserName,
            Email = entity.Email,
        };
    }

    /// <summary>
    /// Convierte un modelo de dominio en una entidad EF Core.
    /// </summary>
    /// <param name="user">Modelo de dominio.</param>
    /// <returns>Entidad preparada para EF Core.</returns>
    public static UserEntity ToEntity(this User user)
    {
        return new UserEntity
        {
            Id = user.Id,
            Name = user.Name,
            UserName = user.UserName,
            Email = user.Email,
        };
    }
}