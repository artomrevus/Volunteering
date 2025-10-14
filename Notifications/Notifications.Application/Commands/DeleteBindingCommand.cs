using MediatR;
using Notifications.Application.Dtos;

namespace Notifications.Application.Commands;

public record DeleteBindingCommand : IRequest
{
    public string IdentityId { get; init; } = null!;
}