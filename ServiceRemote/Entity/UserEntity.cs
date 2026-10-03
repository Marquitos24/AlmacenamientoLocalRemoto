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

    [Required]
    [Column("name")]
    [MaxLength(100)]
    public string Name { get; set; } = "";

    [Required]
    [Column("userName")]
    [MaxLength(150)]
    public string UserName { get; set; } = "";

    [Required]
    [EmailAddress]
    [Column("email")]
    [MaxLength(200)]
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