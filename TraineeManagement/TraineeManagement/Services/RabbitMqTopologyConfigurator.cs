using RabbitMQ.Client;
using TraineeManagement.Models;

namespace TraineeManagement.Services;

public static class RabbitMqTopologyConfigurator
{
    public static async Task ConfigureAsync(
        IChannel channel,
        CancellationToken cancellationToken = default)
    {
        await channel.ExchangeDeclareAsync(
            exchange: RabbitMqTopology.DeadLetterExchange,
            type: ExchangeType.Direct,
            durable: true,
            cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
            queue: RabbitMqTopology.DeadLetterQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await channel.QueueBindAsync(
            queue: RabbitMqTopology.DeadLetterQueue,
            exchange: RabbitMqTopology.DeadLetterExchange,
            routingKey: RabbitMqTopology.ProcessingQueue,
            cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
            queue: RabbitMqTopology.ProcessingQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: RabbitMqTopology.QueueArguments,
            cancellationToken: cancellationToken);
    }
}