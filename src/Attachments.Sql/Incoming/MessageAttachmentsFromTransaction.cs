using System.Transactions;
using Microsoft.Data.SqlClient;
using NServiceBus.Attachments.Sql;

class MessageAttachmentsFromTransaction :
    IMessageAttachments
{
    Transaction transaction;
    Func<Cancellation, Task<SqlConnection>> connectionFactory;
    string messageId;
    IPersister persister;

    public MessageAttachmentsFromTransaction(Transaction transaction, Func<Cancellation, Task<SqlConnection>> connectionFactory, string messageId, IPersister persister)
    {
        this.transaction = transaction;
        this.connectionFactory = connectionFactory;
        this.messageId = messageId;
        this.persister = persister;
    }

    public async Task CopyTo(Stream target, Cancellation cancellation = default)
    {
        using var connection = await GetConnection(cancellation);
        await persister.CopyTo(messageId, "default", connection, null, target, cancellation);
    }

    async Task<SqlConnection> GetConnection(Cancellation cancellation = default)
    {
        var connection = await connectionFactory(cancellation);
        connection.EnlistTransaction(transaction);
        return connection;
    }

    public async Task CopyTo(string name, Stream target, Cancellation cancellation = default)
    {
        using var connection = await GetConnection(cancellation);
        connection.EnlistTransaction(transaction);
        await persister.CopyTo(messageId, name, connection, null, target, cancellation);
    }

    public async Task ProcessStream(Func<AttachmentStream, Cancellation, Task> action, Cancellation cancellation = default)
    {
        using var connection = await GetConnection(cancellation);
        await persister.ProcessStream(messageId, "default", connection, null, action, cancellation);
    }

    public async Task ProcessByteArray(Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancellation = default)
    {
        using var connection = await GetConnection(cancellation);
        await persister.ProcessByteArray(messageId, "default", connection, null, action, cancellation);
    }

    public async Task ProcessStream(string name, Func<AttachmentStream, Cancellation, Task> action, Cancellation cancellation = default)
    {
        using var connection = await GetConnection(cancellation);
        await persister.ProcessStream(messageId, name, connection, null, action, cancellation);
    }

    public async Task ProcessByteArray(string name, Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancellation = default)
    {
        using var connection = await GetConnection(cancellation);
        await persister.ProcessByteArray(messageId, name, connection, null, action, cancellation);
    }

    public async Task ProcessStreams(Func<AttachmentStream, Cancellation, Task> action, Cancellation cancellation = default)
    {
        using var connection = await GetConnection(cancellation);
        await persister.ProcessStreams(messageId, connection, null, action, cancellation);
    }

    public async Task ProcessByteArrays(Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancellation = default)
    {
        using var connection = await GetConnection(cancellation);
        await persister.ProcessByteArrays(messageId, connection, null, action, cancellation);
    }

    public async Task<AttachmentBytes> GetBytes(Cancellation cancellation = default)
    {
        using var connection = await GetConnection(cancellation);
        return await persister.GetBytes(messageId, "default", connection, null, cancellation);
    }

    public async Task<MemoryStream> GetMemoryStream(Cancellation cancellation = default)
    {
        using var connection = await GetConnection(cancellation);
        return await persister.GetMemoryStream(messageId, "default", connection, null, cancellation);
    }

    public async Task<AttachmentBytes> GetBytes(string name, Cancellation cancellation = default)
    {
        using var connection = await GetConnection(cancellation);
        return await persister.GetBytes(messageId, name, connection, null, cancellation);
    }

    public async Task<MemoryStream> GetMemoryStream(string name, Cancellation cancellation = default)
    {
        using var connection = await GetConnection(cancellation);
        return await persister.GetMemoryStream(messageId, name, connection, null, cancellation);
    }

    public async Task<AttachmentString> GetString(Encoding? encoding, Cancellation cancellation = default)
    {
        using var connection = await GetConnection(cancellation);
        return await persister.GetString(messageId, "default", connection, null, encoding, cancellation);
    }

    public async Task<AttachmentString> GetString(string name, Encoding? encoding, Cancellation cancellation = default)
    {
        using var connection = await GetConnection(cancellation);
        return await persister.GetString(messageId, name, connection, null, encoding, cancellation);
    }

    public async Task<AttachmentStream> GetStream(Cancellation cancellation = default)
    {
        using var connection = await GetConnection(cancellation);
        return await persister.GetStream(messageId, "default", connection, null, true, cancellation);
    }

    public async Task<AttachmentStream> GetStream(string name, Cancellation cancellation = default)
    {
        using var connection = await GetConnection(cancellation);
        return await persister.GetStream(messageId, name, connection, null, true, cancellation);
    }

    public async Task CopyToForMessage(string messageId, Stream target, Cancellation cancellation = default)
    {
        using var connection = await GetConnection(cancellation);
        await persister.CopyTo(messageId, "default", connection, null, target, cancellation);
    }

    public async Task CopyToForMessage(string messageId, string name, Stream target, Cancellation cancellation = default)
    {
        using var connection = await GetConnection(cancellation);
        await persister.CopyTo(messageId, name, connection, null, target, cancellation);
    }

    public async Task ProcessStreamForMessage(string messageId, Func<AttachmentStream, Cancellation, Task> action, Cancellation cancellation = default)
    {
        using var connection = await GetConnection(cancellation);
        await persister.ProcessStream(messageId, "default", connection, null, action, cancellation);
    }

    public async Task ProcessByteArrayForMessage(string messageId, Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancellation = default)
    {
        using var connection = await GetConnection(cancellation);
        await persister.ProcessByteArray(messageId, "default", connection, null, action, cancellation);
    }

    public async Task ProcessStreamForMessage(string messageId, string name, Func<AttachmentStream, Cancellation, Task> action, Cancellation cancellation = default)
    {
        using var connection = await GetConnection(cancellation);
        await persister.ProcessStream(messageId, name, connection, null, action, cancellation);
    }

    public async Task ProcessByteArrayForMessage(string messageId, string name, Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancellation = default)
    {
        using var connection = await GetConnection(cancellation);
        await persister.ProcessByteArray(messageId, name, connection, null, action, cancellation);
    }

    public async Task ProcessStreamsForMessage(string messageId, Func<AttachmentStream, Cancellation, Task> action, Cancellation cancellation = default)
    {
        using var connection = await GetConnection(cancellation);
        await persister.ProcessStreams(messageId, connection, null, action, cancellation);
    }

    public async Task ProcessByteArraysForMessage(string messageId, Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancellation = default)
    {
        using var connection = await GetConnection(cancellation);
        await persister.ProcessByteArrays(messageId, connection, null, action, cancellation);
    }

    public async Task<AttachmentBytes> GetBytesForMessage(string messageId, Cancellation cancellation = default)
    {
        using var connection = await GetConnection(cancellation);
        return await persister.GetBytes(messageId, "default", connection, null, cancellation);
    }

    public async Task<MemoryStream> GetMemoryStreamForMessage(string messageId, Cancellation cancellation = default)
    {
        using var connection = await GetConnection(cancellation);
        return await persister.GetMemoryStream(messageId, "default", connection, null, cancellation);
    }

    public async Task<AttachmentBytes> GetBytesForMessage(string messageId, string name, Cancellation cancellation = default)
    {
        using var connection = await GetConnection(cancellation);
        return await persister.GetBytes(messageId, name, connection, null, cancellation);
    }

    public async Task<MemoryStream> GetMemoryStreamForMessage(string messageId, string name, Cancellation cancellation = default)
    {
        using var connection = await GetConnection(cancellation);
        return await persister.GetMemoryStream(messageId, name, connection, null, cancellation);
    }

    public async Task<AttachmentString> GetStringForMessage(string messageId, Encoding? encoding, Cancellation cancellation = default)
    {
        using var connection = await GetConnection(cancellation);
        return await persister.GetString(messageId, "default", connection, null, encoding, cancellation);
    }

    public async Task<AttachmentString> GetStringForMessage(string messageId, string name, Encoding? encoding, Cancellation cancellation = default)
    {
        using var connection = await GetConnection(cancellation);
        return await persister.GetString(messageId, name, connection, null, encoding, cancellation);
    }

    public async Task<AttachmentStream> GetStreamForMessage(string messageId, Cancellation cancellation = default)
    {
        using var connection = await GetConnection(cancellation);
        return await persister.GetStream(messageId, "default", connection, null, true, cancellation);
    }

    public async Task<AttachmentStream> GetStreamForMessage(string messageId, string name, Cancellation cancellation = default)
    {
        using var connection = await GetConnection(cancellation);
        return await persister.GetStream(messageId, name, connection, null, true, cancellation);
    }

    public async IAsyncEnumerable<AttachmentInfo> GetMetadata([EnumeratorCancellation] Cancellation cancellation = default)
    {
        using var connection = await GetConnection(cancellation);
        await foreach (var info in persister.ReadAllMessageInfo(connection, null, messageId, cancellation))
        {
            yield return info;
        }
    }
}