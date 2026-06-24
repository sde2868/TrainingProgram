namespace TraineeManagement.Models;

public class RabbitMqTopology
{
    public const string ProcessingQueue =
        "submission-processing";

    public const string DeadLetterExchange =
        "submission-processing-dlx";

    public const string DeadLetterQueue =
        "submission-processing-dlq";

    public static Dictionary<string, object?> QueueArguments =>
        new()
        {
            ["x-dead-letter-exchange"] =
                DeadLetterExchange,

            ["x-dead-letter-routing-key"] =
                ProcessingQueue
        };
}