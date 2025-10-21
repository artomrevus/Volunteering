using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Tasks.Application.Dtos.Messages;
using Tasks.Application.Interfaces.Queues;
using Tasks.Common.Configuration;
using Tasks.Infrastructure.Constants;

namespace Tasks.Infrastructure.Queues;

public class TasksQueueSender(IOptions<RabbitSettings> rabbitSettings) : ITasksQueueSender, IAsyncDisposable, IDisposable
{
    private IConnection? _connection;
    private IChannel? _channel;
    
    public async Task SendTaskStartedMessageAsync(TaskStartedMessageDto messageDto)
    {
        await SendMessageAsync(messageDto, TasksQueueConstants.TaskStartedKey);
    }

    public async Task SendTaskStatusUpdatedMessageAsync(TaskStatusUpdatedMessageDto messageDto)
    {
        await SendMessageAsync(messageDto, TasksQueueConstants.TaskStatusKey);
    }

    public async Task SendTaskConfirmedMessageAsync(TaskConfirmedMessageDto messageDto)
    {
        await SendMessageAsync(messageDto, TasksQueueConstants.TaskConfirmedKey);
    }

    private async Task SendMessageAsync(object payload, string routingKey)
    {
        await SetupChannelAsync();
        
        if (_channel is null)
        {
            throw new InvalidOperationException("Channel not initialized");
        }
        
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload));

        await _channel.BasicPublishAsync(
            exchange: TasksQueueConstants.Exchange,
            routingKey: routingKey,
            body: body
        );
    }
    
    private async Task SetupChannelAsync()
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

        _connection = await factory.CreateConnectionAsync(); 
        _channel = await _connection.CreateChannelAsync();

        await _channel.ExchangeDeclareAsync(
            exchange: TasksQueueConstants.Exchange,
            ExchangeType.Topic,
            durable: true);
        
        await _channel.QueueDeclareAsync(
            queue: TasksQueueConstants.Queue, 
            durable: true, 
            exclusive: false, 
            autoDelete: false);
        
        await _channel.QueueBindAsync(
            queue: TasksQueueConstants.Queue,
            exchange: TasksQueueConstants.Exchange, 
            routingKey: TasksQueueConstants.TaskTopicKey);
    }
    
    public async ValueTask DisposeAsync()
    {
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