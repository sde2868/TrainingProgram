namespace SubmissionProcessor.Exceptions;

public class RetryableProcessingException : Exception
{
    public RetryableProcessingException(string message)
        : base(message)
    {
    }
}