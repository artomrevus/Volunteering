using MediatR;
using Tasks.Application.Dtos;
using Tasks.Application.Dtos.Tasks;

namespace Tasks.Application.Commands;

public class ConfirmTaskCommand : IRequest<TaskDto>
{
    public string TaskId { get; init; } = null!;
    
    public string MilitaryId { get; init; } = null!;
}