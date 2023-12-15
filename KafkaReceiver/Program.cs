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
            BootstrapServers = "pkc-7xoy1.eu-central-1.aws.confluent.cloud:9092",
            SaslPassword = "YUHm8JhFGJQ12823r0yGHFjW+/3Yb6+FhyS1cFSjxg0ljMnWiQ8YoUUBAYlW5SHe",
            SaslUsername = "C7S7K6PA44AHMFCH",
            SaslMechanism = SaslMechanism.Plain,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            GroupId = "test-consumer-group-nochmal",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
        };

        var schemaRegistryConfig = new SchemaRegistryConfig
        {
            BasicAuthUserInfo = "VWE36QWXLK3QXTCT:WGBX7QQWsTLD5sVBOY1O/kd8LSzFfEN31WXPM60VAfY2mFfghe4OKlpAeovvXb9K",
            BasicAuthCredentialsSource = AuthCredentialsSource.UserInfo,
            Url = "https://psrc-2312y.europe-west3.gcp.confluent.cloud"
        };

        using var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);
        using var consumer =
            new ConsumerBuilder<Ignore, OrderedPresent>(conf)
                .SetValueDeserializer(new AvroDeserializer<OrderedPresent>(schemaRegistry).AsSyncOverAsync())
                .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
                .Build();
        
        consumer.Subscribe("factory.presents.ordered.0");

        CancellationTokenSource cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) => {
            e.Cancel = true;
            cts.Cancel();
        };
        
        
        var messageSize = 10;
        while (true)
        {
            var consumeResults = new List<ConsumeResult<Ignore, OrderedPresent>>();
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