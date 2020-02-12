using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus.Attachments.Sql;

class MessageAttachmentsFromDbTransaction :
    IMessageAttachments
{
    DbTransaction transaction;
    string messageId;
    IPersister persister;

    public MessageAttachmentsFromDbTransaction(DbTransaction transaction, string messageId, IPersister persister)
    {
        this.transaction = transaction;
        this.messageId = messageId;
        this.persister = persister;
    }

    public Task CopyTo(Stream target, CancellationToken cancellation = default)
    {
        return persister.CopyTo(messageId, "default", transaction.Connection, transaction, target, cancellation);
    }

    public Task CopyTo(string name, Stream target, CancellationToken cancellation = default)
    {
        return persister.CopyTo(messageId, name, transaction.Connection, transaction, target, cancellation);
    }

    public Task ProcessStream(Func<AttachmentStream, Task> action, CancellationToken cancellation = default)
    {
        return persister.ProcessStream(messageId, "default", transaction.Connection, transaction, action, cancellation);
    }

    public Task ProcessStream(string name, Func<AttachmentStream, Task> action, CancellationToken cancellation = default)
    {
        return persister.ProcessStream(messageId, name, transaction.Connection, transaction, action, cancellation);
    }

    public Task ProcessStreams(Func<AttachmentStream, Task> action, CancellationToken cancellation = default)
    {
        return persister.ProcessStreams(messageId, transaction.Connection, transaction, action, cancellation);
    }

    public Task<AttachmentBytes> GetBytes(CancellationToken cancellation = default)
    {
        return persister.GetBytes(messageId, "default", transaction.Connection, transaction, cancellation);
    }

    public Task<AttachmentBytes> GetBytes(string name, CancellationToken cancellation = default)
    {
        return persister.GetBytes(messageId, name, transaction.Connection, transaction, cancellation);
    }

    public Task<AttachmentString> GetString(CancellationToken cancellation = default)
    {
        return persister.GetString(messageId, "default", transaction.Connection, transaction, cancellation);
    }

    public Task<AttachmentString> GetString(string name, CancellationToken cancellation = default)
    {
        return persister.GetString(messageId, name, transaction.Connection, transaction, cancellation);
    }

    public Task<AttachmentStream> GetStream(CancellationToken cancellation = default)
    {
        return persister.GetStream(messageId, "default", transaction.Connection, transaction, false, cancellation);
    }

    public Task<AttachmentStream> GetStream(string name, CancellationToken cancellation = default)
    {
        return persister.GetStream(messageId, name, transaction.Connection, transaction, false, cancellation);
    }

    public Task CopyToForMessage(string messageId, Stream target, CancellationToken cancellation = default)
    {
        return persister.CopyTo(messageId, "default", transaction.Connection, transaction, target, cancellation);
    }

    public Task CopyToForMessage(string messageId, string name, Stream target, CancellationToken cancellation = default)
    {
        return persister.CopyTo(messageId, name, transaction.Connection, transaction, target, cancellation);
    }

    public Task ProcessStreamForMessage(string messageId, Func<AttachmentStream, Task> action, CancellationToken cancellation = default)
    {
        return persister.ProcessStream(messageId, "default", transaction.Connection, transaction, action, cancellation);
    }

    public Task ProcessStreamForMessage(string messageId, string name, Func<AttachmentStream, Task> action, CancellationToken cancellation = default)
    {
        return persister.ProcessStream(messageId, name, transaction.Connection, transaction, action, cancellation);
    }

    public Task ProcessStreamsForMessage(string messageId, Func<AttachmentStream, Task> action, CancellationToken cancellation = default)
    {
        return persister.ProcessStreams(messageId, transaction.Connection, transaction, action, cancellation);
    }

    public Task<AttachmentBytes> GetBytesForMessage(string messageId, CancellationToken cancellation = default)
    {
        return persister.GetBytes(messageId, "default", transaction.Connection, transaction, cancellation);
    }

    public Task<AttachmentBytes> GetBytesForMessage(string messageId, string name, CancellationToken cancellation = default)
    {
        return persister.GetBytes(messageId, name, transaction.Connection, transaction, cancellation);
    }

    public Task<AttachmentString> GetStringForMessage(string messageId, CancellationToken cancellation = default)
    {
        return persister.GetString(messageId, "default", transaction.Connection, transaction, cancellation);
    }

    public Task<AttachmentString> GetStringForMessage(string messageId, string name, CancellationToken cancellation = default)
    {
        return persister.GetString(messageId, name, transaction.Connection, transaction, cancellation);
    }

    public Task<AttachmentStream> GetStreamForMessage(string messageId, CancellationToken cancellation = default)
    {
        return persister.GetStream(messageId, "default", transaction.Connection, transaction, false, cancellation);
    }

    public Task<AttachmentStream> GetStreamForMessage(string messageId, string name, CancellationToken cancellation = default)
    {
        return persister.GetStream(messageId, name, transaction.Connection, transaction, false, cancellation);
    }

    public IAsyncEnumerable<AttachmentInfo> GetMetadata(CancellationToken cancellation = default)
    {
        return persister.ReadAllMessageInfo(transaction.Connection, transaction, messageId, cancellation);
    }
}