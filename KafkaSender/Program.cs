using Confluent.Kafka;

namespace KafkaSender;

class Program
{
    public static void Main(string[] args)
    {
        var conf = new ProducerConfig { BootstrapServers = "65.21.139.246:9092", AllowAutoCreateTopics = true };

        Action<DeliveryReport<Null, string>> handler = r =>
        {
            if (r.Error.IsError)
            {
                Console.WriteLine($"Delivery Error: {r.Error.Reason}");
            }
        };
        
        using (var p = new ProducerBuilder<Null, string>(conf).Build())
        {
            var partition = 0;
            while (true)
            {
                Console.WriteLine("Nachricht eingeben");
                var readLine = $"{partition}: {Console.ReadLine()}";
                // var topicPartition = new TopicPartition("my-topic", new Partition(partition % 2));
                // p.Produce(topicPartition, new Message<Null, string> { Value = readLine }, handler);
                // partition++;
                p.Produce("my-topic", new Message<Null, string> { Value = readLine }, handler);
                p.Flush();
            }
        }
    }
}