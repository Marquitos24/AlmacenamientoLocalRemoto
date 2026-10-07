using ServiceRemote.Dto;
using ServiceRemote.Errors;

namespace ServiceRemote.Services;

/// <summary>
/// Implementacion de la interfaz IService para usuarios
/// </summary>
public interface IUserService : IService<UserResponseDto, int , UserCreateDto, UserUpdateDto, UserDomainError > {}