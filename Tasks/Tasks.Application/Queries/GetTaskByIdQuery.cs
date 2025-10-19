using MediatR;
using Tasks.Application.Dtos;
using Tasks.Application.Dtos.Tasks;

namespace Tasks.Application.Queries;

public record GetTaskByIdQuery : IRequest<TaskDto>
{
    public string TaskId { get; init; } = null!;
}