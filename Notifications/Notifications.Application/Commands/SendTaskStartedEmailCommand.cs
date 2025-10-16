using MediatR;

namespace Notifications.Application.Commands;

public class SendTaskStartedEmailCommand : IRequest
{
    public string EmailTo { get; init; } = null!;
    
    public string TaskTitle { get; init; } = null!;
}