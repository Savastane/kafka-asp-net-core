using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

public class WorkProducer : BackgroundService
{
    private readonly ILogger<WorkProducer> _logger;
    private readonly IProducer<string, string> _kafkaProducer;

    public WorkProducer(ILogger<WorkProducer> logger, IProducer<string, string> kafkaProducer)
    {
        _logger = logger;
        _kafkaProducer = kafkaProducer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Mensagem JSON com carimbo de data e hora
            var jsonMessage = new JsonModel
            {
                MenuModel = new[]
            {
                new MenuItem { Nome = "Medicina Geral", Icone = "home", Rota = "/inicio" },
                new MenuItem
                {
                    Nome = "Odonto",
                    Icone = "box",
                    Rota = "/produtos",
                    Subitens = new[]
                    {
                        new SubItem { Nome = "Pilates", Icone = "desktop", Rota = "/produtos/eletronicos" },
                        new SubItem { Nome = "Acuputura", Icone = "tshirt", Rota = "/produtos/roupas" }
                    }
                },
                new MenuItem { Nome = "Terapias", Icone = "envelope", Rota = "/contato" },
                new MenuItem { Nome = "Estética", Icone = "envelope", Rota = "/contato" },
                new MenuItem { Nome = "Veterinario", Icone = "envelope", Rota = "/contato" }
            },
                DataEnvio = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
            };

            var jsonString = JsonSerializer.Serialize(jsonMessage);

            // Envie a mensagem para o tópico Kafka
            await _kafkaProducer.ProduceAsync("seu-topico-kafka", new Message<string, string> { Value = jsonString });

            _logger.LogInformation($"Mensagem enviada para o tópico Kafka às {DateTime.UtcNow}!");

            // Aguarde 1 minuto antes de enviar a próxima mensagem
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }


}


public class SubItem
{
    [JsonPropertyName("nome")]
    public string Nome { get; set; }

    [JsonPropertyName("icone")]
    public string Icone { get; set; }

    [JsonPropertyName("rota")]
    public string Rota { get; set; }
}

public class MenuItem
{
    [JsonPropertyName("nome")]
    public string Nome { get; set; }

    [JsonPropertyName("icone")]
    public string Icone { get; set; }

    [JsonPropertyName("rota")]
    public string Rota { get; set; }

    [JsonPropertyName("subitens")]
    public SubItem[] Subitens { get; set; }
}

public class JsonModel
{
    [JsonPropertyName("MenuModel")]
    public MenuItem[] MenuModel { get; set; }

    [JsonPropertyName("DataEnvio")]
    public string DataEnvio { get; set; }
}
