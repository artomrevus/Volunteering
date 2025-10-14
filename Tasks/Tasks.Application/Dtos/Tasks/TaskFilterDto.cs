using Tasks.Application.Validation;
using Tasks.Domain.Constants;

namespace Tasks.Application.Dtos.Tasks;

public class TaskFilterDto
{
    [AllowedValuesIgnoreCase(
        TaskStatuses.Created, 
        TaskStatuses.InProgress, 
        TaskStatuses.Blocked, 
        TaskStatuses.Delivering, 
        TaskStatuses.Finished, 
        TaskStatuses.Confirmed)
    ]
    public string? Status { get; set; }
    
    [AllowedValuesIgnoreCase(
        TaskPriorities.Low, 
        TaskPriorities.Average,
        TaskPriorities.High)
    ]
    public string? Priority { get; set; }
    
    public string? MilitaryId { get; set; }
    
    public string? VolunteerId { get; set; }
    
    public DateTime? CreatedAtFrom { get; set; }
    
    public DateTime? CreatedAtTo { get; set; }
    
}