using MediatR;
using Tasks.Application.Dtos;

namespace Tasks.Application.Commands;

public class ConfirmTaskCommand : IRequest<TaskDto>
{
    public string TaskId { get; init; } = null!;
    
    public string MilitaryId { get; init; } = null!;
}