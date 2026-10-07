using Microsoft.EntityFrameworkCore;
using ServiceRemote.Entity;

namespace ServiceRemote.Repositories;

public class CrudRepository<T>(AppDbContext context) : ICrudRepository<T> where T : class
{
    // READ: Obtener todos los datos
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await context.Set<T>()
            .ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await context.Set<T>().FindAsync(id);
    }

    public async Task<T> CreateAsync(T entity)
    {
        await context.AddAsync(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    public async Task<T> UpdateAsync(int id, T entity)
    {
        var f = await GetByIdAsync(id);
        if (f is null)
        {
            throw new KeyNotFoundException($"No se encontró la entidad con ID {id}");
        }
        
        context.Set<T>().Update(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity is null)
        {
            throw new KeyNotFoundException($"No se encontró la entidad con ID {id}");
        }
        context.Set<T>().Remove(entity);
        await context.SaveChangesAsync();
    }
}