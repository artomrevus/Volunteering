using Tasks.Application.Dtos;
using Tasks.Application.Dtos.Messages;

namespace Tasks.Application.Interfaces.Queues;

public interface ITasksQueueSender
{
    Task SendTaskStartedMessageAsync(TaskStartedMessageDto messageDto);
    
    Task SendTaskStatusUpdatedMessageAsync(TaskStatusUpdatedMessageDto messageDto);
    
    Task SendTaskConfirmedMessageAsync(TaskConfirmedMessageDto messageDto);
}