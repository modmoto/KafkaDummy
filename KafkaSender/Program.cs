using System.Text;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using digital.thinkport;

namespace KafkaSender;

class Program
{
    public static void Main(string[] args)
    {
        var conf = new ProducerConfig
        {
            BootstrapServers = "pkc-75m1o.europe-west3.gcp.confluent.cloud:9092",
            SaslPassword = "E5AL3BwH3tvuz7nnZyc4T/ENN2TC0UUNTOces8gPefP2jtL+G5HRE8hjgI1bpFgJ",
            SaslUsername = "MTHMXNOJJOMJDWKC",
            SaslMechanism = SaslMechanism.Plain,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            Acks = Acks.All // default, erst ack zurück wenn alles repliziert ist
            // Partitioner = Partitioner.Consistent,
        };
        
        var schemaRegistryConfig = new SchemaRegistryConfig
        {
            BasicAuthUserInfo = "3SYHEWSXNVO7EG3P:9xJ+x9hW2AHEQIaqzBifBMBcgWP1kINcXNRy7+fGIe6xlCOlI1UzjVjXvtHMUenQ",
            BasicAuthCredentialsSource = AuthCredentialsSource.UserInfo,
            Url = "https://psrc-2312y.europe-west3.gcp.confluent.cloud"
        };

        Action<DeliveryReport<string, OrderedPresentChecked>> handler = r =>
        {
            if (r.Error.IsError)
            {
                Console.WriteLine($"Delivery Error: {r.Error.Reason}");
            }
            else
            {
                Console.WriteLine($"Written to offset: {r.Offset}");
            }
        };

        using var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);
        using var p = new ProducerBuilder<string, OrderedPresentChecked>(conf)
            .SetValueSerializer(new AvroSerializer<OrderedPresentChecked>(schemaRegistry).AsSyncOverAsync())
            .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
            .Build();
        while (true)
        {
            var simonClient = "Simon-Client";
            p.Produce("factory.presents.checked.0", new Message<string, OrderedPresentChecked> 
            { 
                Value = new OrderedPresentChecked
                {
                    brand = "siemons",
                    checkedAt = 123512,
                    checkedBy = "simon he",
                    price = 14.56,
                    product = Console.ReadLine()
                }, 
                Key = Guid.NewGuid().ToString(),
                Headers = new Headers
                {
                    { "producer-name", Encoding.UTF8.GetBytes(simonClient) }
                }
            }, handler);
            // p.ProduceAsync("factory.presents.checked.0", new Message<string, OrderedPresentChecked> 
            // { 
            //     Value = new OrderedPresentChecked
            //     {
            //         brand = "siemons",
            //         checkedAt = 123512,
            //         checkedBy = "simon he",
            //         price = 14.56,
            //         product = Console.ReadLine()
            //     }, 
            //     Key = Guid.NewGuid().ToString(),
            //     Headers = new Headers
            //     {
            //         { "producer-name", Encoding.UTF8.GetBytes(simonClient) }
            //     }
            // }).ContinueWith(task => handler.Invoke(task.));
            p.Flush();
        }
    }
}