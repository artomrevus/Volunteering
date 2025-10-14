using Notifications.Application.Dtos;
using Notifications.Application.Dtos.Bindings;
using Notifications.Domain.Entities;

namespace Notifications.Application.Mappers;

public static class TasksMapper
{
    public static BindingDto ToBindingDto(this BindingEntity binding)
    {
        return new BindingDto
        {
            IdentityId = binding.IdentityId,
            Email = binding.Email,
        };
    }
    
    public static IEnumerable<BindingDto> ToBindingsDtos(this IEnumerable<BindingEntity> bindings)
    {
        return bindings.Select(ToBindingDto);
    }
}