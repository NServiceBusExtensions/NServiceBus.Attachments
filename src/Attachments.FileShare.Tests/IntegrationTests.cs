using NServiceBus;
using NServiceBus.Attachments.FileShare;

public class IntegrationTests :
    IDisposable
{
    ManualResetEvent resetEvent = new(false);

    [Fact]
    public async Task Run()
    {
        var configuration = new EndpointConfiguration("FileShareIntegrationTests");
        configuration.UsePersistence<LearningPersistence>();
        configuration.UseTransport<LearningTransport>();
        configuration.RegisterComponents(components => components.RegisterSingleton(resetEvent));
        configuration.EnableAttachments(Path.GetFullPath("attachments/IntegrationTests"), TimeToKeep.Default);
        var endpoint = await Endpoint.Start(configuration);
        await SendStartMessage(endpoint);
        resetEvent.WaitOne();
        await endpoint.Stop();
    }

    public void Dispose()
    {
        resetEvent.Dispose();
    }

    static Task SendStartMessage(IEndpointInstance endpoint)
    {
        SendOptions sendOptions = new();
        sendOptions.RouteToThisEndpoint();
        var attachment = sendOptions.Attachments();
        attachment.Add(GetStream);
        attachment.Add("withMetadata", GetStream, metadata: new Dictionary<string, string> { { "key", "value" } });
        return endpoint.Send(new SendMessage(), sendOptions);
    }

    static Stream GetStream()
    {
        MemoryStream stream = new();
        StreamWriter streamWriter = new(stream);
        streamWriter.Write("content");
        streamWriter.Flush();
        stream.Position = 0;
        return stream;
    }

    class SendHandler :
        IHandleMessages<SendMessage>
    {
        public async Task Handle(SendMessage message, IMessageHandlerContext context)
        {
            var withAttachment = await context.Attachments().GetBytes("withMetadata");
            Assert.Equal("value", withAttachment.Metadata["key"]);
            ReplyOptions replyOptions = new();
            var outgoingAttachment = replyOptions.Attachments();
            outgoingAttachment.Add(() =>
            {
                var incomingAttachment = context.Attachments();
                return incomingAttachment.GetStream();
            });
            await context.Reply(new ReplyMessage(), replyOptions);
        }
    }

    class ReplyHandler :
        IHandleMessages<ReplyMessage>
    {
        ManualResetEvent resetEvent;

        public ReplyHandler(ManualResetEvent resetEvent)
        {
            this.resetEvent = resetEvent;
        }

        public async Task Handle(ReplyMessage message, IMessageHandlerContext context)
        {
            await using MemoryStream memoryStream = new();
            var incomingAttachment = context.Attachments();
            await incomingAttachment.CopyTo(memoryStream);
            memoryStream.Position = 0;
            var buffer = memoryStream.GetBuffer();
            Debug.WriteLine(buffer);
            resetEvent.Set();
        }
    }

    class SendMessage :
        IMessage
    {
    }

    class ReplyMessage :
        IMessage
    {
    }
}