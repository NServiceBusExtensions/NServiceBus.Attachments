using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using NServiceBus.Attachments;

class MessageAttachments : IMessageAttachments
{
    Func<Task<SqlConnection>> connectionFactory;
    string messageId;
    StreamPersister streamPersister;

    internal MessageAttachments(Func<Task<SqlConnection>> connectionFactory, string messageId, StreamPersister streamPersister)
    {
        this.connectionFactory = connectionFactory;
        this.messageId = messageId;
        this.streamPersister = streamPersister;
    }

    public async Task CopyTo(Stream target)
    {
        Guard.AgainstNull(target, nameof(target));
        var sqlConnection = await connectionFactory();
        await streamPersister.CopyTo(messageId, "", sqlConnection, null, target).ConfigureAwait(false);
    }

    public async Task CopyTo(string name, Stream target)
    {
        Guard.AgainstNull(name, nameof(name));
        Guard.AgainstNull(target, nameof(target));
        var sqlConnection = await connectionFactory();
        await streamPersister.CopyTo(messageId, name, sqlConnection, null, target).ConfigureAwait(false);
    }

    public async Task ProcessStream(Func<Stream, Task> action)
    {
        Guard.AgainstNull(action, nameof(action));
        var sqlConnection = await connectionFactory();
        await streamPersister.ProcessStream(messageId, "", sqlConnection, null, action).ConfigureAwait(false);
    }
    public async Task ProcessStream(string name, Func<Stream, Task> action)
    {
        Guard.AgainstNull(name, nameof(name));
        Guard.AgainstNull(action, nameof(action));
        var sqlConnection = await connectionFactory();
        await streamPersister.ProcessStream(messageId, name, sqlConnection, null, action).ConfigureAwait(false);
    }

    public async Task ProcessStreams(Func<string, Stream, Task> action)
    {
        Guard.AgainstNull(action, nameof(action));
        var sqlConnection = await connectionFactory();
        await streamPersister.ProcessStreams(messageId, sqlConnection, null, action).ConfigureAwait(false);
    }

    public async Task<byte[]> GetBytes()
    {
        var sqlConnection = await connectionFactory();
        return await streamPersister.GetBytes(messageId, "", sqlConnection, null).ConfigureAwait(false);
    }

    public async Task<byte[]> GetBytes(string name)
    {
        Guard.AgainstNull(name, nameof(name));
        var sqlConnection = await connectionFactory();
        return await streamPersister.GetBytes(messageId, name, sqlConnection, null).ConfigureAwait(false);
    }

    public async Task<Stream> GetStream()
    {
        var sqlConnection = await connectionFactory();
        return await streamPersister.GetStream(messageId, "", sqlConnection, null).ConfigureAwait(false);
    }

    public async Task<Stream> GetStream(string name)
    {
        Guard.AgainstNull(name, nameof(name));
        var sqlConnection = await connectionFactory();
        return await streamPersister.GetStream(messageId, name, sqlConnection, null).ConfigureAwait(false);
    }

    public async Task CopyToForMessage(string messageId, Stream target)
    {
        Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
        Guard.AgainstNull(target, nameof(target));
        var sqlConnection = await connectionFactory();
        await streamPersister.CopyTo(messageId, "", sqlConnection, null, target).ConfigureAwait(false);
    }

    public async Task CopyToForMessage(string messageId, string name, Stream target)
    {
        Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
        Guard.AgainstNull(name, nameof(name));
        Guard.AgainstNull(target, nameof(target));
        var sqlConnection = await connectionFactory();
        await streamPersister.CopyTo(messageId, name, sqlConnection, null, target).ConfigureAwait(false);
    }

    public async Task ProcessStreamForMessage(string messageId, Func<Stream, Task> action)
    {
        Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
        Guard.AgainstNull(action, nameof(action));
        var sqlConnection = await connectionFactory();
        await streamPersister.ProcessStream(messageId, "", sqlConnection, null, action).ConfigureAwait(false);
    }
    public async Task ProcessStreamForMessage(string messageId, string name, Func<Stream, Task> action)
    {
        Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
        Guard.AgainstNull(name, nameof(name));
        Guard.AgainstNull(action, nameof(action));
        var sqlConnection = await connectionFactory();
        await streamPersister.ProcessStream(messageId, name, sqlConnection, null, action).ConfigureAwait(false);
    }

    public async Task ProcessStreamsForMessage(string messageId, Func<string, Stream, Task> action)
    {
        Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
        Guard.AgainstNull(action, nameof(action));
        var sqlConnection = await connectionFactory();
        await streamPersister.ProcessStreams(messageId, sqlConnection, null, action).ConfigureAwait(false);
    }

    public async Task<byte[]> GetBytesForMessage(string messageId)
    {
        Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
        var sqlConnection = await connectionFactory();
        return await streamPersister.GetBytes(messageId, "", sqlConnection, null).ConfigureAwait(false);
    }

    public async Task<byte[]> GetBytesForMessage(string messageId, string name)
    {
        Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
        Guard.AgainstNull(name, nameof(name));
        var sqlConnection = await connectionFactory();
        return await streamPersister.GetBytes(messageId, name, sqlConnection, null).ConfigureAwait(false);
    }

    public async Task<Stream> GetStreamForMessage(string messageId)
    {
        Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
        var sqlConnection = await connectionFactory();
        return await streamPersister.GetStream(messageId, "", sqlConnection, null).ConfigureAwait(false);
    }

    public async Task<Stream> GetStreamForMessage(string messageId, string name)
    {
        Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
        Guard.AgainstNull(name, nameof(name));
        var sqlConnection = await connectionFactory();
        return await streamPersister.GetStream(messageId, name, sqlConnection, null).ConfigureAwait(false);
    }
}