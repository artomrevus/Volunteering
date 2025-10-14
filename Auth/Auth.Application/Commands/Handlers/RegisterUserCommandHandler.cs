using Auth.Application.Dtos;
using Auth.Application.Exceptions;
using Auth.Application.Interfaces.Hashers;
using Auth.Application.Interfaces.Repositories;
using Auth.Application.Interfaces.Tokens;
using Auth.Domain.Entities;
using MediatR;

namespace Auth.Application.Commands.Handlers;

public class RegisterUserCommandHandler(
    IUsersRepository repository,
    IPasswordHasher passwordHasher,
    ITokensService tokensService)
    : IRequestHandler<RegisterUserCommand, TokenDto>
{
    public async Task<TokenDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        if (await repository.GetByUsernameAsync(request.Username) is not null)
        {
            throw new ConflictException($"User with username '{request.Username}' already exists");
        }
        
        var user = new UserEntity(
            request.Username,
            passwordHasher.Hash(request.Password),
            request.Role);

        var createdUser = await repository.AddAsync(user);

        var token = tokensService.Generate(createdUser);
        
        return new TokenDto { Token = token };
    }
}