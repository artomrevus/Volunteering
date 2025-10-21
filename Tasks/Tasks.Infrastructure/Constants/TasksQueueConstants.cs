namespace Tasks.Infrastructure.Constants;

public static class TasksQueueConstants
{
    public const string Exchange = "tasks-exchange";
    public const string Queue = "tasks-queue";
    
    public const string TaskTopicKey = "task.*";
        
    public const string TaskStartedKey = "task.started";
    public const string TaskStatusKey = "task.status";
    public const string TaskConfirmedKey = "task.confirmed";
}