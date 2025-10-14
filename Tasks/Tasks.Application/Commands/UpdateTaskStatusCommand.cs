using MediatR;
using Tasks.Application.Dtos;

namespace Tasks.Application.Commands;

public class UpdateTaskStatusCommand : IRequest<TaskDto>
{
    public string TaskId { get; init; } = null!;
    
    public string VolunteerId { get; init; } = null!;
    
    public string Status { get; init; } = null!;
}