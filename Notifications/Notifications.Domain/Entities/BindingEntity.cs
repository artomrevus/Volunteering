using Notifications.Domain.Exceptions;
using Notifications.Domain.Helpers;

namespace Notifications.Domain.Entities;

public class BindingEntity
{
    public string IdentityId { get; private set; }
    
    public string Email { get; private set; }
    
    public BindingEntity(
        string identityId,
        string email)
    {
        if (!EmailHelper.IsValidFormat(email))
        {
            throw new BindingException($"Email format '{email}' is not valid.");
        }
        
        IdentityId = identityId;
        Email = EmailHelper.NormalizeEmail(email);
    }
    
    public void UpdateEmail(string newEmail)
    {
        if (!EmailHelper.IsValidFormat(newEmail))
        {
            throw new BindingException($"Email format '{newEmail}' is not valid.");
        }
        
        Email = EmailHelper.NormalizeEmail(newEmail);
    }
}