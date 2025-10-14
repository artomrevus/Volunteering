using System.ComponentModel.DataAnnotations;

namespace Tasks.Application.Dtos.Tasks;

public class ConfirmTaskDto
{
    [Required(ErrorMessage = "TaskId is required")]
    public string TaskId { get; init; } = null!;
}
