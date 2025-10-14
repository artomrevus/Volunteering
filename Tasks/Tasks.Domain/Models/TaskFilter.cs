namespace Tasks.Domain.Models;

public class TaskFilter
{
    public string? Status { get; set; }
    
    public string? Priority { get; set; }
    
    public string? MilitaryId { get; set; }
    
    public string? VolunteerId { get; set; }
    
    public DateTime? CreatedAtFrom { get; set; }
    
    public DateTime? CreatedAtTo { get; set; }
}