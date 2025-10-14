using Auth.Domain.Entities;

namespace Auth.Application.Interfaces.Tokens;

public interface ITokensService
{
    string Generate(UserEntity user);
}