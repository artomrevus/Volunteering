using MediatR;
using Microsoft.Extensions.Logging;
using Notifications.Application.Exceptions;
using Notifications.Application.Interfaces.Notifications;
using Notifications.Domain.Entities;

namespace Notifications.Application.Commands.Handlers;

public class SendEmailCommandHandler(
    IEmailSender emailSender,
    ILogger<SendEmailCommandHandler> logger)
    : IRequestHandler<SendEmailCommand>
{
    public async Task Handle(SendEmailCommand request, CancellationToken cancellationToken)
    {
        var emailMessage = new EmailMessageEntity(
            request.EmailTo,
            request.Subject,
            request.Body);

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