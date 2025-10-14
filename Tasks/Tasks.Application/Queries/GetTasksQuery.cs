using MediatR;
using Tasks.Application.Dtos;

namespace Tasks.Application.Queries;

public class GetTasksQuery : IRequest<IEnumerable<TaskDto>>;