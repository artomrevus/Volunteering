using MediatR;
using Microsoft.Extensions.Logging;
using Notifications.Application.Dtos;
using Notifications.Application.Dtos.Bindings;
using Notifications.Application.Exceptions;
using Notifications.Application.Interfaces.Repositories;
using Notifications.Application.Mappers;
using Notifications.Domain.Entities;

namespace Notifications.Application.Commands.Handlers;

public class UpdateBindingCommandHandler(
    IBindingsRepository repository,
    ILogger<UpdateBindingCommandHandler> logger)
    : IRequestHandler<UpdateBindingCommand, BindingDto>
{
    public async Task<BindingDto> Handle(UpdateBindingCommand request, CancellationToken cancellationToken)
    {
        var binding = await repository.GetByIdentityIdAsync(request.IdentityId);
        if (binding is null)
        {
            logger.LogInformation(
                "Notification binding with identity id '{IdentityId}' was not found", 
                request.IdentityId);
            
            throw new NotFoundException($"Notification binding with identity id '{request.IdentityId}' was not found");
        }
        
        binding.UpdateEmail(request.Email);
        await repository.UpdateAsync(binding);

        logger.LogInformation(
            "Notification binding with identity id '{IdentityId}' was updated successfully to email '{Email}'", 
            binding.IdentityId, 
            binding.Email);
        
        return binding.ToBindingDto();
    }
}