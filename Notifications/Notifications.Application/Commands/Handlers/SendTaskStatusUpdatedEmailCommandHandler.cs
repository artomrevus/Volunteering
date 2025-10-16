using MediatR;
using Microsoft.Extensions.Logging;
using Notifications.Application.Exceptions;
using Notifications.Application.Interfaces.Notifications;
using Notifications.Domain.Entities;

namespace Notifications.Application.Commands.Handlers;

public class SendTaskStatusUpdatedEmailCommandHandler(
    IEmailSender emailSender,
    ILogger<SendTaskStatusUpdatedEmailCommandHandler> logger)
    : IRequestHandler<SendTaskStatusUpdatedEmailCommand>
{
    public async Task Handle(SendTaskStatusUpdatedEmailCommand request, CancellationToken cancellationToken)
    {
        const string subject = "Task status updated";
        var body = 
            $"Your task \"{request.TaskTitle}\" status was updated by volunteer.\n\n" +
            $"{request.OldTaskStatus} -> {request.NewTaskStatus}\n\n" +
            $"You can view detailed information in your profile: https://volunteering-frontend";
        
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