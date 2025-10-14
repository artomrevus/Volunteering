namespace Notifications.Application.Exceptions;

public class InternalServerErrorException(string message) : Exception(message);