using MediatR;
using Tasks.Application.Dtos;

namespace Tasks.Application.Queries;

public record GetTaskByIdQuery : IRequest<TaskDto>
{
    public string TaskId { get; init; } = null!;
}