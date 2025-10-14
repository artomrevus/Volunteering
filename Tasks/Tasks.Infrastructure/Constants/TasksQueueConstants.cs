namespace Tasks.Infrastructure.Constants;

public static class TasksQueueConstants
{
    public const string Exchange = "tasks.exchange";
    public const string Queue = "tasks.queue";
    
    public static class RoutingKeys
    {
        public const string TaskStarted = "task.started";
        public const string TaskStatusUpdated = "task.status_updated";
        public const string TaskConfirmed = "task.confirmed";
    }
}