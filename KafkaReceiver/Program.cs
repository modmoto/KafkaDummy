using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using digital.thinkport;

namespace KafkaReceiver;

class Program
{
    public static async Task Main(string[] args)
    {
        var conf = new ConsumerConfig
        { 
            BootstrapServers = "pkc-75m1o.europe-west3.gcp.confluent.cloud:9092",
            SaslPassword = "E5AL3BwH3tvuz7nnZyc4T/ENN2TC0UUNTOces8gPefP2jtL+G5HRE8hjgI1bpFgJ",
            SaslUsername = "MTHMXNOJJOMJDWKC",
            SaslMechanism = SaslMechanism.Plain,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            GroupId = "test-consumer-group-nochmal",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
        };

        var schemaRegistryConfig = new SchemaRegistryConfig
        {
            BasicAuthUserInfo = "3SYHEWSXNVO7EG3P:9xJ+x9hW2AHEQIaqzBifBMBcgWP1kINcXNRy7+fGIe6xlCOlI1UzjVjXvtHMUenQ",
            BasicAuthCredentialsSource = AuthCredentialsSource.UserInfo,
            Url = "https://psrc-2312y.europe-west3.gcp.confluent.cloud"
        };

        using var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);
        using var consumer =
            new ConsumerBuilder<Ignore, OrderedPresentChecked>(conf)
                .SetValueDeserializer(new AvroDeserializer<OrderedPresentChecked>(schemaRegistry).AsSyncOverAsync())
                .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
                .Build();
        
        consumer.Subscribe("factory.presents.checked.0");

        CancellationTokenSource cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) => {
            e.Cancel = true;
            cts.Cancel();
        };
        
        
        var messageSize = 10;
        while (true)
        {
            var consumeResults = new List<ConsumeResult<Ignore, OrderedPresentChecked>>();
            while (consumeResults.Count < messageSize)
            {
                var cr = consumer.Consume();
                if (cr.Message != null)
                {
                    consumeResults.Add(cr);
                    Console.WriteLine($"Captured messages ({consumeResults.Count, 2})");
                    Console.WriteLine($"message: {cr.Partition.Value} / {cr.Offset}: '{cr.Message.Value}'");
                    consumer.Commit(cr);
                }
                
                await Task.Delay(10, cts.Token);
            }

            // foreach (var consumeResult in consumeResults)
            // {
            //     Console.WriteLine($"message: {consumeResult.Partition.Value} / {consumeResult.Offset}: '{consumeResult.Message.Value}'");    
            // }
        }
    }
}