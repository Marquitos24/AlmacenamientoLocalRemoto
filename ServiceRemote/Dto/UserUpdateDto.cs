using System.ComponentModel.DataAnnotations;

namespace ServiceRemote.Dto;

public class UserUpdateDto
{
    public int Id { get; set; }
    
    [MaxLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres")]
    [MinLength(1, ErrorMessage = "El nombre no puede tener menos de 1 caracter")]
    public string? Name { get; set; } = string.Empty;
    
    [MaxLength(150, ErrorMessage = "El nombre no puede tener más de 150 caracteres")]
    [MinLength(1, ErrorMessage = "El nombre no puede tener menos de 1 caracter")]
    public string? UserName { get; set; } = string.Empty;

    [EmailAddress]
    [MaxLength(200, ErrorMessage = "El Email no puede tener más de 200 caracteres")]
    public string? Email { get; set; } = string.Empty;
}