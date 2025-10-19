using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notifications.Application.Exceptions;
using Notifications.Application.Interfaces.Notifications;
using Notifications.Common.Configuration;
using Notifications.Domain.Entities;

namespace Notifications.Application.Commands.Handlers;

public class SendTaskConfirmedEmailCommandHandler(
    IEmailSender emailSender,
    IOptions<FrontendSettings> frontendSettings,
    ILogger<SendTaskConfirmedEmailCommandHandler> logger)
    : IRequestHandler<SendTaskConfirmedEmailCommand>
{
    public async Task Handle(SendTaskConfirmedEmailCommand request, CancellationToken cancellationToken)
    {
        const string subject = "Task confirmed";
        var body = 
            $"Task \"{request.TaskTitle}\" was confirmed by military unit.\n\n" +
            $"You can view detailed information in your profile: " +
            $"{frontendSettings.Value.BaseUrl}/{frontendSettings.Value.Routes.Profile}";
        
        var emailMessage = new EmailMessageEntity(request.EmailTo, subject, body);

        if (!await emailSender.SendAsync(emailMessage))
        {
            logger.LogInformation(
                "Failed to send email to '{Email}'", 
                request.EmailTo);
            
            throw new InternalServerErrorException($"Failed to send email to '{request.EmailTo}'");
        }
        
        logger.LogInformation(
            "Mail to email address '{Email}' was sent successfully", 
            request.EmailTo);
    }
}