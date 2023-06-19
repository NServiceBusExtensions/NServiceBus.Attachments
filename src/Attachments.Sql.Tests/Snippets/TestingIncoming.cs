using NServiceBus.Attachments.Sql;
using NServiceBus.Attachments.Sql.Testing;
using NServiceBus.Testing;

// ReSharper disable UnusedVariable

class IncomingAttachment
{
    public void InjectAttachmentsInstance()
    {
        #region InjectAttachmentsInstance

        var context = new TestableMessageHandlerContext();
        var mockMessageAttachments = new MyMessageAttachments();
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
        public override Task<AttachmentBytes> GetBytes(Cancellation cancel = default)
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
        public async Task Handle(MyMessage message, HandlerContext context)
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
        var context = new TestableMessageHandlerContext();
        var handler = new Handler();
        var mockMessageAttachments = new CustomMockMessageAttachments();
        context.InjectAttachmentsInstance(mockMessageAttachments);

        //Act
        await handler.Handle(new(), context);

        //Assert
        Assert.True(mockMessageAttachments.GetBytesWasCalled);
    }

    #endregion
}

class MyMessageAttachments : IMessageAttachments
{
    public Task ProcessByteArraysForMessage(string messageId, Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancel = default) =>
        throw new NotImplementedException();

    public Task CopyTo(string name, Stream target, Cancellation cancel = default) =>
        throw new NotImplementedException();

    public Task CopyTo(Stream target, Cancellation cancel = default) =>
        throw new NotImplementedException();

    public Task ProcessStream(string name, Func<AttachmentStream, Cancellation, Task> action, Cancellation cancel = default) =>
        throw new NotImplementedException();

    public Task ProcessStream(Func<AttachmentStream, Cancellation, Task> action, Cancellation cancel = default) =>
        throw new NotImplementedException();

    public Task ProcessStreams(Func<AttachmentStream, Cancellation, Task> action, Cancellation cancel = default) =>
        throw new NotImplementedException();

    public IAsyncEnumerable<AttachmentInfo> GetMetadata(Cancellation cancel = default) =>
        throw new NotImplementedException();

    public Task<AttachmentBytes> GetBytes(Cancellation cancel = default) =>
        throw new NotImplementedException();

    public Task<AttachmentBytes> GetBytes(string name, Cancellation cancel = default) =>
        throw new NotImplementedException();

    public Task<MemoryStream> GetMemoryStream(Cancellation cancel = default) =>
        throw new NotImplementedException();

    public Task<MemoryStream> GetMemoryStream(string name, Cancellation cancel = default) =>
        throw new NotImplementedException();

    public Task<AttachmentString> GetString(Encoding? encoding, Cancellation cancel = default) =>
        throw new NotImplementedException();

    public Task<AttachmentString> GetString(string name, Encoding? encoding, Cancellation cancel = default) =>
        throw new NotImplementedException();

    public Task<AttachmentStream> GetStream(Cancellation cancel = default) =>
        throw new NotImplementedException();

    public Task<AttachmentStream> GetStream(string name, Cancellation cancel = default) =>
        throw new NotImplementedException();

    public Task CopyToForMessage(string messageId, string name, Stream target, Cancellation cancel = default) =>
        throw new NotImplementedException();

    public Task CopyToForMessage(string messageId, Stream target, Cancellation cancel = default) =>
        throw new NotImplementedException();

    public Task ProcessStreamForMessage(string messageId, string name, Func<AttachmentStream, Cancellation, Task> action, Cancellation cancel = default) =>
        throw new NotImplementedException();

    public Task ProcessStreamForMessage(string messageId, Func<AttachmentStream, Cancellation, Task> action, Cancellation cancel = default) =>
        throw new NotImplementedException();

    public Task ProcessStreamsForMessage(string messageId, Func<AttachmentStream, Cancellation, Task> action, Cancellation cancel = default) =>
        throw new NotImplementedException();

    public Task<AttachmentBytes> GetBytesForMessage(string messageId, Cancellation cancel = default) =>
        throw new NotImplementedException();

    public Task<MemoryStream> GetMemoryStreamForMessage(string messageId, Cancellation cancel = default) =>
        throw new NotImplementedException();

    public Task<AttachmentBytes> GetBytesForMessage(string messageId, string name, Cancellation cancel = default) =>
        throw new NotImplementedException();

    public Task<MemoryStream> GetMemoryStreamForMessage(string messageId, string name, Cancellation cancel = default) =>
        throw new NotImplementedException();

    public Task<AttachmentString> GetStringForMessage(string messageId, Encoding? encoding, Cancellation cancel = default) =>
        throw new NotImplementedException();

    public Task<AttachmentString> GetStringForMessage(string messageId, string name, Encoding? encoding, Cancellation cancel = default) =>
        throw new NotImplementedException();

    public Task<AttachmentStream> GetStreamForMessage(string messageId, Cancellation cancel = default) =>
        throw new NotImplementedException();

    public Task<AttachmentStream> GetStreamForMessage(string messageId, string name, Cancellation cancel = default) =>
        throw new NotImplementedException();

    public Task ProcessByteArray(string name, Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancel = default) =>
        throw new NotImplementedException();

    public Task ProcessByteArray(Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancel = default) =>
        throw new NotImplementedException();

    public Task ProcessByteArrays(Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancel = default) =>
        throw new NotImplementedException();

    public Task ProcessByteArrayForMessage(string messageId, string name, Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancel = default) =>
        throw new NotImplementedException();

    public Task ProcessByteArrayForMessage(string messageId, Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancel = default) =>
        throw new NotImplementedException();
}