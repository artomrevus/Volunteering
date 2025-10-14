using Microsoft.EntityFrameworkCore;
using Notifications.Application.Interfaces.Repositories;
using Notifications.Domain.Entities;
using Notifications.Infrastructure.Persistence;

namespace Notifications.Infrastructure.Repositories;

public class BindingsRepository(MongoDbContext context) : IBindingsRepository
{
    public async Task<BindingEntity?> GetByIdentityIdAsync(string identityId)
    {
        return await context.Bindings.FirstOrDefaultAsync(x => x.IdentityId == identityId);
    }
    
    public async Task<BindingEntity> AddAsync(BindingEntity binding)
    {
        context.Bindings.Add(binding);
        await context.SaveChangesAsync();
        return binding;
    }
    
    public async Task UpdateAsync(BindingEntity binding)
    {
        context.Bindings.Update(binding);
        await context.SaveChangesAsync();
    }
    
    public async Task DeleteAsync(BindingEntity binding)
    {
        context.Bindings.Remove(binding);
        await context.SaveChangesAsync();
    }
}