using MediatR;
using Microsoft.Extensions.Logging;
using Tasks.Application.Dtos;
using Tasks.Application.Dtos.Pagination;
using Tasks.Application.Dtos.Tasks;
using Tasks.Application.Interfaces;
using Tasks.Application.Interfaces.Repositories;
using Tasks.Application.Mappers;
using Tasks.Domain.Models;

namespace Tasks.Application.Queries.Handlers;

public class GetTasksWithFilterQueryHandler(
    ITasksRepository repository,
    ILogger<GetTasksWithFilterQueryHandler> logger)
    : IRequestHandler<GetTasksWithFilterQuery, PagedResult<TaskDto>>
{
    public async Task<PagedResult<TaskDto>> Handle(GetTasksWithFilterQuery request, CancellationToken cancellationToken)
    {
        var filter = new TaskFilter
        {
            Status = request.Status,
            Priority = request.Priority,
            MilitaryId = request.MilitaryId,
            VolunteerId = request.VolunteerId,
            CreatedAtFrom = request.CreatedAtFrom,
            CreatedAtTo = request.CreatedAtTo,
        };

        var sorting = new TaskSorting
        {
            SortBy = request.SortBy,
            IsDescending = request.IsDescending
        };

        var (tasks, totalCount) = await repository.GetFilteredTasksAsync(
            filter, 
            sorting, 
            request.PageNumber, 
            request.PageSize);

        logger.LogInformation("Filtered tasks were retrieved successfully");
        
        var taskDtos = tasks.ToTaskDtos();
        return new PagedResult<TaskDto>(taskDtos, totalCount, request.PageNumber, request.PageSize);
    }
}