using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tasks.Application.Dtos.Messages;
using Tasks.Application.Interfaces.Clients;
using Tasks.Common.Configuration;

namespace Tasks.Infrastructure.Clients;

public class NotificationsMicroserviceClient(
    IHttpClientFactory httpClientFactory,
    IOptions<NotificationsMicroserviceSettings> settings,
    ILogger<NotificationsMicroserviceClient> logger)
    : INotificationsMicroserviceClient
{
    public async Task SendTaskStartedNotificationAsync(TaskStartedMessageDto dto)
    {
        await SendTaskNotificationAsync(settings.Value.Endpoints.SendTaskStartedNotification, dto);
    }

    public async Task SendTaskStatusUpdatedNotificationAsync(TaskStatusUpdatedMessageDto dto)
    {
        await SendTaskNotificationAsync(settings.Value.Endpoints.SendTaskStatusUpdatedNotification, dto);
    }

    public async Task SendTaskConfirmedNotificationAsync(TaskConfirmedMessageDto dto)
    {
        await SendTaskNotificationAsync(settings.Value.Endpoints.SendTaskConfirmedNotification, dto);
    }
    
    private async Task SendTaskNotificationAsync<TDto>(string endpoint, TDto dto)
    {
        var httpClient = httpClientFactory.CreateClient("NotificationsMicroservice");
        
        var response = await httpClient.PostAsJsonAsync(endpoint, dto);
        
        if (!response.IsSuccessStatusCode)
        {
            logger.LogInformation(
                "Notification microservice responded with error status code '{ResponseStatusCode}'",
                response.StatusCode);
        }
    }
}