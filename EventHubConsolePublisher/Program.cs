using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;

internal class Program

{

    private const string connectionString = "Endpoint=sb://lgeventhub.servicebus.windows.net/;SharedAccessKeyName=Consumer;SharedAccessKey=h00BY83PM67RfZ0m6DkuQ3eZZU54k++UoimaMNySDCM=;EntityPath=vacancyevents";
    private const string eventHubName = "vacancyevents";
   


    private static async Task Main()
    {
        Console.WriteLine("Hello, World!");
        // Create a producer client that you can use to send events to an event hub
        await using (var producerC1ient = new EventHubProducerClient(connectionString, eventHubName))
        {
            // Create a batch of events
            using EventDataBatch eventBatch = await producerC1ient.CreateBatchAsync();
            // Add events to the batch. An event is a represented by a collection of bytes and metadata.
            eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes("First event")));
            eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes("Second event")));
            eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes("Third event")));

            // Use the producer client to send the batch of events to the event hub
            await producerC1ient.SendAsync(eventBatch);
            Console.WriteLine("A batch of 3 events has been published. ");
        }
    }

}


