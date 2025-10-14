using MediatR;
using Microsoft.Extensions.Logging;
using Tasks.Application.Dtos;
using Tasks.Application.Interfaces;
using Tasks.Application.Interfaces.Repositories;
using Tasks.Application.Mappers;
using Tasks.Domain.Entities;

namespace Tasks.Application.Commands.Handlers;

public class CreateTaskCommandHandler(
    ITasksRepository repository,
    ILogger<ConfirmTaskCommandHandler> logger) 
    : IRequestHandler<CreateTaskCommand, TaskDto>
{
    public async Task<TaskDto> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = new TaskEntity(
            request.MilitaryId, 
            request.Title, 
            request.Description, 
            request.Priority);
        
        var createdTask = await repository.AddAsync(task);
        
        logger.LogInformation(
            "Task with id '{TaskId}' for military with id '{MilitaryId}' was created successfully", 
            task.Id,
            task.MilitaryId);
        
        return createdTask.ToTaskDto();
    }
}