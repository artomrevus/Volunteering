using MediatR;
using Notifications.Application.Dtos;
using Notifications.Application.Dtos.Bindings;

namespace Notifications.Application.Commands;

public record UpdateBindingCommand : IRequest<BindingDto>
{
    public string IdentityId { get; init; } = null!;

    public string Email { get; init; } = null!;
}