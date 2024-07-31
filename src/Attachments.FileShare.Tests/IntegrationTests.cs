﻿public class IntegrationTests :
    IDisposable
{
    static ManualResetEvent resetEvent = new(false);

    [Fact]
    public async Task Run()
    {
        var configuration = new EndpointConfiguration("FileShareIntegrationTests");
        configuration.UsePersistence<LearningPersistence>();
        configuration.UseTransport<LearningTransport>();
        configuration.RegisterComponents(_ => _.AddSingleton(resetEvent));
        configuration.EnableAttachments(Path.GetFullPath("attachments/IntegrationTests"), TimeToKeep.Default);
        configuration.UseSerialization<SystemJsonSerializer>();
        configuration.AssemblyScanner().ExcludeAssemblies("xunit.runner.utility.netcoreapp10.dll");
        var endpoint = await Endpoint.Start(configuration);
        await SendStartMessage(endpoint);
        resetEvent.WaitOne();
        await endpoint.Stop();
    }

    public void Dispose() =>
        resetEvent.Dispose();

    static Task SendStartMessage(IEndpointInstance endpoint)
    {
        var sendOptions = new SendOptions();
        sendOptions.RouteToThisEndpoint();
        var attachment = sendOptions.Attachments();
        attachment.Add(GetStream);
        attachment.Add(
            "withMetadata",
            GetStream,
            metadata: new Dictionary<string, string>
            {
                {
                    "key", "value"
                }
            });
        attachment.Add(async appendAttachment =>
        {
            await appendAttachment("viaAttachmentFactory1", GetStream());
            await appendAttachment("viaAttachmentFactory2", GetStream());
        });
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

    class SendHandler :
        IHandleMessages<SendMessage>
    {
        public async Task Handle(SendMessage message, HandlerContext context)
        {
            var incomingAttachments = context.Attachments();
            var withAttachment = await incomingAttachments.GetBytes("withMetadata", context.CancellationToken);
            Assert.Equal("value", withAttachment.Metadata["key"]);
            var replyOptions = new ReplyOptions();
            var outgoingAttachment = replyOptions.Attachments();
            outgoingAttachment.Add(() => incomingAttachments.GetStream());
            await context.Reply(new ReplyMessage(), replyOptions);
            var attachmentInfos = await incomingAttachments.GetMetadata(context.CancellationToken).ToAsyncList();
            Assert.Equal(4, attachmentInfos.Count);
        }
    }

    class ReplyHandler :
        IHandleMessages<ReplyMessage>
    {
        public async Task Handle(ReplyMessage message, HandlerContext context)
        {
            await using var memoryStream = new MemoryStream();
            var incomingAttachment = context.Attachments();
            await incomingAttachment.CopyTo(memoryStream, context.CancellationToken);
            memoryStream.Position = 0;
            var buffer = memoryStream.GetBuffer();
            Debug.WriteLine(buffer);
            var attachmentInfos = await incomingAttachment.GetMetadata(context.CancellationToken).ToAsyncList();
            Assert.Single(attachmentInfos);
            resetEvent.Set();
        }
    }

    class SendMessage :
        IMessage;

    class ReplyMessage :
        IMessage;
}