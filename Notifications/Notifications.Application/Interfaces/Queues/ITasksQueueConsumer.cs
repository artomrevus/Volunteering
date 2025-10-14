using Notifications.Application.Dtos.Messages;

namespace Notifications.Application.Interfaces.Queues;

public interface ITasksQueueConsumer
{
    public Func<TaskStartedMessage, Task>? HandleTaskStartedAsync { get; set; }
    
    public Func<TaskStatusUpdatedMessage, Task>? HandleTaskStatusUpdatedAsync { get; set; }
    
    public Func<TaskConfirmedMessage, Task>? HandleTaskConfirmedAsync { get; set; }
    
    Task StartConsumingAsync(CancellationToken cancellationToken);
    
    Task StopConsumingAsync();
}