using MediatR;
using Microsoft.Extensions.Logging;
using Tasks.Application.Commands.Handlers;
using Tasks.Application.Dtos;
using Tasks.Application.Exceptions;
using Tasks.Application.Interfaces;
using Tasks.Application.Interfaces.Repositories;
using Tasks.Application.Mappers;

namespace Tasks.Application.Queries.Handlers;

public class GetTaskByIdQueryHandler(
    ITasksRepository repository,
    ILogger<GetTaskByIdQueryHandler> logger)
    : IRequestHandler<GetTaskByIdQuery, TaskDto>
{
    public async Task<TaskDto> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var task = await repository.GetByIdAsync(request.TaskId);
        if (task is null)
        {
            logger.LogInformation(
                "Task with id '{TaskId}' was not found", 
                request.TaskId);
            
            throw new NotFoundException($"Task with Id '{request.TaskId}' not found.");
        }

        logger.LogInformation(
            "Task with id '{TaskId}' for military with id '{MilitaryId}' was retrieved successfully", 
            task.Id,
            task.MilitaryId);
        
        return task.ToTaskDto();
    }
}