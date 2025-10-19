using MediatR;
using Microsoft.Extensions.Logging;
using Tasks.Application.Dtos;
using Tasks.Application.Dtos.Messages;
using Tasks.Application.Dtos.Tasks;
using Tasks.Application.Exceptions;
using Tasks.Application.Interfaces;
using Tasks.Application.Interfaces.Clients;
using Tasks.Application.Interfaces.Queues;
using Tasks.Application.Interfaces.Repositories;
using Tasks.Application.Mappers;

namespace Tasks.Application.Commands.Handlers;

public class ConfirmTaskCommandHandler(
    ITasksRepository repository,
    ITasksQueueSender queueSender,
    INotificationsMicroserviceClient notificationsClient,
    ILogger<ConfirmTaskCommandHandler> logger) 
    : IRequestHandler<ConfirmTaskCommand, TaskDto>
{
    public async Task<TaskDto> Handle(ConfirmTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await repository.GetByIdAsync(request.TaskId);
        if (task is null)
        {
            logger.LogInformation(
                "Task with id '{TaskId}' was not found", 
                request.TaskId);
            
            throw new NotFoundException($"Task with Id '{request.TaskId}' was not found");
        }
        
        if (request.MilitaryId != task.MilitaryId)
        {
            logger.LogInformation(
                "Military with id '{MilitaryId}' doesn't have permission to confirm task with id '{TaskId}'", 
                request.MilitaryId,
                request.TaskId);
            
            throw new ForbiddenException(
                $"Military with id '{request.MilitaryId}' doesn't have permission to confirm task with id '{request.TaskId}'");
        }

        task.Confirm();
        await repository.UpdateAsync(task);
        
        var message = new TaskConfirmedMessageDto
        {
            VolunteerToNotifyId = task.VolunteerId!,
            TaskTitle = task.Title,
        };
        
        //await queueSender.SendTaskConfirmedMessageAsync(message);
        await notificationsClient.SendTaskConfirmedNotificationAsync(message);
        
        logger.LogInformation(
            "Task with id '{TaskId}' for military with id '{MilitaryId}' was confirmed successfully", 
            task.Id,
            task.MilitaryId);
        
        return task.ToTaskDto();
    }
}