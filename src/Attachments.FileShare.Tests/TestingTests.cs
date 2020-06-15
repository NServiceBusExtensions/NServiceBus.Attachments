using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Attachments.FileShare;
using NServiceBus.Attachments.FileShare.Testing;
using NServiceBus.Testing;
using Xunit;

public class TestingTests
{
    [Fact]
    public async Task OutgoingAttachments()
    {
        var context = new TestableMessageHandlerContext();
        var handler = new OutgoingAttachmentsHandler();
        await handler.Handle(new AMessage(), context);
        var attachments = context.SentMessages
            .Single()
            .Options
            .Attachments();
        var names = attachments.Names;
        Assert.Single(names);
        Assert.Contains("theName", names);
        Assert.True(attachments.HasPendingAttachments);
    }

    public class OutgoingAttachmentsHandler :
        IHandleMessages<AMessage>
    {
        public Task Handle(AMessage message, IMessageHandlerContext context)
        {
            var options = new SendOptions();
            var attachments = options.Attachments();
            attachments.Add("theName", () => File.OpenRead(""));
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

    public class CustomMockMessageAttachments :
        MockMessageAttachments
    {
        public override Task<AttachmentBytes> GetBytes(CancellationToken cancellation = default)
        {
            GetBytesWasCalled = true;
            return Task.FromResult(new AttachmentBytes("default", new byte[] {5}));
        }

        public bool GetBytesWasCalled { get; private set; }
    }

    public class IncomingAttachmentHandler :
        IHandleMessages<AMessage>
    {
        public async Task Handle(AMessage message, IMessageHandlerContext context)
        {
            var attachment = context.Attachments();
            var bytes = await attachment.GetBytes();
            Trace.WriteLine(bytes);
        }
    }

    public class AMessage
    {
    }
}