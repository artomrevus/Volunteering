using Auth.Application.Commands;
using Auth.Application.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers;

[Route("[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto dto)
    {
        var command = new RegisterUserCommand
        { 
            Username = dto.Username,
            Password = dto.Password,
            Role = dto.Role,
        };
        
        var result = await mediator.Send(command);
        return Ok(result);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginDto dto)
    {
        var command = new LoginUserCommand
        { 
            Username = dto.Username,
            Password = dto.Password,
        };
        
        var result = await mediator.Send(command);
        return Ok(result);
    }
}