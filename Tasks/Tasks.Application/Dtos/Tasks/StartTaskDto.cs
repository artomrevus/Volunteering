using System.ComponentModel.DataAnnotations;

namespace Tasks.Application.Dtos.Tasks;

public class StartTaskDto
{
    [Required(ErrorMessage = "TaskId is required")]
    public string TaskId { get; init; } = null!;
}
