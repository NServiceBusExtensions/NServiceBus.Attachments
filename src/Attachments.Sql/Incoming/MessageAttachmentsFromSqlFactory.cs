using Microsoft.Data.SqlClient;
using NServiceBus.Attachments.Sql;

class MessageAttachmentsFromSqlFactory :
    IMessageAttachments
{
    Func<Task<SqlConnection>> connectionFactory;
    string messageId;
    IPersister persister;
    public CancellationToken Cancellation { get; }

    public MessageAttachmentsFromSqlFactory(Func<Task<SqlConnection>> connectionFactory, string messageId, IPersister persister, CancellationToken cancellation)
    {
        this.connectionFactory = connectionFactory;
        this.messageId = messageId;
        this.persister = persister;
        Cancellation = cancellation;
    }

    public async Task CopyTo(Stream target)
    {
        using var connection = await connectionFactory();
        await persister.CopyTo(messageId, "default", connection, null, target, Cancellation);
    }

    public async Task CopyTo(string name, Stream target)
    {
        using var connection = await connectionFactory();
        await persister.CopyTo(messageId, name, connection, null, target, Cancellation);
    }

    public async Task ProcessStream(Func<AttachmentStream, Task> action)
    {
        using var connection = await connectionFactory();
        await persister.ProcessStream(messageId, "default", connection, null, action, Cancellation);
    }

    public async Task ProcessStream(string name, Func<AttachmentStream, Task> action)
    {
        using var connection = await connectionFactory();
        await persister.ProcessStream(messageId, name, connection, null, action, Cancellation);
    }

    public async Task ProcessStreams(Func<AttachmentStream, Task> action)
    {
        using var connection = await connectionFactory();
        await persister.ProcessStreams(messageId, connection, null, action, Cancellation);
    }

    public async Task<AttachmentBytes> GetBytes()
    {
        using var connection = await connectionFactory();
        return await persister.GetBytes(messageId, "default", connection, null, Cancellation);
    }

    public async Task<MemoryStream> GetMemoryStream()
    {
        using var connection = await connectionFactory();
        return await persister.GetMemoryStream(messageId, "default", connection, null, Cancellation);
    }

    public async Task<AttachmentString> GetString(Encoding? encoding)
    {
        using var connection = await connectionFactory();
        return await persister.GetString(messageId, "default", connection, null, encoding, Cancellation);
    }

    public async Task<AttachmentBytes> GetBytes(string name)
    {
        using var connection = await connectionFactory();
        return await persister.GetBytes(messageId, name, connection, null, Cancellation);
    }

    public async Task<MemoryStream> GetMemoryStream(string name)
    {
        using var connection = await connectionFactory();
        return await persister.GetMemoryStream(messageId, name, connection, null, Cancellation);
    }

    public async Task<AttachmentString> GetString(string name, Encoding? encoding)
    {
        using var connection = await connectionFactory();
        return await persister.GetString(messageId, name, connection, null, encoding, Cancellation);
    }

    public async Task<AttachmentStream> GetStream()
    {
        var connection = await connectionFactory();
        return await persister.GetStream(messageId, "default", connection, null, true, Cancellation);
    }

    public async Task<AttachmentStream> GetStream(string name)
    {
        var connection = await connectionFactory();
        return await persister.GetStream(messageId, name, connection, null, true, Cancellation);
    }

    public async Task CopyToForMessage(string messageId, Stream target)
    {
        using var connection = await connectionFactory();
        await persister.CopyTo(messageId, "default", connection, null, target, Cancellation);
    }

    public async Task CopyToForMessage(string messageId, string name, Stream target)
    {
        using var connection = await connectionFactory();
        await persister.CopyTo(messageId, name, connection, null, target, Cancellation);
    }

    public async Task ProcessStreamForMessage(string messageId, Func<AttachmentStream, Task> action)
    {
        using var connection = await connectionFactory();
        await persister.ProcessStream(messageId, "default", connection, null, action, Cancellation);
    }

    public async Task ProcessStreamForMessage(string messageId, string name, Func<AttachmentStream, Task> action)
    {
        using var connection = await connectionFactory();
        await persister.ProcessStream(messageId, name, connection, null, action, Cancellation);
    }

    public async Task ProcessStreamsForMessage(string messageId, Func<AttachmentStream, Task> action)
    {
        using var connection = await connectionFactory();
        await persister.ProcessStreams(messageId, connection, null, action, Cancellation);
    }

    public async Task<AttachmentBytes> GetBytesForMessage(string messageId)
    {
        using var connection = await connectionFactory();
        return await persister.GetBytes(messageId, "default", connection, null, Cancellation);
    }

    public async Task<MemoryStream> GetMemoryStreamForMessage(string messageId)
    {
        using var connection = await connectionFactory();
        return await persister.GetMemoryStream(messageId, "default", connection, null, Cancellation);
    }

    public async Task<AttachmentBytes> GetBytesForMessage(string messageId, string name)
    {
        using var connection = await connectionFactory();
        return await persister.GetBytes(messageId, name, connection, null, Cancellation);
    }

    public async Task<MemoryStream> GetMemoryStreamForMessage(string messageId, string name)
    {
        using var connection = await connectionFactory();
        return await persister.GetMemoryStream(messageId, name, connection, null, Cancellation);
    }

    public async Task<AttachmentString> GetStringForMessage(string messageId, Encoding? encoding)
    {
        using var connection = await connectionFactory();
        return await persister.GetString(messageId, "default", connection, null, encoding, Cancellation);
    }

    public async Task<AttachmentString> GetStringForMessage(string messageId, string name, Encoding? encoding)
    {
        using var connection = await connectionFactory();
        return await persister.GetString(messageId, name, connection, null, encoding, Cancellation);
    }

    public async Task<AttachmentStream> GetStreamForMessage(string messageId)
    {
        var connection = await connectionFactory();
        return await persister.GetStream(messageId, "default", connection, null, true, Cancellation);
    }

    public async Task<AttachmentStream> GetStreamForMessage(string messageId, string name)
    {
        var connection = await connectionFactory();
        return await persister.GetStream(messageId, name, connection, null, true, Cancellation);
    }

    public async IAsyncEnumerable<AttachmentInfo> GetMetadata()
    {
        using var connection = await connectionFactory();
        await foreach (var info in persister.ReadAllMessageInfo(connection, null, messageId, Cancellation))
        {
            yield return info;
        }
    }
}