using Confluent.Kafka;
using System.Text.Json;

namespace BuildingBlocks.Messaging;

public class KafkaProducer : IKafkaProducer
{
    private readonly ProducerConfig _config;

    public KafkaProducer(string bootstrapServers)
    {
        _config = new ProducerConfig { BootstrapServers = bootstrapServers };
    }

    public async Task ProduceAsync<T>(string topic, T message)
    {
        using var producer = new ProducerBuilder<string, string>(_config).Build();
        var val = JsonSerializer.Serialize(message);
        await producer.ProduceAsync(topic, new Message<string, string> { Key = Guid.NewGuid().ToString(), Value = val });
    }
}
