using CSharpFunctionalExtensions;
using ServiceRemote.Cache;
using ServiceRemote.Dto;
using ServiceRemote.Errors;
using ServiceRemote.Models;
using ServiceRemote.Notifications.Emiter;
using ServiceRemote.Repositories;

namespace ServiceRemote.Services;

/// <summary>
/// Implementación del servicio de usuarios.
/// </summary>
public class UserService : IUserService
{
    private IUserCache _cache;
    private IUserRepository _repository;
    private ILogger _logger;
    private INotificationService<UserResponseDto, UserDomainError> _notificador;
    
    public UserService(IUserCache cache,IUserRepository repository, ILogger logger, INotificationService<UserResponseDto,UserDomainError> notificador)
    {
        _cache = cache;
        _repository = repository;
        _logger = logger;
        _notificador = notificador;
    }
    
    /// <inehritdoc />
    public Task<Result<IEnumerable<UserResponseDto>, UserDomainError>> GetAll()
    {
        throw new NotImplementedException();
    }

    /// <inehritdoc />
    public Task<Result<UserResponseDto, UserDomainError>> GetById(int id)
    {
        throw new NotImplementedException();
    }

    /// <inehritdoc />
    public Task<Result<UserResponseDto, UserDomainError>> Create(UserCreateDto obj)
    {
        throw new NotImplementedException();
    }

    /// <inehritdoc />
    public Task<Result<UserResponseDto, UserDomainError>> Update(UserUpdateDto obj)
    {
        throw new NotImplementedException();
    }

    /// <inehritdoc />
    public Task<Result<IEnumerable<UserResponseDto>, UserDomainError>> Delete(int id)
    {
        throw new NotImplementedException();
    }
}