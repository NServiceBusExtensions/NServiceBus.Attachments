using System;
using System.IO;
using System.Threading.Tasks;
using ApprovalTests;
using NServiceBus;
using Xunit;
using Xunit.Abstractions;

public class OutgoingWhenNotEnabledTests :
    TestBase
{
    [Fact]
    public async Task Run()
    {
        var configuration = new EndpointConfiguration("FileShareOutgoingWhenNotEnabledTests");
        configuration.UsePersistence<LearningPersistence>();
        configuration.UseTransport<LearningTransport>();
        var endpoint = await Endpoint.Start(configuration);

        var exception = await Assert.ThrowsAsync<Exception>(() => SendStartMessageWithAttachment(endpoint));
        Approvals.Verify(exception.Message);
        await endpoint.Stop();
    }

    static Task SendStartMessageWithAttachment(IEndpointInstance endpoint)
    {
        var sendOptions = new SendOptions();
        sendOptions.RouteToThisEndpoint();
        var attachment = sendOptions.Attachments();
        attachment.Add(GetStream);
        return endpoint.Send(new SendMessage(), sendOptions);
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

    class SendMessage :
        IMessage
    {
    }

    public OutgoingWhenNotEnabledTests(ITestOutputHelper output) :
        base(output)
    {
    }
}