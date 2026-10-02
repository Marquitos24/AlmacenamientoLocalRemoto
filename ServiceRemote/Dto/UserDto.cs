using System;

namespace ServiceRemote.Dto;

public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public DateTime CreateAt { get; set; }
    public DateTime UpdateAt { get; set; }
    public bool IsDelete { get; set; }
    public DateTime? DeleteAt { get; set; }
}