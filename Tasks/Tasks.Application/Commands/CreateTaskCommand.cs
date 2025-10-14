using MediatR;
using Tasks.Application.Dtos;

namespace Tasks.Application.Commands;

public record CreateTaskCommand : IRequest<TaskDto>
{
    public string MilitaryId { get; init; } = null!;
    
    public string Title { get; init; } = null!;

    public string Description { get; init; } = null!;

    public string Priority { get; init; } = null!;
}