using MediatR;
using Notifications.Application.Dtos;
using Notifications.Application.Dtos.Bindings;

namespace Notifications.Application.Queries;

public record GetBindingByIdentityIdQuery : IRequest<BindingDto>
{
    public string IdentityId { get; init; } = null!;
}