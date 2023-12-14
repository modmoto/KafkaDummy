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
            EnableAutoCommit = false
        };

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
                }
                
                await Task.Delay(10, cts.Token);
            }

            foreach (var consumeResult in consumeResults)
            {
                Console.WriteLine($"message: {consumeResult.Partition.Value} / {consumeResult.Offset}: '{consumeResult.Message.Value}'");    
            }
            
            c.Commit();
        }
    }
}