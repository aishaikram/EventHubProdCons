using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Processor;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Storage.Blobs;

internal class Program
{


    private const string connectionString = "<Event hub connection string>";
    private const string eventHubName = "vacancyevents";

    //access keys
    private const string blobStorageConnectionString = "<blog connection string>";
    private const string blobContainerName = "eventhubconsumer";
    static async Task Main()
    {
        Console.WriteLine("Hello, From Consumer Group (aka application)");
        string consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;

        // Create a blob container client that the event processor will use for checkpointing
        BlobContainerClient storageClient = new BlobContainerClient(blobStorageConnectionString, blobContainerName);
        // Create an event processor client to process events in the event hub
        EventProcessorClient processor = new EventProcessorClient(storageClient, consumerGroup, connectionString, eventHubName);
        // Register handlers for processing events and handling errors
        processor.ProcessEventAsync += ProcessEventHandler;
        processor.ProcessErrorAsync += ProcessErrorHandler;

        // Start the processing
        await processor.StartProcessingAsync();
        // Wait for10 seconds for the events to be processed
        await Task.Delay(TimeSpan.FromSeconds(10));
        // Stop the processing
        await processor.StopProcessingAsync();

    }


    static Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
    {
        // Write details about the error to the console window
        Console.WriteLine($"\tPartition {eventArgs.PartitionId}' : an unhandled exception was encountered.");
        Console.WriteLine(eventArgs.Exception.Message);
        return Task.CompletedTask;

    }


    static async Task ProcessEventHandler(ProcessEventArgs eventArgs)
    {
        // Write the body of the event to the console window

        Console.WriteLine("\tReceived Event: {0}", Encoding.UTF8.GetString(eventArgs.Data.Body.ToArray()));
        // Update checkpoint in the blob storage so that the app receives only new events the next time it's run
        await eventArgs.UpdateCheckpointAsync(eventArgs.CancellationToken);
    }
}