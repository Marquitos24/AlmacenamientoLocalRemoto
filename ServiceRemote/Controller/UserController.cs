using ServiceRemote.Dto;

using ServiceRemote.Services;
using Microsoft.AspNetCore.Mvc;
using CSharpFunctionalExtensions;
using ServiceRemote.Errors;


namespace ServiceRemote.Dto;

/// <summary>
/// Controlador que redirige a traves de la ruta a diferentes funciones
/// </summary>
/// <param name="service">Recibe la logica de negocio</param>
[ApiController]
[Route("api/[controller]")]
public class UserController(UserService service) : ControllerBase
{
    
    [HttpGet]
    public async Task<ActionResult<List<UserResponseDto>>> GetAll()
    {
        var resultado = await service.GetAll();
        return resultado.Match(
            onSuccess: user => Ok(user),
            onFailure: error => error.ToHttpResult<List<UserResponseDto>>());
    }
    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserResponseDto>> GetById([FromRoute] int id)
    {
        var resultado = await service.GetById(id);
        return resultado.Match(
            onSuccess: user => Ok(user),
            onFailure: error => error.ToHttpResult<UserResponseDto>());
    }
    
    [HttpPost]
    public async Task<ActionResult<UserResponseDto>> Create([FromBody] UserCreateDto dto)
    {
        var resultado = await service.Create(dto);
        
        return resultado.Match(
            onSuccess: user => CreatedAtAction(nameof(GetById), new { id = user.Id}, user  ),
            onFailure: error => error.ToHttpResult<UserResponseDto>());
    }
    
    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserResponseDto>> Update(int id, [FromBody] UserUpdateDto dto)
    {
        var resultado = await service.Update(dto);
        
        return resultado.Match(
            onSuccess: user => Ok(user),
            onFailure: error => error.ToHttpResult<UserResponseDto>());
    }
    
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<object>> Delete([FromRoute] int id)
    {
        var resultado = await service.Delete(id);
        return resultado.Match(
            onSuccess: () => NoContent(),
            onFailure: error => error.ToHttpResult<object>());
    }
}