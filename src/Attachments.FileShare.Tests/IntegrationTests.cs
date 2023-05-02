﻿using NServiceBus.Attachments.FileShare;

public class IntegrationTests :
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
        attachment.Add(
            new AttachmentToAdd
            {
                Name = "viaAttachmentToAdd",
                Stream = GetStream()
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
            var withAttachment = await incomingAttachments.GetBytes("withMetadata");
            Assert.Equal("value", withAttachment.Metadata["key"]);
            var replyOptions = new ReplyOptions();
            var outgoingAttachment = replyOptions.Attachments();
            outgoingAttachment.Add(() => incomingAttachments.GetStream());
            await context.Reply(new ReplyMessage(), replyOptions);
            var attachmentInfos = await incomingAttachments.GetMetadata().ToAsyncList();
            Assert.Equal(3, attachmentInfos.Count);
        }
    }

    class ReplyHandler :
        IHandleMessages<ReplyMessage>
    {
        public async Task Handle(ReplyMessage message, HandlerContext context)
        {
            await using var memoryStream = new MemoryStream();
            var incomingAttachment = context.Attachments();
            await incomingAttachment.CopyTo(memoryStream);
            memoryStream.Position = 0;
            var buffer = memoryStream.GetBuffer();
            Debug.WriteLine(buffer);
            var attachmentInfos = await incomingAttachment.GetMetadata().ToAsyncList();
            Assert.Equal(1, attachmentInfos.Count);
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