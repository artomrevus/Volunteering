using Auth.Domain.Entities;

namespace Auth.Application.Interfaces.Repositories;

public interface IUsersRepository
{
    Task<UserEntity?> GetByUsernameAsync(string username);
    
    Task<UserEntity> AddAsync(UserEntity user);
}