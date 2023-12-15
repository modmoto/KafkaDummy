using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using digital.thinkport;
using Streamiz.Kafka.Net;
using Streamiz.Kafka.Net.SerDes;
using Streamiz.Kafka.Net.Table;

class Program
{
    static async Task Main(string[] args)
    { 
        var config = new StreamConfig
        {
            BootstrapServers = "pkc-7xoy1.eu-central-1.aws.confluent.cloud:9092",
            ApplicationId = "elf-factorysdfsdfsdfsdf",
            SaslPassword = "YUHm8JhFGJQ12823r0yGHFjW+/3Yb6+FhyS1cFSjxg0ljMnWiQ8YoUUBAYlW5SHe",
            SaslUsername = "C7S7K6PA44AHMFCH",
            SaslMechanism = SaslMechanism.Plain,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            ReplicationFactor = 3
        };

        // var builder = new StreamBuilder();
        //
        // builder.Stream<string, OrderedPresent>("factory.presents.ordered.0", new StringSerDes(), new AvroSerDes<OrderedPresent>())
        //     .Peek((_, val) => Console.WriteLine($"{DateTime.UtcNow} {val.product} "))
        //     .Filter((_, present) => present.price < 50)
        //     .MapValues((_, present) => new OrderedPresentChecked
        //     {
        //         price = present.price,
        //         product = present.product,
        //         brand = present.brand,
        //         checkedAt = DateTime.UtcNow.Ticks,
        //         checkedBy = "simon"
        //     })
        //     .To("factory.presents.checked.0", new StringSerDes(), new AvroSerDes<OrderedPresentChecked>());
        //
        //
        // using KafkaStream stream = new KafkaStream(builder.Build(), config);
        // await stream.StartAsync();

        var builder = new StreamBuilder();

        builder.Stream<string, OrderedPresentChecked>("factory.presents.checked.0", new StringSerDes(), new AvroSerDes<OrderedPresentChecked>())
            .Peek((_, ding) => Console.WriteLine($"{ding.product} {ding.price}"))
            .GroupByKey()
            .Aggregate(() => new PresentsPerRecipient
                {
                    cumulated = 0,
                    presents = new List<string>(),
                    recipient = string.Empty
                },
                (_, newItem, aggregate) =>
                {
                    if (aggregate.cumulated < 50 && aggregate.cumulated + newItem.price < 50)
                    {
                        return new PresentsPerRecipient
                        {
                            cumulated = aggregate.cumulated + newItem.price,
                            recipient = aggregate.recipient,
                            presents = aggregate.presents.Append($"{newItem.brand}+{newItem.product}").ToList()
                        };
                    }

                    return aggregate;
                },
                RocksDb.As<string, PresentsPerRecipient>("my-present-store").WithValueSerdes<AvroSerDes<PresentsPerRecipient>>())
            .ToStream()
            .Peek((_, ding) => Console.WriteLine($"Cumulate Write {ding.recipient} {ding.cumulated}"))
            .To("factory.presents.cumulatedPerRecipient.0", new StringSerDes(), new AvroSerDes<PresentsPerRecipient>());


        using KafkaStream stream = new KafkaStream(builder.Build(), config);
        await stream.StartAsync();

        // await dollarStream.StartAsync();
        // await aggregateStream.StartAsync();
        while (true)
        {
            await Task.Delay(50);
            // Console.WriteLine("troll");
        }
    }

    private static async Task CreateAggregateStream(StreamConfig config)
    {
        var builder = new StreamBuilder();

        builder.Stream<string, OrderedPresentChecked>("factory.presents.checked.0", new StringSerDes(), new AvroSerDes<OrderedPresentChecked>())
            .GroupByKey()
            .Aggregate(() => new PresentsPerRecipient
            {
                cumulated = 0,
                presents = new List<string>(),
                recipient = string.Empty
            },
            (_, newItem, aggregate) =>
            {
                if (aggregate.cumulated < 50 && aggregate.cumulated + newItem.price < 50)
                {
                    return new PresentsPerRecipient
                    {
                        cumulated = aggregate.cumulated + newItem.price,
                        recipient = aggregate.recipient,
                        presents = aggregate.presents.Append($"{newItem.brand}+{newItem.product}").ToList()
                    };
                }

                return aggregate;
            },
            RocksDb.As<string, PresentsPerRecipient>("my-present-store").WithValueSerdes<AvroSerDes<PresentsPerRecipient>>());

        
        using KafkaStream stream = new KafkaStream(builder.Build(), config);

        Console.CancelKeyPress += (o, e) => {
            stream.Dispose();
        };

        await stream.StartAsync();
    }
    private static async Task CreateFilterFor50Dollar(StreamConfig config)
    {
        var builder = new StreamBuilder();

        builder.Stream<string, OrderedPresent>("factory.presents.ordered.0", new StringSerDes(), new AvroSerDes<OrderedPresent>())
            .Peek((_, val) => Console.WriteLine($"{DateTime.UtcNow} {val.product} "))
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
    }
}

internal class AvroSerDes<T> : ISerDes<T> where T : class
{
    private readonly AvroDeserializer<T> _avroDeserializer;
    private readonly AvroSerializer<T> _avroSerializer;

    public AvroSerDes()
    {
        var schemaRegistryConfig = new SchemaRegistryConfig
        {
            BasicAuthUserInfo = "VWE36QWXLK3QXTCT:WGBX7QQWsTLD5sVBOY1O/kd8LSzFfEN31WXPM60VAfY2mFfghe4OKlpAeovvXb9K",
            BasicAuthCredentialsSource = AuthCredentialsSource.UserInfo,
            Url = "https://psrc-2312y.europe-west3.gcp.confluent.cloud"
        };

        var schemaRegistryClient = new CachedSchemaRegistryClient(schemaRegistryConfig);
        _avroDeserializer = new AvroDeserializer<T>(schemaRegistryClient);
        _avroSerializer = new AvroSerializer<T>(schemaRegistryClient);
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