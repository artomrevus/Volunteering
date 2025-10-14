using Tasks.Application.Validation;

namespace Tasks.Application.Dtos.Tasks;

public class TaskSortingDto
{
    [AllowedValuesIgnoreCase("Priority", "CreatedAt")]
    public string SortBy { get; init; } = "CreatedAt";

    public bool IsDescending { get; set; } = true;
}