namespace Tasks.Application.Dtos.Messages;

public class TaskStatusUpdatedMessageDto
{
    public string MilitaryToNotifyId { get; init; } = null!;
    
    public string TaskTitle { get; init; } = null!;
    
    public string OldTaskStatus { get; init; } = null!;
    
    public string NewTaskStatus { get; init; } = null!;
}