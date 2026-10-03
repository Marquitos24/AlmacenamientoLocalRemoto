namespace ServiceRemote.Repositories;

public interface ICrudRepository<T>
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(int id, T entity);
    Task DeleteAsync(int id);
}