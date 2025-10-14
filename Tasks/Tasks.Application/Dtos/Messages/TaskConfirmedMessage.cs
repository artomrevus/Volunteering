namespace Tasks.Application.Dtos.Messages;

public class TaskConfirmedMessage
{
    public string VolunteerToNotifyId { get; init; } = null!;
    
    public string TaskTitle { get; init; } = null!;
}