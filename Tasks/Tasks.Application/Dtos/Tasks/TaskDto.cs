namespace Tasks.Application.Dtos;

public class TaskDto
{
    public string Id { get; init; } = null!;

    public string MilitaryId { get; init; } = null!;
    
    public string? VolunteerId { get; init; }
    
    public string Title { get; init; } = null!;

    public string Description { get; init; } = null!;

    public string Priority { get; init; } = null!;

    public string Status { get; init; } = null!;

    public DateTime CreatedAt { get; init; }

    public DateTime? StartedAt { get; init; }
    
    public DateTime? FinishedAt { get; init; }
    
    public DateTime? ConfirmedAt { get; init; }
}