namespace Auth.Domain.Constants;

public static class UserRoles
{
    public const string Military = "MILITARY";
    public const string Volunteer = "VOLUNTEER";
    
    public static IEnumerable<string> All => [Military, Volunteer];
    
    public static bool Exists(string role)
    {
        return All.Contains(Normalize(role));
    }
    
    public static string Normalize(string role)
    {
        return role.ToUpperInvariant();
    }
}