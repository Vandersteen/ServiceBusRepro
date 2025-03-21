using Azure.Messaging.ServiceBus.Administration;
using ServiceBusRepro;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

var host = builder.Build();

var adminClient = new ServiceBusAdministrationClient(builder.Configuration.GetConnectionString("ServiceBus"));

var topic = builder.Configuration.GetValue<string>("Topic");
var subscription = builder.Configuration.GetValue<string>("Subscription");

if (!await adminClient.TopicExistsAsync(topic))
{
    Console.WriteLine($"Topic {topic} does not exist, creating it");
    await adminClient.CreateTopicAsync(
        new CreateTopicOptions(topic)
        {
            RequiresDuplicateDetection = true,
            DuplicateDetectionHistoryTimeWindow = TimeSpan.FromHours(1)
        }
    );
}

if (!await adminClient.SubscriptionExistsAsync(topic, subscription))
{
    Console.WriteLine($"Subscription {subscription} does not exist, creating it");
    await adminClient.CreateSubscriptionAsync(
        new CreateSubscriptionOptions(topic, subscription)
        {
        }
    );
}


host.Run();
