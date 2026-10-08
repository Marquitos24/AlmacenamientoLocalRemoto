using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using ServiceRemote.Cache;
using ServiceRemote.Dto;
using ServiceRemote.Errors;
using ServiceRemote.Notifications.Emiter;
using ServiceRemote.Repositories;

namespace ServiceRemote.Services;

/// <summary>
/// Implementación del servicio de usuarios.
/// </summary>
public class UserService : IUserService
{
    private readonly IUserCache _cache;
    private readonly IUserRepository _repository;
    private readonly ILogger _logger;
    private readonly INotificationService<UserResponseDto, UserDomainError> _notificador;

    public UserService(
        IUserCache cache,
        IUserRepository repository,
        ILogger logger,
        INotificationService<UserResponseDto, UserDomainError> notificador)
    {
        _cache = cache;
        _repository = repository;
        _logger = logger;
        _notificador = notificador;
    }

    /// <inheritdoc />
    public async Task<Result<IEnumerable<UserResponseDto>, UserDomainError>> GetAll()
    {
        try
        {
            var users = await _repository.GetAllAsync();
            
            return Result.Success<IEnumerable<UserResponseDto>, UserDomainError>(
                users.Select(user => user.ToDto()));
        }
        catch (Exception ex)
        {
            return await NotifyAndReturnStorageFailure<IEnumerable<UserResponseDto>>(ex);
        }
    }

    /// <inheritdoc />
    public async Task<Result<UserResponseDto, UserDomainError>> GetById(int id)
    {
        try
        {
            var cachedUser = await _cache.GetAsync(id.ToString());
            if (cachedUser is not null)
            {
                return Result.Success<UserResponseDto, UserDomainError>(cachedUser.ToDto());
            }

            var user = await _repository.GetByIdAsync(id);
            if (user is null)
            {
                return await NotifyAndReturnNotFoundFailure<UserResponseDto>(id);
            }

            await _cache.SetAsync(user);
            return Result.Success<UserResponseDto, UserDomainError>(user.ToDto());
        }
        catch (Exception ex)
        {
            return await NotifyAndReturnStorageFailure<UserResponseDto>(ex);
        }
    }

    /// <inheritdoc />
    public async Task<Result<UserResponseDto, UserDomainError>> Create(UserCreateDto obj)
    {
        try
        {
            var user = await _repository.CreateAsync(obj.ToModel());
            var dto = user.ToDto();

            await _cache.SetAsync(user);
            await _notificador.NotifyCreated(dto);

            return Result.Success<UserResponseDto, UserDomainError>(dto);
        }
        catch (Exception ex)
        {
            return await NotifyAndReturnStorageFailure<UserResponseDto>(ex);
        }
    }

    /// <inheritdoc />
    public async Task<Result<UserResponseDto, UserDomainError>> Update(UserUpdateDto obj)
    {
        try
        {
            var existingUser = await _repository.GetByIdAsync(obj.Id);
            if (existingUser is null)
            {
                return await NotifyAndReturnNotFoundFailure<UserResponseDto>(obj.Id);
            }

            var user = await _repository.UpdateAsync(obj.ToModel());
            var dto = user.ToDto();

            await _cache.SetAsync(user);
            await _notificador.NotifyUpdated(dto);

            return Result.Success<UserResponseDto, UserDomainError>(dto);
        }
        catch (Exception ex)
        {
            return await NotifyAndReturnStorageFailure<UserResponseDto>(ex);
        }
    }

    /// <inheritdoc />
    public async Task<UnitResult<UserDomainError>> Delete(int id)
    {
        try
        {
            var existingEntity = await _repository.GetByIdAsync(id);
            if (existingEntity is null)
            {
                return await NotifyAndReturnNotFoundUnitFailure(id);
            }
            var existingUser = existingEntity.ToModel();
            
            await _repository.DeleteAsync(id);
            
            await _cache.RemoveAsync(id.ToString());
            
            await _notificador.NotifyDeleted(existingUser.ToDto());
            
            return UnitResult.Success<UserDomainError>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error de almacenamiento en el servicio de usuarios");

            var error = DomainErrors.Storage(ex);

            await _notificador.NotifyError(error);

            return UnitResult.Failure<UserDomainError>(error);
        }
    }

    private async Task<Result<T, UserDomainError>> NotifyAndReturnNotFoundFailure<T>(int id)
    {
        var error = DomainErrors.NotFound(id);
        await _notificador.NotifyError(error);

        return Result.Failure<T, UserDomainError>(error);
    }
    
    private async Task<UnitResult<UserDomainError>> NotifyAndReturnNotFoundUnitFailure(int id)
    {
        var error = DomainErrors.NotFound(id);
        await _notificador.NotifyError(error);

        return UnitResult.Failure<UserDomainError>(error);
    }

    private async Task<Result<T, UserDomainError>> NotifyAndReturnStorageFailure<T>(Exception ex)
    {
        _logger.LogError(ex, "Error de almacenamiento en el servicio de usuarios");

        var error = DomainErrors.Storage(ex);
        await _notificador.NotifyError(error);

        return Result.Failure<T, UserDomainError>(error);
    }
}
