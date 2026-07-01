namespace TraineeManagement.Exceptions;
public class MessageQueueUnavailableException : Exception
{
    public MessageQueueUnavailableException(string message)
        : base(message)
    {
    }
}