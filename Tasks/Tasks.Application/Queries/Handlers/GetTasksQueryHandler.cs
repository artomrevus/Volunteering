using MediatR;
using Microsoft.Extensions.Logging;
using Tasks.Application.Dtos;
using Tasks.Application.Dtos.Tasks;
using Tasks.Application.Interfaces;
using Tasks.Application.Interfaces.Repositories;
using Tasks.Application.Mappers;

namespace Tasks.Application.Queries.Handlers;

public class GetTasksQueryHandle(
    ITasksRepository repository,
    ILogger<GetTasksQueryHandle> logger) 
    : IRequestHandler<GetTasksQuery, IEnumerable<TaskDto>>
{
    public async Task<IEnumerable<TaskDto>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
    {
        var tasks = await repository.GetAllAsync();
        logger.LogInformation("All tasks were retrieved successfully");
        return tasks.ToTaskDtos();
    }
}