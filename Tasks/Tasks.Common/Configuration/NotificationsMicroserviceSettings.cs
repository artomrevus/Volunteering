namespace Tasks.Common.Configuration;

public class NotificationsMicroserviceSettings
{
    public string BaseUrl { get; set; } = null!;

    public EndpointsSettings Endpoints { get; set; } = null!;

    public class EndpointsSettings
    {
        public string SendTaskStartedNotification { get; set; } = null!;

        public string SendTaskStatusUpdatedNotification { get; set; } = null!;
        
        public string SendTaskConfirmedNotification { get; set; } = null!;
    }

}