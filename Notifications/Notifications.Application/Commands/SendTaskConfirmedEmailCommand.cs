using MediatR;

namespace Notifications.Application.Commands;

public class SendTaskConfirmedEmailCommand : IRequest
{
    public string EmailTo { get; init; } = null!;
    
    public string TaskTitle { get; init; } = null!;
}