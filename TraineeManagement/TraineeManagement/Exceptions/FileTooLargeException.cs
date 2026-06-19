namespace TraineeManagement.Exceptions;

public class FileTooLargeException : Exception
{
    public FileTooLargeException(string message)
        : base(message)
    {
    }
}