namespace Notifications.Domain.Exceptions;

public class EmailMessageException(string message) : Exception(message);