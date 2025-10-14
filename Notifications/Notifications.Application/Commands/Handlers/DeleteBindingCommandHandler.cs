using MediatR;
using Microsoft.Extensions.Logging;
using Notifications.Application.Exceptions;
using Notifications.Application.Interfaces.Repositories;

namespace Notifications.Application.Commands.Handlers;

public class DeleteBindingCommandHandler(
    IBindingsRepository repository,
    ILogger<DeleteBindingCommandHandler> logger)
    : IRequestHandler<DeleteBindingCommand>
{
    public async Task Handle(DeleteBindingCommand request, CancellationToken cancellationToken)
    {
        var binding = await repository.GetByIdentityIdAsync(request.IdentityId);
        if (binding is null)
        {
            logger.LogInformation(
                "Notification binding with identity id '{IdentityId}' was not found", 
                request.IdentityId);
            
            throw new NotFoundException($"Notification binding with identity id '{request.IdentityId}' was not found");
        }
        
        await repository.DeleteAsync(binding);
        
        logger.LogInformation(
            "Notification binding with identity id '{IdentityId}' and email '{Email}' was deleted successfully", 
            binding.IdentityId, 
            binding.Email);
    }
}