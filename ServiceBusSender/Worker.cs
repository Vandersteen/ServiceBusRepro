using Azure.Messaging.ServiceBus;

namespace ServiceBusSender;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly ServiceBusClient _serviceBusClient;

    public Worker(ILogger<Worker> logger, IConfiguration configuration, IHostApplicationLifetime applicationLifetime)
    {
        _logger = logger;
        _configuration = configuration;
        _applicationLifetime = applicationLifetime;
        _serviceBusClient = new ServiceBusClient(configuration.GetConnectionString("ServiceBus"));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var topic = _configuration.GetValue<string>("Topic");

        var sender = _serviceBusClient.CreateSender(topic);

        for (var i = 0; i < 10; i++)
        {
            var message = new ServiceBusMessage($"Message {i}");
            await sender.SendMessageAsync(message, stoppingToken);
            _logger.LogInformation("Sent message {Id}", message.MessageId);
        }

        _logger.LogInformation($"Stopping worker");
        _applicationLifetime.StopApplication();
    }

}
