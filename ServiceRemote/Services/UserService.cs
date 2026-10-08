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
            var users = await _repository.GetAll();
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

            var user = await _repository.GetById(id);
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
            var user = await _repository.Create(obj.ToModel());
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
            var existingUser = await _repository.GetById(obj.Id);
            if (existingUser is null)
            {
                return await NotifyAndReturnNotFoundFailure<UserResponseDto>(obj.Id);
            }

            var user = await _repository.Update(obj.ToModel());
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
    public async Task<Result<IEnumerable<UserResponseDto>, UserDomainError>> Delete(int id)
    {
        try
        {
            var existingUser = await _repository.GetById(id);
            if (existingUser is null)
            {
                return await NotifyAndReturnNotFoundFailure<IEnumerable<UserResponseDto>>(id);
            }

            await _repository.Delete(id);
            await _cache.RemoveAsync(id.ToString());
            await _notificador.NotifyDeleted(existingUser.ToDto());

            var users = await _repository.GetAll();
            return Result.Success<IEnumerable<UserResponseDto>, UserDomainError>(
                users.Select(user => user.ToDto()));
        }
        catch (Exception ex)
        {
            return await NotifyAndReturnStorageFailure<IEnumerable<UserResponseDto>>(ex);
        }
    }

    private async Task<Result<T, UserDomainError>> NotifyAndReturnNotFoundFailure<T>(int id)
    {
        var error = DomainErrors.NotFound(id);
        await _notificador.NotifyError(error);

        return Result.Failure<T, UserDomainError>(error);
    }

    private async Task<Result<T, UserDomainError>> NotifyAndReturnStorageFailure<T>(Exception ex)
    {
        _logger.LogError(ex, "Error de almacenamiento en el servicio de usuarios");

        var error = DomainErrors.Storage(ex);
        await _notificador.NotifyError(error);

        return Result.Failure<T, UserDomainError>(error);
    }
}
