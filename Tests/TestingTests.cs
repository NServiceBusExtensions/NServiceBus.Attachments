using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Attachments.Testing;
using NServiceBus.Testing;
using Xunit;

public class TestingTests
{
    [Fact]
    public async Task OutgoingAttachment()
    {
        var context = new TestableMessageHandlerContext();
        var handler = new OutgoingAttachmentHandler();
        await handler.Handle(new AMessage(), context);
        var attachment = context.SentMessages
            .Single()
            .Options
            .OutgoingAttachment();
        Assert.True(attachment.HasPendingAttachment);
    }

    public class OutgoingAttachmentHandler : IHandleMessages<AMessage>
    {
        public Task Handle(AMessage message, IMessageHandlerContext context)
        {
            var options = new SendOptions();
            var attachment = options.OutgoingAttachment();
            attachment.Add(() => File.OpenRead(""));
            return context.Send(new AMessage(), options);
        }
    }
    [Fact]
    public async Task OutgoingAttachments()
    {
        var context = new TestableMessageHandlerContext();
        var handler = new OutgoingAttachmentsHandler();
        await handler.Handle(new AMessage(), context);
        var attachments = context.SentMessages
            .Single()
            .Options
            .OutgoingAttachments();
        var names = attachments.StreamNames;
        Assert.Single(names);
        Assert.Contains("theName", names);
        Assert.True(attachments.HasPendingAttachments);
    }

    public class OutgoingAttachmentsHandler : IHandleMessages<AMessage>
    {
        public Task Handle(AMessage message, IMessageHandlerContext context)
        {
            var options = new SendOptions();
            var attachments = options.OutgoingAttachments();
            attachments.Add("theName",() => File.OpenRead(""));
            return context.Send(new AMessage(), options);
        }
    }

    [Fact]
    public async Task IncomingAttachment()
    {
        var context = new TestableMessageHandlerContext();
        var handler = new IncomingAttachmentHandler();
        var mockMessageAttachments = new CustomMockMessageAttachments();
        context.InjectAttachmentsInstance(mockMessageAttachments);
        await handler.Handle(new AMessage(), context);
        Assert.True(mockMessageAttachments.GetBytesWasCalled);
    }

    public class CustomMockMessageAttachments : MockMessageAttachments
    {
        public override Task<byte[]> GetBytes()
        {
            GetBytesWasCalled = true;
            return Task.FromResult(new byte[] {5});
        }

        public bool GetBytesWasCalled { get; private set; }
    }

    public class IncomingAttachmentHandler : IHandleMessages<AMessage>
    {
        public async Task Handle(AMessage message, IMessageHandlerContext context)
        {
            var attachment = context.Attachments();
            var bytes = await attachment.GetBytes();
        }
    }

    public class AMessage
    {
    }
}

