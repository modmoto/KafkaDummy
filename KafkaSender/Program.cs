using System.Text;
using Confluent.Kafka;

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
            // Partitioner = Partitioner.Consistent,
        };

        Action<DeliveryReport<string, string>> handler = r =>
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
        
        using (var p = new ProducerBuilder<string, string>(conf).Build())
        {
            while (true)
            {
                Console.WriteLine("Nachricht eingeben");
                string simonClient = "Simon-Client";
                p.Produce(new TopicPartition("SimonHEs-amazing-topic", new Partition(2)), new Message<string, string> 
                { 
                    Value = Console.ReadLine(), 
                    Key = Guid.NewGuid().ToString(),
                    Headers = new Headers
                    {
                        { "producer-name", Encoding.UTF8.GetBytes(simonClient) }
                    }
                }, handler);
                p.Flush();
            }
        }
    }
}