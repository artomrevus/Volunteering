using MediatR;
using Microsoft.Extensions.Logging;
using Tasks.Application.Dtos;
using Tasks.Application.Dtos.Messages;
using Tasks.Application.Exceptions;
using Tasks.Application.Interfaces;
using Tasks.Application.Interfaces.Queues;
using Tasks.Application.Interfaces.Repositories;
using Tasks.Application.Mappers;
using Tasks.Domain.Entities;

namespace Tasks.Application.Commands.Handlers;

public class UpdateTaskStatusCommandHandler(
    ITasksRepository repository,
    ITasksQueueSender queueSender,
    ILogger<UpdateTaskStatusCommandHandler> logger) 
    : IRequestHandler<UpdateTaskStatusCommand, TaskDto>
{
    public async Task<TaskDto> Handle(UpdateTaskStatusCommand request, CancellationToken cancellationToken)
    {
        var task = await repository.GetByIdAsync(request.TaskId);
        if (task is null)
        {
            logger.LogInformation(
                "Task with id '{TaskId}' was not found", 
                request.TaskId);
            
            throw new NotFoundException($"Task with Id '{request.TaskId}' was not found");
        }
        
        if (request.VolunteerId != task.VolunteerId)
        {
            logger.LogInformation(
                "Volunteer with id '{VolunteerId}' doesn't have permission to update task with id '{TaskId}'", 
                request.VolunteerId,
                request.TaskId);
            
            throw new ForbiddenException(
                $"Volunteer with id '{request.VolunteerId}' doesn't have permission to update task with id '{request.TaskId}'");
        }
        
        var oldTaskStatus = task.Status;
        
        task.UpdateStatus(request.Status);
        await repository.UpdateAsync(task);

        var message = new TaskStatusUpdatedMessage
        {
            MilitaryToNotifyId = task.MilitaryId,
            TaskTitle = task.Title,
            OldTaskStatus = oldTaskStatus,
            NewTaskStatus = task.Status
        };
        
        await queueSender.SendTaskStatusUpdatedMessageAsync(message);
            
        logger.LogInformation(
            "Task with id '{TaskId}' for military with id '{MilitaryId}' status was updated to '{TaskStatus}' successfully", 
            task.Id,
            task.MilitaryId,
            task.Status);
        
        return task.ToTaskDto();
    }
}