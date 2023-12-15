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
            BootstrapServers = "pkc-7xoy1.eu-central-1.aws.confluent.cloud:9092",
            SaslPassword = "YUHm8JhFGJQ12823r0yGHFjW+/3Yb6+FhyS1cFSjxg0ljMnWiQ8YoUUBAYlW5SHe",
            SaslUsername = "C7S7K6PA44AHMFCH",
            SaslMechanism = SaslMechanism.Plain,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            Acks = Acks.All // default, erst ack zurück wenn alles repliziert ist
            // Partitioner = Partitioner.Consistent,
        };
        
        var schemaRegistryConfig = new SchemaRegistryConfig
        {
            BasicAuthUserInfo = "VWE36QWXLK3QXTCT:WGBX7QQWsTLD5sVBOY1O/kd8LSzFfEN31WXPM60VAfY2mFfghe4OKlpAeovvXb9K",
            BasicAuthCredentialsSource = AuthCredentialsSource.UserInfo,
            Url = "https://psrc-2312y.europe-west3.gcp.confluent.cloud"
        };

        Action<DeliveryReport<string, OrderedPresent>> handler = r =>
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
        using var p = new ProducerBuilder<string, OrderedPresent>(conf)
            .SetValueSerializer(new AvroSerializer<OrderedPresent>(schemaRegistry).AsSyncOverAsync())
            .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
            .Build();
        while (true)
        {
            var simonClient = "Simon-Client";
            p.Produce("factory.presents.ordered.0", new Message<string, OrderedPresent> 
            { 
                Value = new OrderedPresent
                {
                    brand = "siemons",
                    price = 14.56,
                    product = Console.ReadLine()
                }, 
                Key = Guid.NewGuid().ToString(),
                Headers = new Headers
                {
                    { "producer-name", Encoding.UTF8.GetBytes(simonClient) }
                }
            }, handler);
            // p.ProduceAsync("factory.presents.ordered.0", new Message<string, OrderedPresentChecked> 
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