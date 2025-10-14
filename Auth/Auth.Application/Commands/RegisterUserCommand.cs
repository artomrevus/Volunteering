using Auth.Application.Dtos;
using MediatR;

namespace Auth.Application.Commands;

public record RegisterUserCommand : IRequest<TokenDto>
{
    public string Username { get; init; } = null!;

    public string Password { get; init; } = null!;
    
    public string Role { get; init; } = null!;
}