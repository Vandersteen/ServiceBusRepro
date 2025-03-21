# Repro

* Create a Standard service bus namespace
* Update the appsettings.json with the connection string in both projects
* Run the ServiceBusSender project
  * This will create an `azure-repro` topic and a `subscription1` subscription
  * It will send 10 messages to the topic
* Run the ServiceBusRepro project
* Observe that 1 message is received by the subscription, and the processor is then stuck


You will see the following output:

```
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development
info: Microsoft.Hosting.Lifetime[0]
      Content root path: /Users/xxx/Projects/ServiceBusRepro/ServiceBusRepro
info: ServiceBusRepro.Worker[0]
      Received message: b44c9cb1a1eb4bf79bca301c08b5b62d
info: ServiceBusRepro.Worker[0]
      Failed to process message: b44c9cb1a1eb4bf79bca301c08b5b62d, retrying

```

We expect the other messages to be processed (10 in total), but only 1 is processed and the processor is stuck.
