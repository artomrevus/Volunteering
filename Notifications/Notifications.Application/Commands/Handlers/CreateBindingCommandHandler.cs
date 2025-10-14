using MediatR;
using Microsoft.Extensions.Logging;
using Notifications.Application.Dtos;
using Notifications.Application.Dtos.Bindings;
using Notifications.Application.Exceptions;
using Notifications.Application.Interfaces.Repositories;
using Notifications.Application.Mappers;
using Notifications.Domain.Entities;

namespace Notifications.Application.Commands.Handlers;

public class CreateBindingCommandHandler(
    IBindingsRepository repository,
    ILogger<CreateBindingCommandHandler> logger) 
    : IRequestHandler<CreateBindingCommand, BindingDto>
{
    public async Task<BindingDto> Handle(CreateBindingCommand request, CancellationToken cancellationToken)
    {
        if (await repository.GetByIdentityIdAsync(request.IdentityId) is not null)
        {
            logger.LogInformation(
                "Notification binding with identity id '{IdentityId}' already exists", 
                request.IdentityId);
            
            throw new ConflictException($"Notification binding with identity id '{request.IdentityId}' already exists");
        }
        
        var binding = new BindingEntity(
            request.IdentityId, 
            request.Email);
        
        var createdBinding = await repository.AddAsync(binding);
        
        logger.LogInformation(
            "Notification binding with identity id '{IdentityId}' and email '{Email}' was created successfully", 
            request.IdentityId, 
            request.Email);
        
        return createdBinding.ToBindingDto();
    }
}