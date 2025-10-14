using Tasks.Application.Dtos;
using Tasks.Application.Dtos.Messages;

namespace Tasks.Application.Interfaces.Queues;

public interface ITasksQueueSender
{
    Task SendTaskStartedMessageAsync(TaskStartedMessage message);
    
    Task SendTaskStatusUpdatedMessageAsync(TaskStatusUpdatedMessage message);
    
    Task SendTaskConfirmedMessageAsync(TaskConfirmedMessage message);
}