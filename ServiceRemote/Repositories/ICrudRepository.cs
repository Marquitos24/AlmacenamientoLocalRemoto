namespace ServiceRemote.Repositories;

public interface ICrudRepository<T>
{
    Task<IEnumerable<T>> GetAll();
    Task<T?> GetById(int id);
    Task<T> Create(T obj);
    Task<T> Update(T obj);
    Task Delete(int id);
}
