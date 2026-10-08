using System;
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
            Id = dto.Id,
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