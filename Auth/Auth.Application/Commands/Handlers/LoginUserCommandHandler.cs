using Auth.Application.Dtos;
using Auth.Application.Exceptions;
using Auth.Application.Interfaces.Hashers;
using Auth.Application.Interfaces.Repositories;
using Auth.Application.Interfaces.Tokens;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Auth.Application.Commands.Handlers;

public class LoginUserCommandHandler(
    IUsersRepository repository,
    IPasswordHasher passwordHasher,
    ITokensService tokensService,
    ILogger<LoginUserCommandHandler> logger)
    : IRequestHandler<LoginUserCommand, TokenDto>
{
    public async Task<TokenDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await repository.GetByUsernameAsync(request.Username);
        if (user is null || passwordHasher.Hash(request.Password) != user.PasswordHash)
        {
            logger.LogInformation(
                "Login: incorrect username or password for user with username '{Username}'", 
                request.Username);
            
            throw new UnauthorizedException("Incorrect username or password");
        }

        var token = tokensService.Generate(user);
        
        logger.LogInformation(
            "Login success for user with username '{Username}'", 
            user.Username);
        
        return new TokenDto { Token = token };
    }
}