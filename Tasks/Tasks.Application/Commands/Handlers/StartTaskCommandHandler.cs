using MediatR;
using Microsoft.Extensions.Logging;
using Tasks.Application.Dtos;
using Tasks.Application.Dtos.Messages;
using Tasks.Application.Dtos.Tasks;
using Tasks.Application.Exceptions;
using Tasks.Application.Interfaces.Clients;
using Tasks.Application.Interfaces.Queues;
using Tasks.Application.Interfaces.Repositories;
using Tasks.Application.Mappers;

namespace Tasks.Application.Commands.Handlers;

public class StartTaskCommandHandler(
    ITasksRepository repository,
    ITasksQueueSender queueSender,
    INotificationsMicroserviceClient notificationsClient,
    ILogger<StartTaskCommandHandler> logger)
    : IRequestHandler<StartTaskCommand, TaskDto>
{
    public async Task<TaskDto> Handle(StartTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await repository.GetByIdAsync(request.TaskId);
        if (task is null)
        {
            logger.LogInformation(
                "Task with id '{TaskId}' was not found", 
                request.TaskId);
            
            throw new NotFoundException($"Task with Id '{request.TaskId}' was not found");
        }

        task.Start(request.VolunteerId);
        await repository.UpdateAsync(task);

        var message = new TaskStartedMessageDto
        {
            MilitaryToNotifyId = task.MilitaryId,
            TaskTitle = task.Title,
        };
        
        //await queueSender.SendTaskStartedMessageAsync(message);
        await notificationsClient.SendTaskStartedNotificationAsync(message);
        
        logger.LogInformation(
            "Task with id '{TaskId}' for military with id '{MilitaryId}' was started successfully", 
            task.Id,
            task.MilitaryId);
        
        return task.ToTaskDto();
    }
}