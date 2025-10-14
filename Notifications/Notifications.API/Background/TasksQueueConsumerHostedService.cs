using MediatR;
using Notifications.Application.Commands;
using Notifications.Application.Dtos.Bindings;
using Notifications.Application.Dtos.Messages;
using Notifications.Application.Exceptions;
using Notifications.Application.Interfaces.Queues;
using Notifications.Application.Queries;

namespace Notifications.API.Background;

public class TasksQueueConsumerHostedService(IServiceProvider serviceProvider) : BackgroundService
{
    private ITasksQueueConsumer? _queueConsumer;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = serviceProvider.CreateScope();
        _queueConsumer = scope.ServiceProvider.GetRequiredService<ITasksQueueConsumer>();
        
        _queueConsumer.HandleTaskStartedAsync = HandleTaskStartedAsync;
        _queueConsumer.HandleTaskStatusUpdatedAsync = HandleTaskStatusUpdatedAsync;
        _queueConsumer.HandleTaskConfirmedAsync = HandleTaskConfirmedAsync;

        await _queueConsumer.StartConsumingAsync(stoppingToken);
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_queueConsumer is not null)
        {
            await _queueConsumer.StopConsumingAsync();
        }
        
        await base.StopAsync(cancellationToken);
    }
    
    private async Task HandleTaskStartedAsync(TaskStartedMessage message)
    {
        const string subject = "Task started";
        var body = 
            $"Your task \"{message.TaskTitle}\" was started by volunteer.\n\n" +
            $"You can view detailed information in your profile: " +
            $"https://volunteering.frontend/profile/{message.MilitaryToNotifyId}";

        await SendEmailAsync(message.MilitaryToNotifyId, subject, body);
    }

    private async Task HandleTaskStatusUpdatedAsync(TaskStatusUpdatedMessage message)
    {
        const string subject = "Task status updated";
        var body = 
            $"Your task \"{message.TaskTitle}\" status was updated by volunteer.\n\n" +
            $"{message.OldTaskStatus} -> {message.NewTaskStatus}\n\n" +
            $"You can view detailed information in your profile: " +
            $"https://volunteering.frontend/profile/{message.MilitaryToNotifyId}";

        await SendEmailAsync(message.MilitaryToNotifyId, subject, body);
    }
    
    private async Task HandleTaskConfirmedAsync(TaskConfirmedMessage message)
    {
        const string subject = "Task confirmed";
        var body = 
            $"Task \"{message.TaskTitle}\" was confirmed by military unit.\n\n" +
            $"You can view detailed information in your profile: " +
            $"https://volunteering.frontend/profile/{message.VolunteerToNotifyId}";

        await SendEmailAsync(message.VolunteerToNotifyId, subject, body);
    }
    
    private async Task SendEmailAsync(string identityId, string subject, string body)
    {
        using var scope = serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        
        BindingDto binding;
        try
        {
            var getBindingQuery = new GetBindingByIdentityIdQuery { IdentityId = identityId };
            binding = await mediator.Send(getBindingQuery);
        }
        catch (NotFoundException)
        {
            return;
        }
        
        var sendEmailCommand = new SendEmailCommand
        {
            EmailTo = binding.Email,
            Subject = subject,
            Body = body,
        };

        await mediator.Send(sendEmailCommand);
    }
}