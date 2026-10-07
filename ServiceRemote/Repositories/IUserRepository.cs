using ServiceRemote.Entity;
using ServiceRemote.Models;

namespace ServiceRemote.Repositories;

public interface IUserRepository
{
    Task<List<UserEntity>> GetAllAsync();
    Task<UserEntity?> GetByIdAsync(int id);
    Task<UserEntity> CreateAsync(UserEntity entity);
    Task<UserEntity> UpdateAsync(int id, UserEntity entity);
    Task DeleteAsync(int id);
}