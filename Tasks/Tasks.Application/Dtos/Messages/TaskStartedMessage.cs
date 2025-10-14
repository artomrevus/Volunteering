namespace Tasks.Application.Dtos.Messages;

public class TaskStartedMessage
{
    public string MilitaryToNotifyId { get; init; } = null!;
    
    public string TaskTitle { get; init; } = null!;
}