using Microsoft.Data.SqlClient;
using NServiceBus.Attachments.Sql;

class MessageAttachmentsFromSqlFactory :
    IMessageAttachments
{
    Func<Cancellation, Task<SqlConnection>> connectionFactory;
    string messageId;
    IPersister persister;

    public MessageAttachmentsFromSqlFactory(Func<Cancellation, Task<SqlConnection>> connectionFactory, string messageId, IPersister persister)
    {
        this.connectionFactory = connectionFactory;
        this.messageId = messageId;
        this.persister = persister;
    }

    public async Task CopyTo(Stream target, Cancellation cancel = default)
    {
        using var connection = await connectionFactory(cancel);
        await persister.CopyTo(messageId, "default", connection, null, target, cancel);
    }

    public async Task CopyTo(string name, Stream target, Cancellation cancel = default)
    {
        using var connection = await connectionFactory(cancel);
        await persister.CopyTo(messageId, name, connection, null, target, cancel);
    }

    public async Task ProcessStream(Func<AttachmentStream, Cancellation, Task> action, Cancellation cancel = default)
    {
        using var connection = await connectionFactory(cancel);
        await persister.ProcessStream(messageId, "default", connection, null, action, cancel);
    }

    public async Task ProcessByteArray(Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancel = default)
    {
        using var connection = await connectionFactory(cancel);
        await persister.ProcessByteArray(messageId, "default", connection, null, action, cancel);
    }

    public async Task ProcessStream(string name, Func<AttachmentStream, Cancellation, Task> action, Cancellation cancel = default)
    {
        using var connection = await connectionFactory(cancel);
        await persister.ProcessStream(messageId, name, connection, null, action, cancel);
    }

    public async Task ProcessByteArray(string name, Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancel = default)
    {
        using var connection = await connectionFactory(cancel);
        await persister.ProcessByteArray(messageId, name, connection, null, action, cancel);
    }

    public async Task ProcessStreams(Func<AttachmentStream, Cancellation, Task> action, Cancellation cancel = default)
    {
        using var connection = await connectionFactory(cancel);
        await persister.ProcessStreams(messageId, connection, null, action, cancel);
    }

    public async Task ProcessByteArrays(Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancel = default)
    {
        using var connection = await connectionFactory(cancel);
        await persister.ProcessByteArrays(messageId, connection, null, action, cancel);
    }

    public async Task<AttachmentBytes> GetBytes(Cancellation cancel = default)
    {
        using var connection = await connectionFactory(cancel);
        return await persister.GetBytes(messageId, "default", connection, null, cancel);
    }

    public async Task<MemoryStream> GetMemoryStream(Cancellation cancel = default)
    {
        using var connection = await connectionFactory(cancel);
        return await persister.GetMemoryStream(messageId, "default", connection, null, cancel);
    }

    public async Task<AttachmentString> GetString(Encoding? encoding, Cancellation cancel = default)
    {
        using var connection = await connectionFactory(cancel);
        return await persister.GetString(messageId, "default", connection, null, encoding, cancel);
    }

    public async Task<AttachmentBytes> GetBytes(string name, Cancellation cancel = default)
    {
        using var connection = await connectionFactory(cancel);
        return await persister.GetBytes(messageId, name, connection, null, cancel);
    }

    public async Task<MemoryStream> GetMemoryStream(string name, Cancellation cancel = default)
    {
        using var connection = await connectionFactory(cancel);
        return await persister.GetMemoryStream(messageId, name, connection, null, cancel);
    }

    public async Task<AttachmentString> GetString(string name, Encoding? encoding, Cancellation cancel = default)
    {
        using var connection = await connectionFactory(cancel);
        return await persister.GetString(messageId, name, connection, null, encoding, cancel);
    }

    public async Task<AttachmentStream> GetStream(Cancellation cancel = default)
    {
        var connection = await connectionFactory(cancel);
        return await persister.GetStream(messageId, "default", connection, null, true, cancel);
    }

    public async Task<AttachmentStream> GetStream(string name, Cancellation cancel = default)
    {
        var connection = await connectionFactory(cancel);
        return await persister.GetStream(messageId, name, connection, null, true, cancel);
    }

    public async Task CopyToForMessage(string messageId, Stream target, Cancellation cancel = default)
    {
        using var connection = await connectionFactory(cancel);
        await persister.CopyTo(messageId, "default", connection, null, target, cancel);
    }

    public async Task CopyToForMessage(string messageId, string name, Stream target, Cancellation cancel = default)
    {
        using var connection = await connectionFactory(cancel);
        await persister.CopyTo(messageId, name, connection, null, target, cancel);
    }

    public async Task ProcessStreamForMessage(string messageId, Func<AttachmentStream, Cancellation, Task> action, Cancellation cancel = default)
    {
        using var connection = await connectionFactory(cancel);
        await persister.ProcessStream(messageId, "default", connection, null, action, cancel);
    }

    public async Task ProcessByteArrayForMessage(string messageId, Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancel = default)
    {
        using var connection = await connectionFactory(cancel);
        await persister.ProcessByteArray(messageId, "default", connection, null, action, cancel);
    }

    public async Task ProcessStreamForMessage(string messageId, string name, Func<AttachmentStream, Cancellation, Task> action, Cancellation cancel = default)
    {
        using var connection = await connectionFactory(cancel);
        await persister.ProcessStream(messageId, name, connection, null, action, cancel);
    }

    public async Task ProcessByteArrayForMessage(string messageId, string name, Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancel = default)
    {
        using var connection = await connectionFactory(cancel);
        await persister.ProcessByteArray(messageId, name, connection, null, action, cancel);
    }

    public async Task ProcessStreamsForMessage(string messageId, Func<AttachmentStream, Cancellation, Task> action, Cancellation cancel = default)
    {
        using var connection = await connectionFactory(cancel);
        await persister.ProcessStreams(messageId, connection, null, action, cancel);
    }

    public async Task ProcessByteArraysForMessage(string messageId, Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancel = default)
    {
        using var connection = await connectionFactory(cancel);
        await persister.ProcessByteArrays(messageId, connection, null, action, cancel);
    }

    public async Task<AttachmentBytes> GetBytesForMessage(string messageId, Cancellation cancel = default)
    {
        using var connection = await connectionFactory(cancel);
        return await persister.GetBytes(messageId, "default", connection, null, cancel);
    }

    public async Task<MemoryStream> GetMemoryStreamForMessage(string messageId, Cancellation cancel = default)
    {
        using var connection = await connectionFactory(cancel);
        return await persister.GetMemoryStream(messageId, "default", connection, null, cancel);
    }

    public async Task<AttachmentBytes> GetBytesForMessage(string messageId, string name, Cancellation cancel = default)
    {
        using var connection = await connectionFactory(cancel);
        return await persister.GetBytes(messageId, name, connection, null, cancel);
    }

    public async Task<MemoryStream> GetMemoryStreamForMessage(string messageId, string name, Cancellation cancel = default)
    {
        using var connection = await connectionFactory(cancel);
        return await persister.GetMemoryStream(messageId, name, connection, null, cancel);
    }

    public async Task<AttachmentString> GetStringForMessage(string messageId, Encoding? encoding, Cancellation cancel = default)
    {
        using var connection = await connectionFactory(cancel);
        return await persister.GetString(messageId, "default", connection, null, encoding, cancel);
    }

    public async Task<AttachmentString> GetStringForMessage(string messageId, string name, Encoding? encoding, Cancellation cancel = default)
    {
        using var connection = await connectionFactory(cancel);
        return await persister.GetString(messageId, name, connection, null, encoding, cancel);
    }

    public async Task<AttachmentStream> GetStreamForMessage(string messageId, Cancellation cancel = default)
    {
        var connection = await connectionFactory(cancel);
        return await persister.GetStream(messageId, "default", connection, null, true, cancel);
    }

    public async Task<AttachmentStream> GetStreamForMessage(string messageId, string name, Cancellation cancel = default)
    {
        var connection = await connectionFactory(cancel);
        return await persister.GetStream(messageId, name, connection, null, true, cancel);
    }

    public async IAsyncEnumerable<AttachmentInfo> GetMetadata([EnumeratorCancellation] Cancellation cancel = default)
    {
        using var connection = await connectionFactory(cancel);
        await foreach (var info in persister.ReadAllMessageInfo(connection, null, messageId, cancel))
        {
            yield return info;
        }
    }
}