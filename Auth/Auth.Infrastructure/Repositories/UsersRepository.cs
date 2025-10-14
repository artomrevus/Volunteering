using Auth.Application.Interfaces.Repositories;
using Auth.Domain.Entities;
using Auth.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Repositories;

public class UsersRepository(MongoDbContext context) : IUsersRepository
{
    public async Task<UserEntity?> GetByUsernameAsync(string username)
    {
        return await context.Users.FirstOrDefaultAsync(x => x.Username == username);
    }

    public async Task<UserEntity> AddAsync(UserEntity user)
    {
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }
}