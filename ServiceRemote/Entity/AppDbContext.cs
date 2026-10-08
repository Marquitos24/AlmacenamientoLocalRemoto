using Microsoft.EntityFrameworkCore;

namespace ServiceRemote.Entity;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<UserEntity> Users => Set<UserEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // reglas que no se describen directamente en el entity
        modelBuilder.Entity<UserEntity>(entity =>
        {
            // ambos atributos son unicos
            entity.HasIndex(p => p.UserName).IsUnique();
            entity.HasIndex(p => p.Email).IsUnique();
            
            // Para no borrar los datos de un usurio en laBD ponemos un diltro para que si se elimina
            // los descarte de las otras consultas (en un filtro)
            entity.HasQueryFilter(u => !u.IsDeleted);
            });
    }
}
