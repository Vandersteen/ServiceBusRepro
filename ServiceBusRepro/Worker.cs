using Azure.Messaging.ServiceBus;

namespace ServiceBusRepro;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;
    private readonly ServiceBusClient _serviceBusClient;

    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        _serviceBusClient = new ServiceBusClient(configuration.GetConnectionString("ServiceBus"));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        var topic = _configuration.GetValue<string>("Topic");
        var subscription = _configuration.GetValue<string>("Subscription");
        await using var processor = _serviceBusClient.CreateProcessor(topic, subscription, new ServiceBusProcessorOptions()
        {
            PrefetchCount = 1,
            AutoCompleteMessages = false
        });

        processor.ProcessMessageAsync += ProcessMessageAsync;
        processor.ProcessErrorAsync += args =>
        {
            _logger.LogError(args.Exception, "Error processing message");
            return Task.CompletedTask;
        };

        await processor.StartProcessingAsync(stoppingToken);

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            //ignore
        }
    }

    private async Task ProcessMessageAsync(ProcessMessageEventArgs arg)
    {
        _logger.LogInformation($"Received message: {arg.Message.MessageId}");

        var success = false;

        if (success)
        {
            _logger.LogInformation($"Successfully processed message: {arg.Message.MessageId}");
            await arg.CompleteMessageAsync(arg.Message);
            return;
        }

        _logger.LogInformation($"Failed to process message: {arg.Message.MessageId}, retrying");
        //We do not abandon the message, so it will be retried with a delay (lock duration)
        // await arg.AbandonMessageAsync();
    }
}
