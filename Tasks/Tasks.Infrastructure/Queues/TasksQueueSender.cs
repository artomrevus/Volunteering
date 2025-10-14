using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Tasks.Application.Dtos.Messages;
using Tasks.Application.Interfaces.Queues;
using Tasks.Infrastructure.Configuration;
using Tasks.Infrastructure.Constants;

namespace Tasks.Infrastructure.Queues;

public class TasksQueueSender(IOptions<RabbitSettings> rabbitSettings) : ITasksQueueSender
{
    public async Task SendTaskStartedMessageAsync(TaskStartedMessage message)
    {
        await SendMessageAsync(message, TasksQueueConstants.RoutingKeys.TaskStarted);
    }

    public async Task SendTaskStatusUpdatedMessageAsync(TaskStatusUpdatedMessage message)
    {
        await SendMessageAsync(message, TasksQueueConstants.RoutingKeys.TaskStatusUpdated);
    }

    public async Task SendTaskConfirmedMessageAsync(TaskConfirmedMessage message)
    {
        await SendMessageAsync(message, TasksQueueConstants.RoutingKeys.TaskConfirmed);
    }
    
    private async Task SendMessageAsync(object payload, string routingKey)
    {
        var factory = new ConnectionFactory
        {
            HostName = rabbitSettings.Value.HostName,
            Port = rabbitSettings.Value.Port,
            UserName = rabbitSettings.Value.UserName,
            Password = rabbitSettings.Value.Password,
        };

        await using var connection = await factory.CreateConnectionAsync(); 
        await using var channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(
            exchange: TasksQueueConstants.Exchange,
            ExchangeType.Topic,
            durable: true);
        
        await channel.QueueDeclareAsync(
            queue: TasksQueueConstants.Queue, 
            durable: true, 
            exclusive: false, 
            autoDelete: false);
        
        await channel.QueueBindAsync(
            queue: TasksQueueConstants.Queue,
            exchange: TasksQueueConstants.Exchange, 
            routingKey: routingKey);
        
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload));

        await channel.BasicPublishAsync(
            exchange: TasksQueueConstants.Exchange,
            routingKey: routingKey,
            body: body
        );
    }
}