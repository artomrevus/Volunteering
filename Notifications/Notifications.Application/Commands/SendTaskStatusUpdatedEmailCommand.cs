using MediatR;

namespace Notifications.Application.Commands;

public class SendTaskStatusUpdatedEmailCommand : IRequest
{
    public string EmailTo { get; init; } = null!;
    
    public string TaskTitle { get; init; } = null!;
    
    public string OldTaskStatus { get; init; } = null!;
    
    public string NewTaskStatus { get; init; } = null!;
}