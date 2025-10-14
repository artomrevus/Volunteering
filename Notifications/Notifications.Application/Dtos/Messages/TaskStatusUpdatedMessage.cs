namespace Notifications.Application.Dtos.Messages;

public class TaskStatusUpdatedMessage
{
    public string MilitaryToNotifyId { get; init; } = null!;
    
    public string TaskTitle { get; init; } = null!;
    
    public string OldTaskStatus { get; init; } = null!;
    
    public string NewTaskStatus { get; init; } = null!;
}