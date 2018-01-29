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

    public async Task CopyTo(string name, Stream target)
    {
        Guard.AgainstNull(name, nameof(name));
        Guard.AgainstNull(target, nameof(target));
        var sqlConnection = await connectionFactory();
        await streamPersister.CopyTo(messageId, name, sqlConnection, null, target).ConfigureAwait(false);
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

    public async Task<byte[]> GetBytes(string name)
    {
        Guard.AgainstNull(name, nameof(name));
        var sqlConnection = await connectionFactory();
        return await streamPersister.GetBytes(messageId, name, sqlConnection, null).ConfigureAwait(false);
    }

    public async Task<Stream> GetStream(string name)
    {
        Guard.AgainstNull(name, nameof(name));
        var sqlConnection = await connectionFactory();
        return await streamPersister.GetStream(messageId, name, sqlConnection, null).ConfigureAwait(false);
    }
}