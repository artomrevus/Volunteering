using Notifications.Domain.Entities;

namespace Notifications.Application.Interfaces.Notifications;

public interface IEmailSender
{
    Task<bool> SendAsync(EmailMessageEntity emailMessage);
}