using System.ComponentModel.DataAnnotations;
using Tasks.Application.Validation;
using Tasks.Domain.Constants;

namespace Tasks.Application.Dtos.Tasks;

public class UpdateTaskStatusDto
{
    [Required(ErrorMessage = "TaskId is required")]
    public string TaskId { get; init; } = null!;
    
    [Required(ErrorMessage = "Status is required")]
    [AllowedValuesIgnoreCase(
        TaskStatuses.Created, 
        TaskStatuses.InProgress, 
        TaskStatuses.Blocked, 
        TaskStatuses.Delivering, 
        TaskStatuses.Finished, 
        TaskStatuses.Confirmed)
    ]
    public string Status { get; init; } = null!;
}
