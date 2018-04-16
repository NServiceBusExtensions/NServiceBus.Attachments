using System;
using System.IO;
using System.Threading.Tasks;
using ApprovalTests;
using NServiceBus;
using Xunit;

public class OutgoingWhenNotEnabledTests
{
    [Fact]
    public void Run()
    {
        var configuration = new EndpointConfiguration("FileShareOutgoingWhenNotEnabledTests");
        configuration.UsePersistence<LearningPersistence>();
        configuration.UseTransport<LearningTransport>();
        var endpoint = Endpoint.Start(configuration).Result;

        var exception = Assert.Throws<AggregateException>(() => SendStartMessageWithAttachment(endpoint).Wait());
        Approvals.Verify(exception.InnerException.Message);
        endpoint.Stop().Wait();
    }

    static async Task SendStartMessageWithAttachment(IEndpointInstance endpoint)
    {
        var sendOptions = new SendOptions();
        sendOptions.RouteToThisEndpoint();
        var attachment = sendOptions.Attachments();
        attachment.Add(GetStream);
        await endpoint.Send(new SendMessage(), sendOptions);
    }

    static Stream GetStream()
    {
        var stream = new MemoryStream();
        var streamWriter = new StreamWriter(stream);
        streamWriter.Write("content");
        streamWriter.Flush();
        stream.Position = 0;
        return stream;
    }

    class SendMessage : IMessage
    {
    }
}