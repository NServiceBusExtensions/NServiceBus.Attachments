using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Attachments.FileShare;

public class AttachmentsRunner
{
    public static CountdownEvent countdownEvent;
    static int iterations = 100;

    public static async Task Run()
    {
        countdownEvent = new CountdownEvent(iterations);
        var configuration = new EndpointConfiguration("FileShareAttachmentPerfTests");
        configuration.ApplySharedPerfConfig();
        var fileShare = Path.GetFullPath("attachments");
        Directory.CreateDirectory(fileShare);
        Helpers.PurgeDirectory(fileShare);
        configuration.EnableAttachments(fileShare, TimeToKeep.Default);
        var endpoint = await Endpoint.Start(configuration).ConfigureAwait(false);
        var stopwatch = Stopwatch.StartNew();
        await SendStartMessages(endpoint).ConfigureAwait(false);
        Console.WriteLine(stopwatch.Elapsed);
        countdownEvent.Wait();
        Console.WriteLine(stopwatch.Elapsed);
        await endpoint.Stop().ConfigureAwait(false);
        countdownEvent.Dispose();
    }

    static Task SendStartMessages(IEndpointInstance endpoint)
    {
        var tasks = new List<Task>();
        for (var i = 0; i < iterations; i++)
        {
            var sendOptions = new SendOptions();
            sendOptions.RouteToThisEndpoint();
            var attachment = sendOptions.Attachments();
            attachment.AddBytes(Helpers.Buffer);
            var send = endpoint.Send(new SendMessage(), sendOptions);
            tasks.Add(send);
            Console.WriteLine("Sending " + i);
        }
        return Task.WhenAll(tasks);
    }
}