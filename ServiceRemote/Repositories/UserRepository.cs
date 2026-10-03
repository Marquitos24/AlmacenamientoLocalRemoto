using ServiceRemote.Entity;

namespace ServiceRemote.Repositories;

public class UserRepository(AppDbContext context) : CrudRepository<UserEntity>(context), IUserRepository
{
    
}