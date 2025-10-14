using Notifications.Domain.Entities;

namespace Notifications.Application.Interfaces.Repositories;

public interface IBindingsRepository
{
    Task<BindingEntity?> GetByIdentityIdAsync(string identityId);
    
    Task<BindingEntity> AddAsync(BindingEntity binding);
    
    Task UpdateAsync(BindingEntity binding);
    
    Task DeleteAsync(BindingEntity binding);
}