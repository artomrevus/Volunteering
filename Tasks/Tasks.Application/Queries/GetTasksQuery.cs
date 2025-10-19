using MediatR;
using Tasks.Application.Dtos;
using Tasks.Application.Dtos.Tasks;

namespace Tasks.Application.Queries;

public class GetTasksQuery : IRequest<IEnumerable<TaskDto>>;