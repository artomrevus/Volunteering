using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Notifications.Application.Dtos.Messages;
using Notifications.Application.Interfaces.Queues;
using Notifications.Infrastructure.Configuration;
using Notifications.Infrastructure.Constants;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Notifications.Infrastructure.Queues;

public class TasksQueueConsumer(IOptions<RabbitSettings> rabbitSettings) : ITasksQueueConsumer, IDisposable
{
    private IConnection? _connection;
    private IChannel? _channel;
    private string? _consumerTag;
    
    public Func<TaskStartedMessageDto, Task>? HandleTaskStartedAsync { get; set; }
    public Func<TaskStatusUpdatedMessageDto, Task>? HandleTaskStatusUpdatedAsync { get; set; }
    public Func<TaskConfirmedMessageDto, Task>? HandleTaskConfirmedAsync { get; set; }

    public async Task StartConsumingAsync(CancellationToken cancellationToken)
    {
        await InitializeRabbitMqAsync(cancellationToken);
        await SetupConsumerAsync(cancellationToken);
    }

    private async Task InitializeRabbitMqAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = rabbitSettings.Value.HostName,
            Port = rabbitSettings.Value.Port,
            UserName = rabbitSettings.Value.UserName,
            Password = rabbitSettings.Value.Password,
        };
        
        _connection = await factory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
        
        await _channel.ExchangeDeclareAsync(
            exchange: TasksQueueConstants.Exchange, 
            ExchangeType.Topic, 
            durable: true,
            cancellationToken: cancellationToken);
        
        await _channel.QueueDeclareAsync(
            queue: TasksQueueConstants.Queue, 
            durable: true, 
            exclusive: false, 
            autoDelete: false,
            cancellationToken: cancellationToken);
        
        await _channel.QueueBindAsync(
            queue: TasksQueueConstants.Queue, 
            exchange: TasksQueueConstants.Exchange, 
            routingKey: TasksQueueConstants.RoutingKeys.TaskStarted,
            cancellationToken: cancellationToken);
        
        await _channel.QueueBindAsync(
            queue: TasksQueueConstants.Queue, 
            exchange: TasksQueueConstants.Exchange, 
            routingKey: TasksQueueConstants.RoutingKeys.TaskStatusUpdated,
            cancellationToken: cancellationToken);
        
        await _channel.QueueBindAsync(
            queue: TasksQueueConstants.Queue, 
            exchange: TasksQueueConstants.Exchange, 
            routingKey: TasksQueueConstants.RoutingKeys.TaskConfirmed,
            cancellationToken: cancellationToken);
    }

    private async Task SetupConsumerAsync(CancellationToken cancellationToken)
    {
        if (_channel is null)
        {
            return;
        }
        
        var consumer = new AsyncEventingBasicConsumer(_channel);
        
        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            try
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var routingKey = eventArgs.RoutingKey;

                await ProcessMessageAsync(routingKey, message);
                
                await _channel.BasicAckAsync(
                    deliveryTag: eventArgs.DeliveryTag,
                    multiple: false, 
                    cancellationToken: cancellationToken);
            }
            catch (Exception)
            {
                await _channel.BasicNackAsync(
                    deliveryTag: eventArgs.DeliveryTag,
                    multiple: false,
                    requeue: true, 
                    cancellationToken: cancellationToken);
            }
        };

        _consumerTag = await _channel.BasicConsumeAsync(
            queue: TasksQueueConstants.Queue,
            autoAck: false,
            consumer: consumer, 
            cancellationToken: cancellationToken);
    }

    public async Task StopConsumingAsync()
    {
        if (_channel is not null && !string.IsNullOrEmpty(_consumerTag))
        {
            await _channel.BasicCancelAsync(_consumerTag);
        }
    }
    
    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        
        GC.SuppressFinalize(this);
    }
    
    private async Task ProcessMessageAsync(string routingKey, string message)
    {
        switch (routingKey)
        {
            case TasksQueueConstants.RoutingKeys.TaskStarted:
                await ProcessTaskStartedMessageAsync(message);
                break;
            case TasksQueueConstants.RoutingKeys.TaskStatusUpdated:
                await ProcessTaskStatusUpdatedMessageAsync(message);
                break;
            case TasksQueueConstants.RoutingKeys.TaskConfirmed:
                await ProcessTaskConfirmedMessageAsync(message);
                break;
        }
    }
    
    private async Task ProcessTaskStartedMessageAsync(string message)
    {
        var taskStartedMessage = JsonSerializer.Deserialize<TaskStartedMessageDto>(message);
        
        if (taskStartedMessage is null)
        {
            throw new InvalidOperationException($"Cannot deserialize message of type '{nameof(TaskStartedMessageDto)}'");
        }
        
        if (HandleTaskStartedAsync is null)
        {
            throw new InvalidOperationException($"Cannot handle message of type '{nameof(TaskStartedMessageDto)}'");
        }
        
        await HandleTaskStartedAsync.Invoke(taskStartedMessage);
    }
    
    private async Task ProcessTaskStatusUpdatedMessageAsync(string message)
    {
        var taskStatusUpdatedMessage = JsonSerializer.Deserialize<TaskStatusUpdatedMessageDto>(message);
        
        if (taskStatusUpdatedMessage is null)
        {
            throw new InvalidOperationException($"Cannot deserialize message of type '{nameof(TaskStatusUpdatedMessageDto)}'");
        }
        
        if (HandleTaskStatusUpdatedAsync is null)
        {
            throw new InvalidOperationException($"Cannot handle message of type '{nameof(TaskStatusUpdatedMessageDto)}'");
        }
        
        await HandleTaskStatusUpdatedAsync.Invoke(taskStatusUpdatedMessage);
    }
    
    private async Task ProcessTaskConfirmedMessageAsync(string message)
    {
        var taskConfirmedMessage = JsonSerializer.Deserialize<TaskConfirmedMessageDto>(message);
        
        if (taskConfirmedMessage is null)
        {
            throw new InvalidOperationException($"Cannot deserialize message of type '{nameof(TaskConfirmedMessageDto)}'");
        }
        
        if (HandleTaskConfirmedAsync is null)
        {
            throw new InvalidOperationException($"Cannot handle message of type '{nameof(TaskConfirmedMessageDto)}'");
        }
        
        await HandleTaskConfirmedAsync.Invoke(taskConfirmedMessage);
    }
}