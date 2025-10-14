using Tasks.Domain.Constants;
using Tasks.Domain.Exceptions;

namespace Tasks.Domain.Entities;

public class TaskEntity
{
    public string Id { get; private set; }

    public string MilitaryId { get; private set; }
    
    public string? VolunteerId { get; private set; }
    
    public string Title { get; private set; }

    public string Description { get; private set; }

    public string Priority { get; private set; }

    public string Status { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? StartedAt { get; private set; }
    
    public DateTime? FinishedAt { get; private set; }
    
    public DateTime? ConfirmedAt { get; private set; }

    public TaskEntity(
        string militaryId,
        string title, 
        string description,
        string priority)
    {
        if (!TaskPriorities.Exists(priority))
        {
            throw new TaskException($"Task priority '{priority}' does not exist.");
        }
        
        Id = Guid.NewGuid().ToString();
        MilitaryId = militaryId;
        Title = title;
        Description = description;
        Priority = TaskPriorities.Normalize(priority);
        Status = TaskStatuses.Created;
        CreatedAt = DateTime.UtcNow;
    }
    
    public void Start(string volunteerId)
    {
        if (Status != TaskStatuses.Created)
        {
            throw new TaskException($"Cannot start task in '{Status}' status.");
        }
        
        VolunteerId = volunteerId;
        Status = TaskStatuses.InProgress;
        StartedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(string newStatus)
    {
        if (!TaskStatuses.Exists(newStatus))
        {
            throw new TaskException($"Status '{newStatus}' does not exist.");
        }
        
        if (!TaskStatuses.IsTransitionAllowed(Status, newStatus))
        {
            throw new TaskException($"Cannot transit from '{Status}' to {newStatus}.");
        }

        Status = TaskStatuses.Normalize(newStatus);

        if (TaskStatuses.Normalize(newStatus) == TaskStatuses.Finished)
        {
            FinishedAt = DateTime.UtcNow;
        }
    }

    public void Confirm()
    {
        if (Status != TaskStatuses.Finished)
        {
            throw new TaskException($"Cannot confirm task in '{Status}' status.");
        }
        
        Status = TaskStatuses.Confirmed;
        ConfirmedAt = DateTime.UtcNow;
    }
    
    public void UpdateTitle(string newTitle)
    {
        Title = newTitle;
    }
    
    public void UpdateDescription(string newDescription)
    {
        Description = newDescription;
    }
    
    public void UpdatePriority(string newPriority)
    {
        if (!TaskPriorities.Exists(newPriority))
        {
            throw new TaskException($"Task priority '{newPriority}' does not exist.");
        }
        
        Priority = TaskPriorities.Normalize(newPriority);
    }
}