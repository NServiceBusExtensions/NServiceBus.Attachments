using NServiceBus;
using NServiceBus.Attachments.FileShare;
using NServiceBus.Attachments.FileShare.Testing;
using NServiceBus.Testing;

// ReSharper disable UnusedVariable

class IncomingAttachment
{
    public void InjectAttachmentsInstance()
    {
        #region InjectAttachmentsInstance

        TestableMessageHandlerContext context = new();
        MyMessageAttachments mockMessageAttachments = new();
        context.InjectAttachmentsInstance(mockMessageAttachments);

        #endregion
    }
}

public class TestingIncoming
{
    #region CustomMockMessageAttachments

    public class CustomMockMessageAttachments :
        MockMessageAttachments
    {
        public override Task<AttachmentBytes> GetBytes(
            CancellationToken cancellation = default)
        {
            GetBytesWasCalled = true;
            return Task.FromResult(new AttachmentBytes("name", new byte[] {5}));
        }

        public bool GetBytesWasCalled { get; private set; }
    }

    #endregion

    #region TestIncomingHandler

    public class Handler :
        IHandleMessages<MyMessage>
    {
        public async Task Handle(MyMessage message, IMessageHandlerContext context)
        {
            var attachment = context.Attachments();
            var bytes = await attachment.GetBytes();
        }
    }

    #endregion

    #region TestIncoming

    [Fact]
    public async Task TestIncomingAttachment()
    {
        //Arrange
        TestableMessageHandlerContext context = new();
        Handler handler = new();
        CustomMockMessageAttachments mockMessageAttachments = new();
        context.InjectAttachmentsInstance(mockMessageAttachments);

        //Act
        await handler.Handle(new(), context);

        //Assert
        Assert.True(mockMessageAttachments.GetBytesWasCalled);
    }

    #endregion
}

class MyMessageAttachments :
    IMessageAttachments
{
    public Task<AttachmentStream> GetStream(CancellationToken cancellation = default) =>
        throw new NotImplementedException();

    public Task<AttachmentStream> GetStream(string name, CancellationToken cancellation = default) =>
        throw new NotImplementedException();

    public Task<AttachmentStream> GetStreamForMessage(string messageId, CancellationToken cancellation = default) =>
        throw new NotImplementedException();

    public Task<AttachmentStream> GetStreamForMessage(string messageId, string name, CancellationToken cancellation = default) =>
        throw new NotImplementedException();

    public Task CopyTo(string name, Stream target, CancellationToken cancellation = default) =>
        throw new NotImplementedException();

    public Task CopyTo(Stream target, CancellationToken cancellation = default) =>
        throw new NotImplementedException();

    public Task ProcessStream(string name, Func<AttachmentStream, Task> action, CancellationToken cancellation = default) =>
        throw new NotImplementedException();

    public Task ProcessStream(Func<AttachmentStream, Task> action, CancellationToken cancellation = default) =>
        throw new NotImplementedException();

    public Task ProcessStreams(Func<AttachmentStream, Task> action, CancellationToken cancellation = default) =>
        throw new NotImplementedException();

    public IAsyncEnumerable<AttachmentInfo> GetMetadata(CancellationToken cancellation = default) =>
        throw new NotImplementedException();

    public Task<AttachmentBytes> GetBytes(CancellationToken cancellation = default) =>
        throw new NotImplementedException();

    public Task<MemoryStream> GetMemoryStream(CancellationToken cancellation = default) =>
        throw new NotImplementedException();

    public Task<AttachmentBytes> GetBytes(string name, CancellationToken cancellation = default) =>
        throw new NotImplementedException();

    public Task<MemoryStream> GetMemoryStream(string name, CancellationToken cancellation = default) =>
        throw new NotImplementedException();

    public Task<AttachmentString> GetString(Encoding? encoding, CancellationToken cancellation = default) =>
        throw new NotImplementedException();

    public Task<AttachmentString> GetString(string name, Encoding? encoding, CancellationToken cancellation = default) =>
        throw new NotImplementedException();

    public Task CopyToForMessage(string messageId, string name, Stream target, CancellationToken cancellation = default) =>
        throw new NotImplementedException();

    public Task CopyToForMessage(string messageId, Stream target, CancellationToken cancellation = default) =>
        throw new NotImplementedException();

    public Task ProcessStreamForMessage(string messageId, string name, Func<AttachmentStream, Task> action, CancellationToken cancellation = default) =>
        throw new NotImplementedException();

    public Task ProcessStreamForMessage(string messageId, Func<AttachmentStream, Task> action, CancellationToken cancellation = default) =>
        throw new NotImplementedException();

    public Task ProcessStreamsForMessage(string messageId, Func<AttachmentStream, Task> action, CancellationToken cancellation = default) =>
        throw new NotImplementedException();

    public Task<AttachmentBytes> GetBytesForMessage(string messageId, CancellationToken cancellation = default) =>
        throw new NotImplementedException();

    public Task<MemoryStream> GetMemoryStreamForMessage(string messageId, CancellationToken cancellation = default) =>
        throw new NotImplementedException();

    public Task<AttachmentBytes> GetBytesForMessage(string messageId, string name, CancellationToken cancellation = default) =>
        throw new NotImplementedException();

    public Task<MemoryStream> GetMemoryStreamForMessage(string messageId, string name, CancellationToken cancellation = default) =>
        throw new NotImplementedException();

    public Task<AttachmentString> GetStringForMessage(string messageId, Encoding? encoding, CancellationToken cancellation = default) =>
        throw new NotImplementedException();

    public Task<AttachmentString> GetStringForMessage(string messageId, string name, Encoding? encoding, CancellationToken cancellation = default) =>
        throw new NotImplementedException();
}