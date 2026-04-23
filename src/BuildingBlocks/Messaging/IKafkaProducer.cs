namespace BuildingBlocks.Messaging;

public interface IKafkaProducer
{
    Task ProduceAsync<T>(string topic, T message);
}

public interface IKafkaConsumer<T>
{
    Task SubscribeAsync(string topic, Func<T, Task> action, CancellationToken ct);
}
