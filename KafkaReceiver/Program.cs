using Confluent.Kafka;

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
            GroupId = "test-consumer-group",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
        };

        // 3SYHEWSXNVO7EG3P
        // 9xJ+x9hW2AHEQIaqzBifBMBcgWP1kINcXNRy7+fGIe6xlCOlI1UzjVjXvtHMUenQ
        // "schema.registry.url", "https://psrc-2312y.europe-west3.gcp.confluent.cloud"
        //     

        using var c = new ConsumerBuilder<Ignore, string>(conf).Build();
        
        c.Subscribe("SimonHEs-amazing-topic");

        CancellationTokenSource cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) => {
            e.Cancel = true;
            cts.Cancel();
        };

        var messageSize = 10;
        while (true)
        {
            var consumeResults = new List<ConsumeResult<Ignore, string>>();
            while (consumeResults.Count < messageSize)
            {
                var cr = c.Consume();
                if (cr.Message != null)
                {
                    consumeResults.Add(cr);
                    Console.WriteLine($"Captured messages ({consumeResults.Count, 2})");
                    Console.WriteLine($"message: {cr.Partition.Value} / {cr.Offset}: '{cr.Message.Value}'");
                    c.Commit(cr);
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