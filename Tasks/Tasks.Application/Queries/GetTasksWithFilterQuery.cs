using MediatR;
using Tasks.Application.Dtos;
using Tasks.Application.Dtos.Pagination;
using Tasks.Application.Dtos.Tasks;

namespace Tasks.Application.Queries;

public record GetTasksWithFilterQuery : IRequest<PagedResult<TaskDto>>
{
    public string? Status { get; init; }
    
    public string? Priority { get; init; }
    
    public string? MilitaryId { get; init; }
    
    public string? VolunteerId { get; init; }
    
    public DateTime? CreatedAtFrom { get; init; }
    
    public DateTime? CreatedAtTo { get; init; }
    
    public string? SortBy { get; init; }
    
    public bool IsDescending { get; init; }
    
    public int PageNumber { get; init; }
    
    public int PageSize { get; init; }
}