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
        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        
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
    
    private async Task HandleTaskStartedAsync(TaskStartedMessageDto messageDto)
    {
        using var scope = serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        
        BindingDto binding;
        try
        {
            var getBindingQuery = new GetBindingByIdentityIdQuery { IdentityId = messageDto.MilitaryToNotifyId };
            binding = await mediator.Send(getBindingQuery);
        }
        catch (NotFoundException)
        {
            return;
        }
        
        var sendEmailCommand = new SendTaskStartedEmailCommand
        {
            EmailTo = binding.Email,
            TaskTitle = messageDto.TaskTitle,
        };

        await mediator.Send(sendEmailCommand);
    }

    private async Task HandleTaskStatusUpdatedAsync(TaskStatusUpdatedMessageDto messageDto)
    {
        using var scope = serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        
        BindingDto binding;
        try
        {
            var getBindingQuery = new GetBindingByIdentityIdQuery { IdentityId = messageDto.MilitaryToNotifyId };
            binding = await mediator.Send(getBindingQuery);
        }
        catch (NotFoundException)
        {
            return;
        }
        
        var sendEmailCommand = new SendTaskStatusUpdatedEmailCommand
        {
            EmailTo = binding.Email,
            TaskTitle = messageDto.TaskTitle,
            OldTaskStatus = messageDto.OldTaskStatus,
            NewTaskStatus = messageDto.NewTaskStatus
        };

        await mediator.Send(sendEmailCommand);
    }
    
    private async Task HandleTaskConfirmedAsync(TaskConfirmedMessageDto messageDto)
    {
        using var scope = serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        
        BindingDto binding;
        try
        {
            var getBindingQuery = new GetBindingByIdentityIdQuery { IdentityId = messageDto.VolunteerToNotifyId };
            binding = await mediator.Send(getBindingQuery);
        }
        catch (NotFoundException)
        {
            return;
        }
        
        var sendEmailCommand = new SendTaskConfirmedEmailCommand
        {
            EmailTo = binding.Email,
            TaskTitle = messageDto.TaskTitle,
        };

        await mediator.Send(sendEmailCommand);
    }
}