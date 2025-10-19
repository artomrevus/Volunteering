namespace Notifications.Common.Configuration;

public class FrontendSettings
{
    public string BaseUrl { get; set; } = null!;

    public RoutesSettings Routes { get; set; } = null!;

    public class RoutesSettings
    {
        public string Profile { get; set; } = null!;
    }

}