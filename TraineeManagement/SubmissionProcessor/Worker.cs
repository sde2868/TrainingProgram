using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using SubmissionProcessor.Models;
 
namespace SubmissionProcessor;
 
public class Worker : BackgroundService
{
    private readonly IConfiguration _config;
    private readonly ILogger<Worker> _logger;
 
    private IConnection _connection;
    private IModel _channel;
 
    public Worker(IConfiguration config, ILogger<Worker> logger)
    {
        _config = config;
        _logger = logger;
 
        var factory = new ConnectionFactory()
        {
            HostName = _config["RabbitMQ:HostName"],
            Port = int.Parse(_config["RabbitMQ:Port"] ?? "5672"),
            UserName = _config["RabbitMQ:Username"],
            Password = _config["RabbitMQ:Password"],
            VirtualHost = _config["RabbitMQ:VirtualHost"] ?? "/"
        };
 
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
 
        var queue = _config["RabbitMQ:QueueName"];
 
        _channel.QueueDeclare(
            queue: queue,
            durable: true,
            exclusive: false,
            autoDelete: false);
 
        _channel.BasicQos(0, 1, false);
    }
 
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var queue = _config["RabbitMq:QueueName"];
 
        var consumer = new EventingBasicConsumer(_channel);
 
        consumer.Received += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
 
                var message = JsonSerializer.Deserialize<SubmissionProcessingRequested>(json);
 
                _logger.LogInformation(
                    "Processing TaskSubmissionId: {SubmissionId}, FileId: {FileId}",
                    message?.TaskSubmissionId,
                    message?.SubmissionFileId
                );
 
                await Task.Delay(2000);
                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Processing failed");
 
                _channel.BasicNack(ea.DeliveryTag, false, true);
            }
        };
 
        _channel.BasicConsume(queue: queue, autoAck: false, consumer: consumer);
 
        _logger.LogInformation("✅ Worker started listening to queue: {Queue}", queue);
 
        return Task.CompletedTask;
    }
 
    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}