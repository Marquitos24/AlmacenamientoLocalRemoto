using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceRemote.Entity;

[Table("Users")]
public class UserEntity
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required (ErrorMessage =  "El nombre es obligatorio")]
    [Column("name")]
    [MaxLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres")]
    [MinLength(1, ErrorMessage = "El nombre no puede tener menos de 1 caracter")]
    public string Name { get; set; } = "";

    [Required]
    [Column("userName")]
    [MaxLength(150, ErrorMessage = "El nombre no puede tener más de 150 caracteres")]
    [MinLength(1, ErrorMessage = "El nombre no puede tener menos de 1 caracter")]
    public string UserName { get; set; } = "";

    [Required]
    [EmailAddress]
    [Column("email")]
    [MaxLength(200, ErrorMessage = "El Email no puede tener más de 200 caracteres")]
    public string Email { get; set; } = "";
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    //añadimos ? ya que cuando se crea un user aun no ha sido ni actualizado ni borrado
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [Column("is_deleted")]
    public bool IsDeleted { get; set; } = false;

    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }
}