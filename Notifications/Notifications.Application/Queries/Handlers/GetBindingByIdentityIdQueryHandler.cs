using MediatR;
using Microsoft.Extensions.Logging;
using Notifications.Application.Dtos;
using Notifications.Application.Dtos.Bindings;
using Notifications.Application.Exceptions;
using Notifications.Application.Interfaces.Repositories;
using Notifications.Application.Mappers;

namespace Notifications.Application.Queries.Handlers;

public class GetBindingByIdentityIdQueryHandler(
    IBindingsRepository repository,
    ILogger<GetBindingByIdentityIdQueryHandler> logger)
    : IRequestHandler<GetBindingByIdentityIdQuery, BindingDto>
{
    public async Task<BindingDto> Handle(GetBindingByIdentityIdQuery request, CancellationToken cancellationToken)
    {
        var binding = await repository.GetByIdentityIdAsync(request.IdentityId);
        if (binding is null)
        {
            logger.LogInformation(
                "Notification binding with identity id '{IdentityId}' was not found", 
                request.IdentityId);
            
            throw new NotFoundException($"Notification binding with identity id '{request.IdentityId}' was not found");
        }

        logger.LogInformation(
            "Notification binding with identity id '{IdentityId}' and email '{Email}' was retrieved successfully", 
            binding.IdentityId,
            binding.Email);
        
        return binding.ToBindingDto();
    }
}