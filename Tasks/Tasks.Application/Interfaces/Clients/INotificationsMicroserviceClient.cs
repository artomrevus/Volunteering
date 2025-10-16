using Tasks.Application.Dtos.Messages;

namespace Tasks.Application.Interfaces.Clients;

public interface INotificationsMicroserviceClient
{
    Task SendTaskStartedNotificationAsync(TaskStartedMessageDto dto);
    
    Task SendTaskStatusUpdatedNotificationAsync(TaskStatusUpdatedMessageDto dto);
    
    Task SendTaskConfirmedNotificationAsync(TaskConfirmedMessageDto dto);
}