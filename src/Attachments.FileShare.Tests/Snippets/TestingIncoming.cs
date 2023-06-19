﻿using NServiceBus.Attachments.FileShare;
using NServiceBus.Attachments.FileShare.Testing;
using NServiceBus.Testing;
// ReSharper disable UnusedMember.Global

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
        public override Task<AttachmentBytes> GetBytes(Cancel cancel = default)
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

class MyMessageAttachments :
    IMessageAttachments
{
    public Task<AttachmentStream> GetStream(Cancel cancel = default) =>
        throw new NotImplementedException();

    public Task<AttachmentStream> GetStream(string name, Cancel cancel = default) =>
        throw new NotImplementedException();

    public Task<AttachmentStream> GetStreamForMessage(string messageId, Cancel cancel = default) =>
        throw new NotImplementedException();

    public Task<AttachmentStream> GetStreamForMessage(string messageId, string name, Cancel cancel = default) =>
        throw new NotImplementedException();

    public Task CopyTo(string name, Stream target, Cancel cancel = default) =>
        throw new NotImplementedException();

    public Task CopyTo(Stream target, Cancel cancel = default) =>
        throw new NotImplementedException();

    public Task ProcessStream(string name, Func<AttachmentStream, Cancel, Task> action, Cancel cancel = default) =>
        throw new NotImplementedException();

    public Task ProcessStream(Func<AttachmentStream, Cancel, Task> action, Cancel cancel = default) =>
        throw new NotImplementedException();

    public Task ProcessStreams(Func<AttachmentStream, Cancel, Task> action, Cancel cancel = default) =>
        throw new NotImplementedException();

    public IAsyncEnumerable<AttachmentInfo> GetMetadata(Cancel cancel = default) =>
        throw new NotImplementedException();

    public Task<AttachmentBytes> GetBytes(Cancel cancel = default) =>
        throw new NotImplementedException();

    public Task<MemoryStream> GetMemoryStream(Cancel cancel = default) =>
        throw new NotImplementedException();

    public Task<AttachmentBytes> GetBytes(string name, Cancel cancel = default) =>
        throw new NotImplementedException();

    public Task<MemoryStream> GetMemoryStream(string name, Cancel cancel = default) =>
        throw new NotImplementedException();

    public Task<AttachmentString> GetString(Encoding? encoding, Cancel cancel = default) =>
        throw new NotImplementedException();

    public Task<AttachmentString> GetString(string name, Encoding? encoding, Cancel cancel = default) =>
        throw new NotImplementedException();

    public Task CopyToForMessage(string messageId, string name, Stream target, Cancel cancel = default) =>
        throw new NotImplementedException();

    public Task CopyToForMessage(string messageId, Stream target, Cancel cancel = default) =>
        throw new NotImplementedException();

    public Task ProcessStreamForMessage(string messageId, string name, Func<AttachmentStream, Cancel, Task> action, Cancel cancel = default) =>
        throw new NotImplementedException();

    public Task ProcessStreamForMessage(string messageId, Func<AttachmentStream, Cancel, Task> action, Cancel cancel = default) =>
        throw new NotImplementedException();

    public Task ProcessStreamsForMessage(string messageId, Func<AttachmentStream, Cancel, Task> action, Cancel cancel = default) =>
        throw new NotImplementedException();

    public Task<AttachmentBytes> GetBytesForMessage(string messageId, Cancel cancel = default) =>
        throw new NotImplementedException();

    public Task<MemoryStream> GetMemoryStreamForMessage(string messageId, Cancel cancel = default) =>
        throw new NotImplementedException();

    public Task<AttachmentBytes> GetBytesForMessage(string messageId, string name, Cancel cancel = default) =>
        throw new NotImplementedException();

    public Task<MemoryStream> GetMemoryStreamForMessage(string messageId, string name, Cancel cancel = default) =>
        throw new NotImplementedException();

    public Task<AttachmentString> GetStringForMessage(string messageId, Encoding? encoding, Cancel cancel = default) =>
        throw new NotImplementedException();

    public Task<AttachmentString> GetStringForMessage(string messageId, string name, Encoding? encoding, Cancel cancel = default) =>
        throw new NotImplementedException();
}