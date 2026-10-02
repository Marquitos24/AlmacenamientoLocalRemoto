using System;
using ServiceRemote.Models;

namespace ServiceRemote.Dto;

public static class UserDtoEctensionMapper
{
    public static UserDto ToDto(this User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            UserName = user.UserName,
            Email = user.Email,
            CreateAt = DateTime.Now,
            UpdateAt = DateTime.Now,
            IsDelete = false,
            DeleteAt = null
        };
    }
    public static User ToModel(this Dto.UserDto dto)
    {
        return new User
        {
            Id = dto.Id,
            Name = dto.Name,
            UserName = dto.UserName,
            Email = dto.Email,
        };
    }
    public static string ToJson(this UserDto dto)
    {
        return System.Text.Json.JsonSerializer.Serialize(dto);
    }
}