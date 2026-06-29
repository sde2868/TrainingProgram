using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using TraineeManagement.Models;
using TraineeManagement.Interfaces;
using Microsoft.Extensions.Options;
using TraineeManagement.Services;
using TraineeManagement.Constants;

public class RabbitMqPublisher : IMessagePublisher
{
    private readonly RabbitMqSettings _settings;
    private readonly ILogger<RabbitMqPublisher> _logger;

    public RabbitMqPublisher(
        IOptions<RabbitMqSettings> options,
        ILogger<RabbitMqPublisher> logger)
    {
        _settings = options.Value;
        _logger = logger;
    }

    public async Task PublishSubmissionProcessingAsync(
        SubmissionProcessingRequested message,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"PublishSubmissionProcessingAsync: new request to process submission for task submission ${message.TaskSubmissionId}.");
        var factory = new ConnectionFactory
        {
            HostName = _settings.HostName,
            Port = _settings.Port,
            UserName = _settings.UserName,
            Password = _settings.Password,
            VirtualHost = _settings.VirtualHost
        };

        await using var connection =
            await factory.CreateConnectionAsync(cancellationToken);

        await using var channel =
            await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        // var queueArgs = new Dictionary<string, object?>
        // {
        //     ["x-dead-letter-exchange"] = "submission-processing-dlx",
        //     ["x-dead-letter-routing-key"] = "submission-processing"
        // };

        // await channel.QueueDeclareAsync(
        //     queue: QueueNames.SubmissionProcessing,
        //     durable: true,
        //     exclusive: false,
        //     autoDelete: false,
        //     arguments: queueArgs,
        //     cancellationToken: cancellationToken);
        await RabbitMqTopologyConfigurator.ConfigureAsync(channel, cancellationToken);

        var json = JsonSerializer.Serialize(message);

        var body = Encoding.UTF8.GetBytes(json);

        var properties = new BasicProperties
        {
            Persistent = true,
            MessageId = message.MessageId.ToString(),
            CorrelationId = message.CorrelationId,
            ContentType = "application/json"
        };

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: QueueNames.SubmissionProcessing,
            mandatory: true,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken);

        _logger.LogInformation(
            "Submission processing message published. MessageId: {MessageId}, CorrelationId: {CorrelationId}, SubmissionId: {SubmissionId}, FileId: {FileId}",
            message.MessageId,
            message.CorrelationId,
            message.TaskSubmissionId,
            message.SubmissionFileId);
    }
}