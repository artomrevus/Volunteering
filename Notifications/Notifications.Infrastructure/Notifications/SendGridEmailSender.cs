using Microsoft.Extensions.Options;
using Notifications.Application.Interfaces.Notifications;
using Notifications.Common.Configuration;
using Notifications.Domain.Entities;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Notifications.Infrastructure.Notifications;

public class SendGridEmailSender(
    ISendGridClient sendGridClient,
    IOptions<SendGridEmailSettings> sendGridSettings)
    : IEmailSender
{
    public async Task<bool> SendAsync(EmailMessageEntity emailMessage)
    {
        var from = new EmailAddress(sendGridSettings.Value.FromEmail, sendGridSettings.Value.FromName);
        var recipient = new EmailAddress(emailMessage.EmailTo);
        
        var msg = MailHelper.CreateSingleEmail(
            from, 
            recipient, 
            emailMessage.Subject, 
            emailMessage.Body,
            null);

        try
        {
            var response = await sendGridClient.SendEmailAsync(msg);
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }
}