﻿using System.Data.Common;
using System.Transactions;
using NServiceBus.Attachments.Sql;

class MessageAttachmentsFromTransaction :
    IMessageAttachments
{
    Transaction transaction;
    Func<Task<DbConnection>> connectionFactory;
    string messageId;
    IPersister persister;

    public MessageAttachmentsFromTransaction(Transaction transaction, Func<Task<DbConnection>> connectionFactory, string messageId, IPersister persister)
    {
        this.transaction = transaction;
        this.connectionFactory = connectionFactory;
        this.messageId = messageId;
        this.persister = persister;
    }

    public async Task CopyTo(Stream target, CancellationToken cancellation = default)
    {
        using var connection = await GetConnection();
        await persister.CopyTo(messageId, "default", connection, null, target, cancellation);
    }

    private async Task<DbConnection> GetConnection()
    {
        var connection = await connectionFactory();
        connection.EnlistTransaction(transaction);
        return connection;
    }

    public async Task CopyTo(string name, Stream target, CancellationToken cancellation = default)
    {
        using var connection = await GetConnection();
        connection.EnlistTransaction(transaction);
        await persister.CopyTo(messageId, name, connection, null, target, cancellation);
    }

    public async Task ProcessStream(Func<AttachmentStream, Task> action, CancellationToken cancellation = default)
    {
        using var connection = await GetConnection();
        await persister.ProcessStream(messageId, "default", connection, null, action, cancellation);
    }

    public async Task ProcessStream(string name, Func<AttachmentStream, Task> action, CancellationToken cancellation = default)
    {
        using var connection = await GetConnection();
        await persister.ProcessStream(messageId, name, connection, null, action, cancellation);
    }

    public async Task ProcessStreams(Func<AttachmentStream, Task> action, CancellationToken cancellation = default)
    {
        using var connection = await GetConnection();
        await persister.ProcessStreams(messageId, connection, null, action, cancellation);
    }

    public async Task<AttachmentBytes> GetBytes(CancellationToken cancellation = default)
    {
        using var connection = await GetConnection();
        return await persister.GetBytes(messageId, "default", connection, null, cancellation);
    }

    public async Task<AttachmentBytes> GetBytes(string name, CancellationToken cancellation = default)
    {
        using var connection = await GetConnection();
        return await persister.GetBytes(messageId, name, connection, null, cancellation);
    }

    public async Task<AttachmentString> GetString(Encoding? encoding, CancellationToken cancellation = default)
    {
        using var connection = await GetConnection();
        return await persister.GetString(messageId, "default", connection, null, encoding, cancellation);
    }

    public async Task<AttachmentString> GetString(string name, Encoding? encoding, CancellationToken cancellation = default)
    {
        using var connection = await GetConnection();
        return await persister.GetString(messageId, name, connection, null, encoding, cancellation);
    }

    public async Task<AttachmentStream> GetStream(CancellationToken cancellation = default)
    {
        using var connection = await GetConnection();
        return await persister.GetStream(messageId, "default", connection, null, true, cancellation);
    }

    public async Task<AttachmentStream> GetStream(string name, CancellationToken cancellation = default)
    {
        using var connection = await GetConnection();
        return await persister.GetStream(messageId, name, connection, null, true, cancellation);
    }

    public async Task CopyToForMessage(string messageId, Stream target, CancellationToken cancellation = default)
    {
        using var connection = await GetConnection();
        await persister.CopyTo(messageId, "default", connection, null, target, cancellation);
    }

    public async Task CopyToForMessage(string messageId, string name, Stream target, CancellationToken cancellation = default)
    {
        using var connection = await GetConnection();
        await persister.CopyTo(messageId, name, connection, null, target, cancellation);
    }

    public async Task ProcessStreamForMessage(string messageId, Func<AttachmentStream, Task> action, CancellationToken cancellation = default)
    {
        using var connection = await GetConnection();
        await persister.ProcessStream(messageId, "default", connection, null, action, cancellation);
    }

    public async Task ProcessStreamForMessage(string messageId, string name, Func<AttachmentStream, Task> action, CancellationToken cancellation = default)
    {
        using var connection = await GetConnection();
        await persister.ProcessStream(messageId, name, connection, null, action, cancellation);
    }

    public async Task ProcessStreamsForMessage(string messageId, Func<AttachmentStream, Task> action, CancellationToken cancellation = default)
    {
        using var connection = await GetConnection();
        await persister.ProcessStreams(messageId, connection, null, action, cancellation);
    }

    public async Task<AttachmentBytes> GetBytesForMessage(string messageId, CancellationToken cancellation = default)
    {
        using var connection = await GetConnection();
        return await persister.GetBytes(messageId, "default", connection, null, cancellation);
    }

    public async Task<AttachmentBytes> GetBytesForMessage(string messageId, string name, CancellationToken cancellation = default)
    {
        using var connection = await GetConnection();
        return await persister.GetBytes(messageId, name, connection, null, cancellation);
    }

    public async Task<AttachmentString> GetStringForMessage(string messageId, Encoding? encoding, CancellationToken cancellation = default)
    {
        using var connection = await GetConnection();
        return await persister.GetString(messageId, "default", connection, null, encoding, cancellation);
    }

    public async Task<AttachmentString> GetStringForMessage(string messageId, string name, Encoding? encoding, CancellationToken cancellation = default)
    {
        using var connection = await GetConnection();
        return await persister.GetString(messageId, name, connection, null, encoding, cancellation);
    }

    public async Task<AttachmentStream> GetStreamForMessage(string messageId, CancellationToken cancellation = default)
    {
        using var connection = await GetConnection();
        return await persister.GetStream(messageId, "default", connection, null, true, cancellation);
    }

    public async Task<AttachmentStream> GetStreamForMessage(string messageId, string name, CancellationToken cancellation = default)
    {
        using var connection = await GetConnection();
        return await persister.GetStream(messageId, name, connection, null, true, cancellation);
    }

    public async IAsyncEnumerable<AttachmentInfo> GetMetadata([EnumeratorCancellation] CancellationToken cancellation = default)
    {
        using var connection = await GetConnection();
        await foreach (var info in persister.ReadAllMessageInfo(connection, null, messageId, cancellation))
        {
            yield return info;
        }
    }
}