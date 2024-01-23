using Microsoft.Extensions.DependencyInjection;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((hostContext, services) =>
{
    // Configurações do Kafka
    var kafkaConfig = new ProducerConfig
    {
        BootstrapServers = "localhost:9092", // Substitua pelo endereço do seu servidor Kafka
        ClientId = "WorkProducer",
    };


  //  services.AddSingleton<IProducer<string, string>>(_ => new Producer<string, string>(kafkaConfig));

    services.AddSingleton<IProducer<string, string>>(provider => new ProducerBuilder<string, string>(kafkaConfig).Build());

    //using (IProducer<string, string> producer = new ProducerBuilder<string, string>(kafkaConfig).Build())
    //{
    //    services.AddSingleton<IProducer<string, string>>(provider => producer);
    //}


    services.AddHostedService<WorkProducer>();
});

await builder.RunConsoleAsync();
