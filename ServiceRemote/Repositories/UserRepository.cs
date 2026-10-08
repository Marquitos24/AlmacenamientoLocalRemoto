using Microsoft.EntityFrameworkCore;
using ServiceRemote.Entity;

namespace ServiceRemote.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task<List<UserEntity>> GetAllAsync()
    {
        return await context.Users
            .ToListAsync();
    }
    public async Task<UserEntity?> GetByIdAsync(int id)
    {
        return await context.Users.FindAsync(id);
    }

    public async Task<UserEntity> CreateAsync(UserEntity entity)
    {
        await context.AddAsync(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    public async Task<UserEntity> UpdateAsync(int id, UserEntity entity)
    {
        var f = await GetByIdAsync(id);
        if (f is null)
        {
            throw new KeyNotFoundException($"No se encontró la entidad con ID {id}");
        }
        
        context.Users.Update(entity);
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
        context.Users.Remove(entity);
        await context.SaveChangesAsync();
    }
}