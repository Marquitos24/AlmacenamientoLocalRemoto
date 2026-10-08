using System.ComponentModel.DataAnnotations;

namespace ServiceRemote.Dto;

public class UserCreateDto
{
    [Required (ErrorMessage =  "El nombre es obligatorio")]
    [MaxLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres")]
    [MinLength(1, ErrorMessage = "El nombre no puede tener menos de 1 caracter")]
    public string Name { get; set; } = string.Empty;
    
    [Required (ErrorMessage =  "El nombre de usuario es obligatorio")]
    [MaxLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres")]
    [MinLength(1, ErrorMessage = "El nombre no puede tener menos de 1 caracter")]
    public string UserName { get; set; } = string.Empty;
    
    [Required (ErrorMessage =  "El Email es obligatorio")]
    [EmailAddress]
    [MaxLength(200, ErrorMessage = "El Email no puede tener más de 200 caracteres")]
    public string Email { get; set; } = string.Empty;
}