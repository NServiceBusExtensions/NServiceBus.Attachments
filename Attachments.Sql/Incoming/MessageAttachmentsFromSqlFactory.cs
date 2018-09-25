using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus.Attachments.Sql;

class MessageAttachmentsFromSqlFactory : IMessageAttachments
{
    Func<Task<SqlConnection>> connectionFactory;
    string messageId;
    IPersister persister;

    public MessageAttachmentsFromSqlFactory(Func<Task<SqlConnection>> connectionFactory, string messageId, IPersister persister)
    {
        this.connectionFactory = connectionFactory;
        this.messageId = messageId;
        this.persister = persister;
    }

    public async Task CopyTo(Stream target, CancellationToken cancellation = default)
    {
        using (var connection = await connectionFactory().ConfigureAwait(false))
        {
            await persister.CopyTo(messageId, "default", connection, null, target, cancellation).ConfigureAwait(false);
        }
    }

    public async Task CopyTo(string name, Stream target, CancellationToken cancellation = default)
    {
        using (var connection = await connectionFactory().ConfigureAwait(false))
        {
            await persister.CopyTo(messageId, name, connection, null, target, cancellation).ConfigureAwait(false);
        }
    }

    public async Task ProcessStream(Func<AttachmentStream, Task> action, CancellationToken cancellation = default)
    {
        using (var connection = await connectionFactory().ConfigureAwait(false))
        {
            await persister.ProcessStream(messageId, "default", connection, null, action, cancellation).ConfigureAwait(false);
        }
    }

    public async Task ProcessStream(string name, Func<AttachmentStream, Task> action, CancellationToken cancellation = default)
    {
        using (var connection = await connectionFactory().ConfigureAwait(false))
        {
            await persister.ProcessStream(messageId, name, connection, null, action, cancellation).ConfigureAwait(false);
        }
    }

    public async Task ProcessStreams(Func<string, AttachmentStream, Task> action, CancellationToken cancellation = default)
    {
        using (var connection = await connectionFactory().ConfigureAwait(false))
        {
            await persister.ProcessStreams(messageId, connection, null, action, cancellation).ConfigureAwait(false);
        }
    }

    public async Task<AttachmentBytes> GetBytes(CancellationToken cancellation = default)
    {
        using (var connection = await connectionFactory().ConfigureAwait(false))
        {
            return await persister.GetBytes(messageId, "default", connection, null, cancellation).ConfigureAwait(false);
        }
    }

    public async Task<AttachmentBytes> GetBytes(string name, CancellationToken cancellation = default)
    {
        using (var connection = await connectionFactory().ConfigureAwait(false))
        {
            return await persister.GetBytes(messageId, name, connection, null, cancellation).ConfigureAwait(false);
        }
    }

    public async Task<AttachmentStream> GetStream(CancellationToken cancellation = default)
    {
        var connection = await connectionFactory().ConfigureAwait(false);
        return await persister.GetStream(messageId, "default", connection, null, cancellation).ConfigureAwait(false);
    }

    public async Task<AttachmentStream> GetStream(string name, CancellationToken cancellation = default)
    {
        var connection = await connectionFactory().ConfigureAwait(false);
        return await persister.GetStream(messageId, name, connection, null, cancellation).ConfigureAwait(false);
    }

    public async Task CopyToForMessage(string messageId, Stream target, CancellationToken cancellation = default)
    {
        using (var connection = await connectionFactory().ConfigureAwait(false))
        {
            await persister.CopyTo(messageId, "default", connection, null, target, cancellation).ConfigureAwait(false);
        }
    }

    public async Task CopyToForMessage(string messageId, string name, Stream target, CancellationToken cancellation = default)
    {
        using (var connection = await connectionFactory().ConfigureAwait(false))
        {
            await persister.CopyTo(messageId, name, connection, null, target, cancellation).ConfigureAwait(false);
        }
    }

    public async Task ProcessStreamForMessage(string messageId, Func<AttachmentStream, Task> action, CancellationToken cancellation = default)
    {
        using (var connection = await connectionFactory().ConfigureAwait(false))
        {
            await persister.ProcessStream(messageId, "default", connection, null, action, cancellation).ConfigureAwait(false);
        }
    }

    public async Task ProcessStreamForMessage(string messageId, string name, Func<AttachmentStream, Task> action, CancellationToken cancellation = default)
    {
        using (var connection = await connectionFactory().ConfigureAwait(false))
        {
            await persister.ProcessStream(messageId, name, connection, null, action, cancellation).ConfigureAwait(false);
        }
    }

    public async Task ProcessStreamsForMessage(string messageId, Func<string, AttachmentStream, Task> action, CancellationToken cancellation = default)
    {
        using (var connection = await connectionFactory().ConfigureAwait(false))
        {
            await persister.ProcessStreams(messageId, connection, null, action, cancellation).ConfigureAwait(false);
        }
    }

    public async Task<AttachmentBytes> GetBytesForMessage(string messageId, CancellationToken cancellation = default)
    {
        using (var connection = await connectionFactory().ConfigureAwait(false))
        {
            return await persister.GetBytes(messageId, "default", connection, null, cancellation).ConfigureAwait(false);
        }
    }

    public async Task<AttachmentBytes> GetBytesForMessage(string messageId, string name, CancellationToken cancellation = default)
    {
        using (var connection = await connectionFactory().ConfigureAwait(false))
        {
            return await persister.GetBytes(messageId, name, connection, null, cancellation).ConfigureAwait(false);
        }
    }

    public async Task<AttachmentStream> GetStreamForMessage(string messageId, CancellationToken cancellation = default)
    {
        var connection = await connectionFactory().ConfigureAwait(false);
        return await persister.GetStream(messageId, "default", connection, null, cancellation).ConfigureAwait(false);
    }

    public async Task<AttachmentStream> GetStreamForMessage(string messageId, string name, CancellationToken cancellation = default)
    {
        var connection = await connectionFactory().ConfigureAwait(false);
        return await persister.GetStream(messageId, name, connection, null, cancellation).ConfigureAwait(false);
    }
}