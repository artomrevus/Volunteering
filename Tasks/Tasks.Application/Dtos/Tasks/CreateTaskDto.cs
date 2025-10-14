using System.ComponentModel.DataAnnotations;
using Tasks.Application.Validation;
using Tasks.Domain.Constants;

namespace Tasks.Application.Dtos.Tasks;

public class CreateTaskDto
{
    [Required(ErrorMessage = "Title is required")]
    [MinLength(3, ErrorMessage = "Title must at least 3 characters")]
    [MaxLength(200, ErrorMessage = "Title must not exceed 200 characters")]
    public string Title { get; init; } = null!;

    [Required(ErrorMessage = "Description is required")]
    [MinLength(3, ErrorMessage = "Description must at least 3 characters")]
    [MaxLength(1000, ErrorMessage = "Description must not exceed 1000 characters")]
    public string Description { get; init; } = null!;

    [Required(ErrorMessage = "Priority is required")]
    [AllowedValuesIgnoreCase(
        TaskPriorities.Low, 
        TaskPriorities.Average,
        TaskPriorities.High)
    ]
    public string Priority { get; init; } = null!;
}