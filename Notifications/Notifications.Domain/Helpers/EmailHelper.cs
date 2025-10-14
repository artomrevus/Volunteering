using System.Text.RegularExpressions;

namespace Notifications.Domain.Helpers;

public static partial class EmailHelper
{
    public static bool IsValidFormat(string email)
    {
        return EmailRegex().IsMatch(email);
    }
    
    public static string NormalizeEmail(string email)
    {
        return email.ToLowerInvariant(); 
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();
}