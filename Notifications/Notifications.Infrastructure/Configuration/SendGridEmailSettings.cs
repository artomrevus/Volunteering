namespace Notifications.Infrastructure.Configuration;

public class SendGridEmailSettings
{
    public string FromEmail { get; set; } = null!;
    
    public string FromName { get; set; } = null!;
    
    public string SendGridApiKey { get; set; } = null!;
}