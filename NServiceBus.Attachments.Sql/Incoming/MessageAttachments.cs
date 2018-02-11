using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus.Attachments;

class MessageAttachments : IMessageAttachments
{
    Func<CancellationToken, Task<SqlConnection>> connectionFactory;
    string messageId;
    Persister persister;
    internal CancellationToken Cancellation;

    internal MessageAttachments(Func<CancellationToken, Task<SqlConnection>> connectionFactory, string messageId, Persister persister, CancellationToken cancellation)
    {
        this.connectionFactory = connectionFactory;
        this.messageId = messageId;
        this.persister = persister;
        Cancellation = cancellation;
    }

    public async Task CopyTo(Stream target, CancellationToken cancellation = default)
    {
        Guard.AgainstNull(target, nameof(target));
        var token = cancellation.Or(Cancellation);
        var connection = await connectionFactory(token).ConfigureAwait(false);
        await persister.CopyTo(messageId, "", connection, null, target, token).ConfigureAwait(false);
    }

    public async Task CopyTo(string name, Stream target, CancellationToken cancellation = default)
    {
        Guard.AgainstNull(name, nameof(name));
        Guard.AgainstNull(target, nameof(target));
        var token = cancellation.Or(Cancellation);
        var connection = await connectionFactory(token).ConfigureAwait(false);
        await persister.CopyTo(messageId, name, connection, null, target, token).ConfigureAwait(false);
    }

    public async Task ProcessStream(Func<Stream, Task> action, CancellationToken cancellation = default)
    {
        Guard.AgainstNull(action, nameof(action));
        var token = cancellation.Or(Cancellation);
        var connection = await connectionFactory(token).ConfigureAwait(false);
        await persister.ProcessStream(messageId, "", connection, null, action, token).ConfigureAwait(false);
    }

    public async Task ProcessStream(string name, Func<Stream, Task> action, CancellationToken cancellation = default)
    {
        Guard.AgainstNull(name, nameof(name));
        Guard.AgainstNull(action, nameof(action));
        var connection = await connectionFactory(cancellation.Or(Cancellation)).ConfigureAwait(false);
        await persister.ProcessStream(messageId, name, connection, null, action, cancellation.Or(Cancellation)).ConfigureAwait(false);
    }

    public async Task ProcessStreams(Func<string, Stream, Task> action, CancellationToken cancellation = default)
    {
        Guard.AgainstNull(action, nameof(action));
        var token = cancellation.Or(Cancellation);
        var connection = await connectionFactory(token).ConfigureAwait(false);
        await persister.ProcessStreams(messageId, connection, null, action, token).ConfigureAwait(false);
    }

    public async Task<byte[]> GetBytes(CancellationToken cancellation = default)
    {
        var token = cancellation.Or(Cancellation);
        var connection = await connectionFactory(token).ConfigureAwait(false);
        return await persister.GetBytes(messageId, "", connection, null, token).ConfigureAwait(false);
    }

    public async Task<byte[]> GetBytes(string name, CancellationToken cancellation = default)
    {
        Guard.AgainstNull(name, nameof(name));
        var token = cancellation.Or(Cancellation);
        var connection = await connectionFactory(token).ConfigureAwait(false);
        return await persister.GetBytes(messageId, name, connection, null, token).ConfigureAwait(false);
    }

    public async Task<Stream> GetStream(CancellationToken cancellation = default)
    {
        var token = cancellation.Or(Cancellation);
        var connection = await connectionFactory(token).ConfigureAwait(false);
        return await persister.GetStream(messageId, "", connection, null, token).ConfigureAwait(false);
    }

    public async Task<Stream> GetStream(string name, CancellationToken cancellation = default)
    {
        Guard.AgainstNull(name, nameof(name));
        var token = cancellation.Or(Cancellation);
        var connection = await connectionFactory(token).ConfigureAwait(false);
        return await persister.GetStream(messageId, name, connection, null, token).ConfigureAwait(false);
    }

    public async Task CopyToForMessage(string messageId, Stream target, CancellationToken cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
        Guard.AgainstNull(target, nameof(target));
        var token = cancellation.Or(Cancellation);
        var connection = await connectionFactory(token).ConfigureAwait(false);
        await persister.CopyTo(messageId, "", connection, null, target, token).ConfigureAwait(false);
    }

    public async Task CopyToForMessage(string messageId, string name, Stream target, CancellationToken cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
        Guard.AgainstNull(name, nameof(name));
        Guard.AgainstNull(target, nameof(target));
        var token = cancellation.Or(Cancellation);
        var connection = await connectionFactory(token).ConfigureAwait(false);
        await persister.CopyTo(messageId, name, connection, null, target, token).ConfigureAwait(false);
    }

    public async Task ProcessStreamForMessage(string messageId, Func<Stream, Task> action, CancellationToken cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
        Guard.AgainstNull(action, nameof(action));
        var token = cancellation.Or(Cancellation);
        var connection = await connectionFactory(token).ConfigureAwait(false);
        await persister.ProcessStream(messageId, "", connection, null, action, token).ConfigureAwait(false);
    }

    public async Task ProcessStreamForMessage(string messageId, string name, Func<Stream, Task> action, CancellationToken cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
        Guard.AgainstNull(name, nameof(name));
        Guard.AgainstNull(action, nameof(action));
        var token = cancellation.Or(Cancellation);
        var connection = await connectionFactory(token).ConfigureAwait(false);
        await persister.ProcessStream(messageId, name, connection, null, action, token).ConfigureAwait(false);
    }

    public async Task ProcessStreamsForMessage(string messageId, Func<string, Stream, Task> action, CancellationToken cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
        Guard.AgainstNull(action, nameof(action));
        var token = cancellation.Or(Cancellation);
        var connection = await connectionFactory(token).ConfigureAwait(false);
        await persister.ProcessStreams(messageId, connection, null, action, token).ConfigureAwait(false);
    }

    public async Task<byte[]> GetBytesForMessage(string messageId, CancellationToken cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
        var token = cancellation.Or(Cancellation);
        var connection = await connectionFactory(token).ConfigureAwait(false);
        return await persister.GetBytes(messageId, "", connection, null, token).ConfigureAwait(false);
    }

    public async Task<byte[]> GetBytesForMessage(string messageId, string name, CancellationToken cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
        Guard.AgainstNull(name, nameof(name));
        var token = cancellation.Or(Cancellation);
        var connection = await connectionFactory(token).ConfigureAwait(false);
        return await persister.GetBytes(messageId, name, connection, null, token).ConfigureAwait(false);
    }

    public async Task<Stream> GetStreamForMessage(string messageId, CancellationToken cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
        var token = cancellation.Or(Cancellation);
        var connection = await connectionFactory(token).ConfigureAwait(false);
        return await persister.GetStream(messageId, "", connection, null, token).ConfigureAwait(false);
    }

    public async Task<Stream> GetStreamForMessage(string messageId, string name, CancellationToken cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
        Guard.AgainstNull(name, nameof(name));
        var token = cancellation.Or(Cancellation);
        var connection = await connectionFactory(token).ConfigureAwait(false);
        return await persister.GetStream(messageId, name, connection, null, token).ConfigureAwait(false);
    }
}