using Confluent.Kafka;
using digital.thinkport;
using Streamiz.Kafka.Net;
using Streamiz.Kafka.Net.SerDes;
using Streamiz.Kafka.Net.Stream;
using Streamiz.Kafka.Net.Table;

class Program
{
    static async Task Main(string[] args)
    { 
        var config = new StreamConfig<StringSerDes, StringSerDes>()
        {
            BootstrapServers = "pkc-7xoy1.eu-central-1.aws.confluent.cloud:9092",
            SaslPassword = "YUHm8JhFGJQ12823r0yGHFjW+/3Yb6+FhyS1cFSjxg0ljMnWiQ8YoUUBAYlW5SHe",
            SaslUsername = "C7S7K6PA44AHMFCH",
            SaslMechanism = SaslMechanism.Plain,
            SecurityProtocol = SecurityProtocol.SaslSsl,
        };
    
        var builder = new StreamBuilder();

        var kstream = builder.Stream<string, OrderedPresent>("factory.presents.ordered.0")
            .Peek((_, val) => Console.WriteLine(val.product))
            .Filter((s, present) => present.price < 50)
            .MapValues((s, present) => new OrderedPresentChecked
            {
                price = present.price,
                product = present.product,
                brand = present.brand,
                checkedAt = DateTime.UtcNow.Ticks,
                checkedBy = "simon"
            });
        var ktable = builder.Table("table", InMemory.As<string, string>("table-store"));

        kstream.Join(ktable, (v, v1) => $"{v}-{v1}")
            .To("join-topic");

        Topology t = builder.Build();
        KafkaStream stream = new KafkaStream(t, config);

        Console.CancelKeyPress += (o, e) => {
            stream.Dispose();
        };

        await stream.StartAsync();
    }
}