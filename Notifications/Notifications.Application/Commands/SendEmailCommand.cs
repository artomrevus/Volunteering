using MediatR;

namespace Notifications.Application.Commands;

public class SendEmailCommand : IRequest
{
    public string EmailTo { get; init; } = null!;
    
    public string Subject { get; init; } = null!;
    
    public string Body { get; init; } = null!;
}