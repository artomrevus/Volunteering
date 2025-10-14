using Tasks.Domain.Exceptions;

namespace Tasks.Domain.Constants;

public static class TaskStatuses
{
    public const string Created = "CREATED";
    public const string InProgress = "IN_PROGRESS";
    public const string Blocked = "BLOCKED";
    public const string Delivering = "DELIVERING";
    public const string Finished = "FINISHED";
    public const string Confirmed = "CONFIRMED";
    
    public static IEnumerable<string> All => [Created, InProgress, Blocked, Delivering, Finished, Confirmed];
    
    public static bool IsTransitionAllowed(string oldStatus, string newStatus)
    {
        var allowedTransitions = GetAllowedTransitions(oldStatus);
        return allowedTransitions.Contains(Normalize(newStatus));
    }
    
    public static bool Exists(string status)
    {
        return All.Contains(Normalize(status));
    }
    
    private static IEnumerable<string> GetAllowedTransitions(string oldStatus)
    {
        return Normalize(oldStatus) switch
        {
            Created => [InProgress],
            InProgress => [Blocked, Delivering],
            Blocked => [InProgress],
            Delivering => [Finished],
            Finished => [Confirmed],
            _ => throw new TaskException($"Task status '{oldStatus}' does not exist.")
        };
    }
    
    public static string Normalize(string status)
    {
        return status.ToUpperInvariant();
    }
}