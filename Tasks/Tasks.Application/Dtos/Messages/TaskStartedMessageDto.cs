namespace Tasks.Application.Dtos.Messages;

public class TaskStartedMessageDto
{
    public string MilitaryToNotifyId { get; init; } = null!;
    
    public string TaskTitle { get; init; } = null!;
}