using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using TraineeManagement.Models;
using TraineeManagement.Services;
using SubmissionProcessor.Services;
using SubmissionProcessor.Exceptions;

namespace SubmissionProcessor;

public class Worker : BackgroundService
{
    private readonly IConfiguration _config;
    private readonly ILogger<Worker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    private IConnection? _connection;
    private IChannel? _channel;

    public Worker(
        IConfiguration config,
        ILogger<Worker> logger,
        IServiceScopeFactory scopeFactory)
    {
        _config = config;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _config["RabbitMQ:HostName"],
            Port = int.Parse(_config["RabbitMQ:Port"] ?? "5672"),
            UserName = _config["RabbitMQ:Username"],
            Password = _config["RabbitMQ:Password"],
            VirtualHost = _config["RabbitMQ:VirtualHost"] ?? "/"
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _connection = await factory.CreateConnectionAsync(stoppingToken);
                break;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "RabbitMQ unavailable. Retrying in 5 seconds...");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
        if (_connection == null)
        {
            _logger.LogWarning("RabbitMQ connection was not created because the worker is stopping.");
            return;
        }

        _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

        // var queue = _config["RabbitMQ:QueueName"];
        // var deadLetterExchange = _config["RabbitMQ:DeadLetterExchange"];
        // var deadLetterQueue = _config["RabbitMQ:DeadLetterQueue"];
        var queue = RabbitMqTopology.ProcessingQueue;

        if (string.IsNullOrWhiteSpace(queue))
        {
            throw new InvalidOperationException("RabbitMQ: QueueName configuration is missing.");
        }

        // await _channel.ExchangeDeclareAsync(
        //     exchange: deadLetterExchange!,
        //     type: ExchangeType.Direct,
        //     durable: true,
        //     cancellationToken: stoppingToken);

        // await _channel.QueueDeclareAsync(
        //     queue: deadLetterQueue!,
        //     durable: true,
        //     exclusive: false,
        //     autoDelete: false,
        //     cancellationToken: stoppingToken);

        // await _channel.QueueBindAsync(
        //     queue: deadLetterQueue!,
        //     exchange: deadLetterExchange!,
        //     routingKey: queue!,
        //     cancellationToken: stoppingToken);

        // var queueArgs = new Dictionary<string, object?>
        // {
        //     ["x-dead-letter-exchange"] = deadLetterExchange,
        //     ["x-dead-letter-routing-key"] = queue
        // };

        // await _channel.QueueDeclareAsync(
        //     queue: queue,
        //     durable: true,
        //     exclusive: false,
        //     autoDelete: false,
        //     arguments: queueArgs,
        //     cancellationToken: stoppingToken);
        await RabbitMqTopologyConfigurator.ConfigureAsync(_channel, stoppingToken);

        await _channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: 1,
            global: false,
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (sender, ea) =>
        {
            SubmissionProcessingRequested? message = null;
            try
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);

                message = JsonSerializer.Deserialize<SubmissionProcessingRequested>(json);
                if (message == null)
                {
                    throw new InvalidOperationException("Unable to deserialize message.");
                }

                using var logScope = _logger.BeginScope("CorrelationId:{CorrelationId}, MessageId:{MessageId}, SubmissionId:{SubmissionId}, SubmissionFileId:{SubmissionFileId}",
                                        message.CorrelationId,
                                        message.MessageId,
                                        message.TaskSubmissionId,
                                        message.SubmissionFileId);

                _logger.LogInformation("Received message {MessageId} for SubmissionId {SubmissionId} and FileId {FileId}",
                    message.MessageId,
                    message.TaskSubmissionId,
                    message.SubmissionFileId);

                using var scope = _scopeFactory.CreateScope();

                var processor = scope.ServiceProvider.GetRequiredService<ISubmissionProcessingService>();
                await processor.ProcessAsync(message, stoppingToken);

                await _channel.BasicAckAsync(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false,
                    cancellationToken: stoppingToken);

                _logger.LogInformation("Successfully processed message {MessageId}",
                    message.MessageId);
            }
            catch (RetryableProcessingException ex)
            {
                _logger.LogWarning(ex, "Retryable failure occurred for message {MessageId}", message?.MessageId);

                await _channel.BasicNackAsync(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false,
                    requeue: true,
                    cancellationToken: stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Processing failed for message {MessageId}", message?.MessageId);
                if (_channel != null)
                {
                    await _channel.BasicNackAsync(
                        deliveryTag: ea.DeliveryTag,
                        multiple: false,
                        requeue: false,
                        cancellationToken: stoppingToken);
                }
            }
        };

        await _channel.BasicConsumeAsync(
            queue: queue,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        _logger.LogInformation("Worker started listening to queue: {Queue}",
            queue);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel != null)
        {
            await _channel.CloseAsync(cancellationToken);
        }

        if (_connection != null)
        {
            await _connection.CloseAsync(cancellationToken);
        }

        await base.StopAsync(cancellationToken);
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();

        base.Dispose();
    }
}