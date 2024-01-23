// See https://aka.ms/new-console-template for more information
using Confluent.Kafka;



var conf = new ConsumerConfig
{
    GroupId = "test-consumer-group",
    BootstrapServers = "127.0.0.1:9092",
    AutoOffsetReset = AutoOffsetReset.Earliest
};

using var c = new ConsumerBuilder<Ignore, string>(conf).Build();

{
    c.Subscribe("seu-topico-kafka");

    var cts = new CancellationTokenSource();
    Console.CancelKeyPress += (_, e) => {
        e.Cancel = true;
        cts.Cancel();
    };

    try
    {
        while (true)
        {
            try
            {
                var cr = c.Consume(cts.Token);
                Console.WriteLine($"Consumed message '{cr.Value}' at: '{cr.TopicPartitionOffset}'.");
            }
            catch (ConsumeException e)
            {
                Console.WriteLine($"Error occured: {e.Error.Reason}");
            }
        }
    }
    catch (OperationCanceledException)
    {
        c.Close();
    }
}