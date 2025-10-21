using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Notifications.Application.Dtos.Messages;
using Notifications.Application.Interfaces.Queues;
using Notifications.Common.Configuration;
using Notifications.Infrastructure.Constants;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Notifications.Infrastructure.Queues;

public class TasksQueueConsumer(IOptions<RabbitSettings> rabbitSettings) : ITasksQueueConsumer, IAsyncDisposable, IDisposable
{
    private IConnection? _connection;
    private IChannel? _channel;
    private string? _consumerTag;
    
    public Func<TaskStartedMessageDto, Task>? HandleTaskStartedAsync { get; set; }
    public Func<TaskStatusUpdatedMessageDto, Task>? HandleTaskStatusUpdatedAsync { get; set; }
    public Func<TaskConfirmedMessageDto, Task>? HandleTaskConfirmedAsync { get; set; }

    public async Task StartConsumingAsync(CancellationToken cancellationToken)
    {
        await SetupChannelAsync(cancellationToken);
        await SetupConsumerAsync(cancellationToken);
    }

    public async Task StopConsumingAsync()
    {
        if (_channel is not null && !string.IsNullOrEmpty(_consumerTag))
        {
            await _channel.BasicCancelAsync(_consumerTag);
        }
    }
    
    private async Task SetupChannelAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null)
        {
            return;
        }
        
        var factory = new ConnectionFactory
        {
            HostName = rabbitSettings.Value.HostName,
            Port = rabbitSettings.Value.Port,
            UserName = rabbitSettings.Value.UserName,
            Password = rabbitSettings.Value.Password,
        };

        _connection = await factory.CreateConnectionAsync(cancellationToken: cancellationToken); 
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
            routingKey: TasksQueueConstants.TaskTopicKey,
            cancellationToken: cancellationToken);
    }
    
    private async Task SetupConsumerAsync(CancellationToken cancellationToken)
    {
        if (_channel is null)
        {
            throw new InvalidOperationException("Channel not initialized");
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
    
    private async Task ProcessMessageAsync(string routingKey, string message)
    {
        switch (routingKey)
        {
            case TasksQueueConstants.TaskStartedKey:
                await ProcessTaskStartedMessageAsync(message);
                break;
            case TasksQueueConstants.TaskStatusKey:
                await ProcessTaskStatusUpdatedMessageAsync(message);
                break;
            case TasksQueueConstants.TaskConfirmedKey:
                await ProcessTaskConfirmedMessageAsync(message);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(routingKey), routingKey, "Unknown routing key");
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
    
    public async ValueTask DisposeAsync()
    {
        await StopConsumingAsync();

        if (_channel is not null)
        {
            await _channel.CloseAsync();
        }

        if (_connection is not null)
        {
            await _connection.CloseAsync();
        }
        
        _channel?.Dispose();
        _connection?.Dispose();
        
        GC.SuppressFinalize(this);
    }
    
    public void Dispose()
    {
        DisposeAsync().AsTask().GetAwaiter().GetResult();
    }
}