namespace Tasks.Domain.Constants;

public static class TaskPriorities
{
    public const string Low = "LOW";
    public const string Average = "AVERAGE";
    public const string High = "HIGH";

    public static IEnumerable<string> All => [Low, Average, High];
    
    public static bool Exists(string priority)
    {
        return All.Contains(Normalize(priority));
    }
    
    public static string Normalize(string priority)
    {
        return priority.ToUpperInvariant();
    }
}