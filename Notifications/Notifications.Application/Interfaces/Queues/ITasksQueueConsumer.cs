using Notifications.Application.Dtos.Messages;

namespace Notifications.Application.Interfaces.Queues;

public interface ITasksQueueConsumer
{
    public Func<TaskStartedMessageDto, Task>? HandleTaskStartedAsync { get; set; }
    
    public Func<TaskStatusUpdatedMessageDto, Task>? HandleTaskStatusUpdatedAsync { get; set; }
    
    public Func<TaskConfirmedMessageDto, Task>? HandleTaskConfirmedAsync { get; set; }
    
    Task StartConsumingAsync(CancellationToken cancellationToken);
    
    Task StopConsumingAsync();
}