namespace Notifications.Application.Dtos.Messages;

public class TaskConfirmedMessageDto
{
    public string VolunteerToNotifyId { get; init; } = null!;
    
    public string TaskTitle { get; init; } = null!;
}