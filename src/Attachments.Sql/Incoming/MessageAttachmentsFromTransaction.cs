using System.Transactions;
using Microsoft.Data.SqlClient;
using NServiceBus.Attachments.Sql;

class MessageAttachmentsFromTransaction :
    IMessageAttachments
{
    Transaction transaction;
    Func<Task<SqlConnection>> connectionFactory;
    string messageId;
    IPersister persister;
    public Cancellation Cancellation { get; }

    public MessageAttachmentsFromTransaction(Transaction transaction, Func<Task<SqlConnection>> connectionFactory, string messageId, IPersister persister, Cancellation cancellation)
    {
        this.transaction = transaction;
        this.connectionFactory = connectionFactory;
        this.messageId = messageId;
        this.persister = persister;
        Cancellation = cancellation;
    }

    public async Task CopyTo(Stream target)
    {
        using var connection = await GetConnection();
        await persister.CopyTo(messageId, "default", connection, null, target, Cancellation);
    }

    async Task<SqlConnection> GetConnection()
    {
        var connection = await connectionFactory();
        connection.EnlistTransaction(transaction);
        return connection;
    }

    public async Task CopyTo(string name, Stream target)
    {
        using var connection = await GetConnection();
        connection.EnlistTransaction(transaction);
        await persister.CopyTo(messageId, name, connection, null, target, Cancellation);
    }

    public async Task ProcessStream(Func<AttachmentStream, Task> action)
    {
        using var connection = await GetConnection();
        await persister.ProcessStream(messageId, "default", connection, null, action, Cancellation);
    }

    public async Task ProcessByteArray(Func<AttachmentBytes, Task> action)
    {
        using var connection = await GetConnection();
        await persister.ProcessByteArray(messageId, "default", connection, null, action, Cancellation);
    }

    public async Task ProcessStream(string name, Func<AttachmentStream, Task> action)
    {
        using var connection = await GetConnection();
        await persister.ProcessStream(messageId, name, connection, null, action, Cancellation);
    }

    public async Task ProcessByteArray(string name, Func<AttachmentBytes, Task> action)
    {
        using var connection = await GetConnection();
        await persister.ProcessByteArray(messageId, name, connection, null, action, Cancellation);
    }

    public async Task ProcessStreams(Func<AttachmentStream, Task> action)
    {
        using var connection = await GetConnection();
        await persister.ProcessStreams(messageId, connection, null, action, Cancellation);
    }

    public async Task ProcessByteArrays(Func<AttachmentBytes, Task> action)
    {
        using var connection = await GetConnection();
        await persister.ProcessByteArrays(messageId, connection, null, action, Cancellation);
    }

    public async Task<AttachmentBytes> GetBytes()
    {
        using var connection = await GetConnection();
        return await persister.GetBytes(messageId, "default", connection, null, Cancellation);
    }

    public async Task<MemoryStream> GetMemoryStream()
    {
        using var connection = await GetConnection();
        return await persister.GetMemoryStream(messageId, "default", connection, null, Cancellation);
    }

    public async Task<AttachmentBytes> GetBytes(string name)
    {
        using var connection = await GetConnection();
        return await persister.GetBytes(messageId, name, connection, null, Cancellation);
    }

    public async Task<MemoryStream> GetMemoryStream(string name)
    {
        using var connection = await GetConnection();
        return await persister.GetMemoryStream(messageId, name, connection, null, Cancellation);
    }

    public async Task<AttachmentString> GetString(Encoding? encoding)
    {
        using var connection = await GetConnection();
        return await persister.GetString(messageId, "default", connection, null, encoding, Cancellation);
    }

    public async Task<AttachmentString> GetString(string name, Encoding? encoding)
    {
        using var connection = await GetConnection();
        return await persister.GetString(messageId, name, connection, null, encoding, Cancellation);
    }

    public async Task<AttachmentStream> GetStream()
    {
        using var connection = await GetConnection();
        return await persister.GetStream(messageId, "default", connection, null, true, Cancellation);
    }

    public async Task<AttachmentStream> GetStream(string name)
    {
        using var connection = await GetConnection();
        return await persister.GetStream(messageId, name, connection, null, true, Cancellation);
    }

    public async Task CopyToForMessage(string messageId, Stream target)
    {
        using var connection = await GetConnection();
        await persister.CopyTo(messageId, "default", connection, null, target, Cancellation);
    }

    public async Task CopyToForMessage(string messageId, string name, Stream target)
    {
        using var connection = await GetConnection();
        await persister.CopyTo(messageId, name, connection, null, target, Cancellation);
    }

    public async Task ProcessStreamForMessage(string messageId, Func<AttachmentStream, Task> action)
    {
        using var connection = await GetConnection();
        await persister.ProcessStream(messageId, "default", connection, null, action, Cancellation);
    }

    public async Task ProcessByteArrayForMessage(string messageId, Func<AttachmentBytes, Task> action)
    {
        using var connection = await GetConnection();
        await persister.ProcessByteArray(messageId, "default", connection, null, action, Cancellation);
    }

    public async Task ProcessStreamForMessage(string messageId, string name, Func<AttachmentStream, Task> action)
    {
        using var connection = await GetConnection();
        await persister.ProcessStream(messageId, name, connection, null, action, Cancellation);
    }

    public async Task ProcessByteArrayForMessage(string messageId, string name, Func<AttachmentBytes, Task> action)
    {
        using var connection = await GetConnection();
        await persister.ProcessByteArray(messageId, name, connection, null, action, Cancellation);
    }

    public async Task ProcessStreamsForMessage(string messageId, Func<AttachmentStream, Task> action)
    {
        using var connection = await GetConnection();
        await persister.ProcessStreams(messageId, connection, null, action, Cancellation);
    }

    public async Task ProcessByteArraysForMessage(string messageId, Func<AttachmentBytes, Task> action)
    {
        using var connection = await GetConnection();
        await persister.ProcessByteArrays(messageId, connection, null, action, Cancellation);
    }

    public async Task<AttachmentBytes> GetBytesForMessage(string messageId)
    {
        using var connection = await GetConnection();
        return await persister.GetBytes(messageId, "default", connection, null, Cancellation);
    }

    public async Task<MemoryStream> GetMemoryStreamForMessage(string messageId)
    {
        using var connection = await GetConnection();
        return await persister.GetMemoryStream(messageId, "default", connection, null, Cancellation);
    }

    public async Task<AttachmentBytes> GetBytesForMessage(string messageId, string name)
    {
        using var connection = await GetConnection();
        return await persister.GetBytes(messageId, name, connection, null, Cancellation);
    }

    public async Task<MemoryStream> GetMemoryStreamForMessage(string messageId, string name)
    {
        using var connection = await GetConnection();
        return await persister.GetMemoryStream(messageId, name, connection, null, Cancellation);
    }

    public async Task<AttachmentString> GetStringForMessage(string messageId, Encoding? encoding)
    {
        using var connection = await GetConnection();
        return await persister.GetString(messageId, "default", connection, null, encoding, Cancellation);
    }

    public async Task<AttachmentString> GetStringForMessage(string messageId, string name, Encoding? encoding)
    {
        using var connection = await GetConnection();
        return await persister.GetString(messageId, name, connection, null, encoding, Cancellation);
    }

    public async Task<AttachmentStream> GetStreamForMessage(string messageId)
    {
        using var connection = await GetConnection();
        return await persister.GetStream(messageId, "default", connection, null, true, Cancellation);
    }

    public async Task<AttachmentStream> GetStreamForMessage(string messageId, string name)
    {
        using var connection = await GetConnection();
        return await persister.GetStream(messageId, name, connection, null, true, Cancellation);
    }

    public async IAsyncEnumerable<AttachmentInfo> GetMetadata()
    {
        using var connection = await GetConnection();
        await foreach (var info in persister.ReadAllMessageInfo(connection, null, messageId, Cancellation))
        {
            yield return info;
        }
    }
}