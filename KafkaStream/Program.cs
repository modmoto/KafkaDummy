using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using digital.thinkport;
using Streamiz.Kafka.Net;
using Streamiz.Kafka.Net.SerDes;

class Program
{
    static async Task Main(string[] args)
    { 
        var config = new StreamConfig
        {
            BootstrapServers = "pkc-7xoy1.eu-central-1.aws.confluent.cloud:9092",
            ApplicationId = "SimonsMegaStreamsdad",
            SaslPassword = "YUHm8JhFGJQ12823r0yGHFjW+/3Yb6+FhyS1cFSjxg0ljMnWiQ8YoUUBAYlW5SHe",
            SaslUsername = "C7S7K6PA44AHMFCH",
            SaslMechanism = SaslMechanism.Plain,
            SecurityProtocol = SecurityProtocol.SaslSsl,
        };
        
        var builder = new StreamBuilder();

        builder.Stream<string, OrderedPresent>("factory.presents.ordered.0", new StringSerDes(), new AvroSerDes<OrderedPresent>())
            .Peek((_, val) => Console.WriteLine(val.product))
            .Filter((_, present) => present.price < 50)
            .MapValues((_, present) => new OrderedPresentChecked
            {
                price = present.price,
                product = present.product,
                brand = present.brand,
                checkedAt = DateTime.UtcNow.Ticks,
                checkedBy = "simon"
            })
            .To("factory.presents.checked.0", new StringSerDes(), new AvroSerDes<OrderedPresentChecked>());

        using KafkaStream stream = new KafkaStream(builder.Build(), config);

        Console.CancelKeyPress += (o, e) => {
            stream.Dispose();
        };

        await stream.StartAsync();
        while (true)
        {
            await Task.Delay(50);
            // Console.WriteLine("troll");
        }
    }
}

internal class AvroSerDes<T> : ISerDes<T> where T : class
{
    private AvroDeserializer<T> _avroDeserializer;
    private AvroSerializer<T> _avroSerializer;
    private CachedSchemaRegistryClient _schemaRegistryClient;

    public AvroSerDes()
    {
        var schemaRegistryConfig = new SchemaRegistryConfig
        {
            BasicAuthUserInfo = "VWE36QWXLK3QXTCT:WGBX7QQWsTLD5sVBOY1O/kd8LSzFfEN31WXPM60VAfY2mFfghe4OKlpAeovvXb9K",
            BasicAuthCredentialsSource = AuthCredentialsSource.UserInfo,
            Url = "https://psrc-2312y.europe-west3.gcp.confluent.cloud"
        };

        _schemaRegistryClient = new CachedSchemaRegistryClient(schemaRegistryConfig);
    }
    public object DeserializeObject(byte[] data, SerializationContext context)
    {
        return _avroDeserializer.DeserializeAsync(data, false, context).Result;
    }

    public byte[] SerializeObject(object data, SerializationContext context)
    {
        return _avroSerializer.SerializeAsync(data as T, context).Result;
    }

    public void Initialize(SerDesContext context)
    {
        _avroDeserializer = new AvroDeserializer<T>(_schemaRegistryClient);
        _avroSerializer = new AvroSerializer<T>(_schemaRegistryClient);
    }

    public T Deserialize(byte[] data, SerializationContext context)
    {
        return _avroDeserializer.DeserializeAsync(data, false, context).Result;
    }

    public byte[] Serialize(T data, SerializationContext context)
    {
        return _avroSerializer.SerializeAsync(data, context).Result;
    }
}