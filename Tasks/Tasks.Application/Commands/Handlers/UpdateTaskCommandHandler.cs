using MediatR;
using Microsoft.Extensions.Logging;
using Tasks.Application.Dtos;
using Tasks.Application.Exceptions;
using Tasks.Application.Interfaces.Repositories;
using Tasks.Application.Mappers;

namespace Tasks.Application.Commands.Handlers;

public class UpdateTaskCommandHandler(
    ITasksRepository repository,
    ILogger<UpdateTaskCommandHandler> logger)
    : IRequestHandler<UpdateTaskCommand, TaskDto>
{
    public async Task<TaskDto> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
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
                "Military with id '{MilitaryId}' doesn't have permission to update task with id '{TaskId}'", 
                request.MilitaryId,
                request.TaskId);
            
            throw new ForbiddenException(
                $"Military with id '{request.MilitaryId}' doesn't have permission to update task with id '{request.TaskId}'");
        }
        
        task.UpdateTitle(request.Title);
        task.UpdateDescription(request.Description);
        task.UpdatePriority(request.Priority);
        await repository.UpdateAsync(task);

        logger.LogInformation(
            "Task with id '{TaskId}' for military with id '{MilitaryId}' was updated successfully", 
            task.Id,
            task.MilitaryId);
        
        return task.ToTaskDto();
    }
}