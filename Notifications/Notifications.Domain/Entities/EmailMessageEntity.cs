using Notifications.Domain.Exceptions;
using Notifications.Domain.Helpers;

namespace Notifications.Domain.Entities;

public class EmailMessageEntity
{
    public string EmailTo { get; private init; }
    
    public string Subject { get; private init; }
    
    public string Body { get; private init; }
    
    public EmailMessageEntity(
        string emailTo,
        string subject,
        string body)
    {
        if (!EmailHelper.IsValidFormat(emailTo))
        {
            throw new EmailMessageException($"Email format '{emailTo}' is not valid.");
        }
        
        EmailTo = EmailHelper.NormalizeEmail(emailTo);;
        Subject = subject;
        Body = body;
    }
}