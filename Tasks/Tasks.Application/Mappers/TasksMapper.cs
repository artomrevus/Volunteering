using Tasks.Application.Dtos;
using Tasks.Domain.Entities;

namespace Tasks.Application.Mappers;

public static class TasksMapper
{
    public static TaskDto ToTaskDto(this TaskEntity task)
    {
        return new TaskDto
        {
            Id = task.Id,
            MilitaryId = task.MilitaryId,
            VolunteerId = task.VolunteerId,
            Title = task.Title,
            Description = task.Description,
            Priority = task.Priority,
            Status = task.Status,
            CreatedAt = task.CreatedAt,
            StartedAt = task.StartedAt,
            FinishedAt = task.FinishedAt,
            ConfirmedAt = task.ConfirmedAt,
        };
    }
    
    public static IEnumerable<TaskDto> ToTaskDtos(this IEnumerable<TaskEntity> tasks)
    {
        return tasks.Select(ToTaskDto);
    }
}