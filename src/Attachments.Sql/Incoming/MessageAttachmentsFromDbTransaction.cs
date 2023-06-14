using Microsoft.Data.SqlClient;
using NServiceBus.Attachments.Sql;

class MessageAttachmentsFromSqlTransaction :
    IMessageAttachments
{
    SqlTransaction transaction;
    string messageId;
    IPersister persister;

    public MessageAttachmentsFromSqlTransaction(SqlTransaction transaction, string messageId, IPersister persister)
    {
        this.transaction = transaction;
        this.messageId = messageId;
        this.persister = persister;
    }

    public Task CopyTo(Stream target, Cancellation cancellation = default) =>
        persister.CopyTo(messageId, "default", transaction.Connection!, transaction, target, cancellation);

    public Task CopyTo(string name, Stream target, Cancellation cancellation = default) =>
        persister.CopyTo(messageId, name, transaction.Connection!, transaction, target, cancellation);

    public Task ProcessStream(Func<AttachmentStream, Cancellation, Task> action, Cancellation cancellation = default) =>
        persister.ProcessStream(messageId, "default", transaction.Connection!, transaction, action, cancellation);

    public Task ProcessByteArray(Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancellation = default) =>
        persister.ProcessByteArray(messageId, "default", transaction.Connection!, transaction, action, cancellation);

    public Task ProcessStream(string name, Func<AttachmentStream, Cancellation, Task> action, Cancellation cancellation = default) =>
        persister.ProcessStream(messageId, name, transaction.Connection!, transaction, action, cancellation);

    public Task ProcessByteArray(string name, Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancellation = default) =>
        persister.ProcessByteArray(messageId, name, transaction.Connection!, transaction, action, cancellation);

    public Task ProcessStreams(Func<AttachmentStream, Cancellation, Task> action, Cancellation cancellation = default) =>
        persister.ProcessStreams(messageId, transaction.Connection!, transaction, action, cancellation);

    public Task ProcessByteArrays(Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancellation = default) =>
        persister.ProcessByteArrays(messageId, transaction.Connection!, transaction, action, cancellation);

    public Task<AttachmentBytes> GetBytes(Cancellation cancellation = default) =>
        persister.GetBytes(messageId, "default", transaction.Connection!, transaction, cancellation);

    public Task<MemoryStream> GetMemoryStream(Cancellation cancellation = default) =>
        persister.GetMemoryStream(messageId, "default", transaction.Connection!, transaction, cancellation);

    public Task<AttachmentBytes> GetBytes(string name, Cancellation cancellation = default) =>
        persister.GetBytes(messageId, name, transaction.Connection!, transaction, cancellation);

    public Task<MemoryStream> GetMemoryStream(string name, Cancellation cancellation = default) =>
        persister.GetMemoryStream(messageId, name, transaction.Connection!, transaction, cancellation);

    public Task<AttachmentString> GetString(Encoding? encoding, Cancellation cancellation = default) =>
        persister.GetString(messageId, "default", transaction.Connection!, transaction, encoding, cancellation);

    public Task<AttachmentString> GetString(string name, Encoding? encoding, Cancellation cancellation = default) =>
        persister.GetString(messageId, name, transaction.Connection!, transaction, encoding, cancellation);

    public Task<AttachmentStream> GetStream(Cancellation cancellation = default) =>
        persister.GetStream(messageId, "default", transaction.Connection!, transaction, false, cancellation);

    public Task<AttachmentStream> GetStream(string name, Cancellation cancellation = default) =>
        persister.GetStream(messageId, name, transaction.Connection!, transaction, false, cancellation);

    public Task CopyToForMessage(string messageId, Stream target, Cancellation cancellation = default) =>
        persister.CopyTo(messageId, "default", transaction.Connection!, transaction, target, cancellation);

    public Task CopyToForMessage(string messageId, string name, Stream target, Cancellation cancellation = default) =>
        persister.CopyTo(messageId, name, transaction.Connection!, transaction, target, cancellation);

    public Task ProcessStreamForMessage(string messageId, Func<AttachmentStream, Cancellation, Task> action, Cancellation cancellation = default) =>
        persister.ProcessStream(messageId, "default", transaction.Connection!, transaction, action, cancellation);

    public Task ProcessByteArrayForMessage(string messageId, Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancellation = default) =>
        persister.ProcessByteArray(messageId, "default", transaction.Connection!, transaction, action, cancellation);

    public Task ProcessStreamForMessage(string messageId, string name, Func<AttachmentStream, Cancellation, Task> action, Cancellation cancellation = default) =>
        persister.ProcessStream(messageId, name, transaction.Connection!, transaction, action, cancellation);

    public Task ProcessByteArrayForMessage(string messageId, string name, Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancellation = default) =>
        persister.ProcessByteArray(messageId, name, transaction.Connection!, transaction, action, cancellation);

    public Task ProcessStreamsForMessage(string messageId, Func<AttachmentStream, Cancellation, Task> action, Cancellation cancellation = default) =>
        persister.ProcessStreams(messageId, transaction.Connection!, transaction, action, cancellation);

    public Task ProcessByteArraysForMessage(string messageId, Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancellation = default) =>
        persister.ProcessByteArrays(messageId, transaction.Connection!, transaction, action, cancellation);

    public Task<AttachmentBytes> GetBytesForMessage(string messageId, Cancellation cancellation = default) =>
        persister.GetBytes(messageId, "default", transaction.Connection!, transaction, cancellation);

    public Task<MemoryStream> GetMemoryStreamForMessage(string messageId, Cancellation cancellation = default) =>
        persister.GetMemoryStream(messageId, "default", transaction.Connection!, transaction, cancellation);

    public Task<AttachmentBytes> GetBytesForMessage(string messageId, string name, Cancellation cancellation = default) =>
        persister.GetBytes(messageId, name, transaction.Connection!, transaction, cancellation);

    public Task<MemoryStream> GetMemoryStreamForMessage(string messageId, string name, Cancellation cancellation = default) =>
        persister.GetMemoryStream(messageId, name, transaction.Connection!, transaction, cancellation);

    public Task<AttachmentString> GetStringForMessage(string messageId, Encoding? encoding, Cancellation cancellation = default) =>
        persister.GetString(messageId, "default", transaction.Connection!, transaction, encoding, cancellation);

    public Task<AttachmentString> GetStringForMessage(string messageId, string name, Encoding? encoding, Cancellation cancellation = default) =>
        persister.GetString(messageId, name, transaction.Connection!, transaction, encoding, cancellation);

    public Task<AttachmentStream> GetStreamForMessage(string messageId, Cancellation cancellation = default) =>
        persister.GetStream(messageId, "default", transaction.Connection!, transaction, false, cancellation);

    public Task<AttachmentStream> GetStreamForMessage(string messageId, string name, Cancellation cancellation = default) =>
        persister.GetStream(messageId, name, transaction.Connection!, transaction, false, cancellation);

    public IAsyncEnumerable<AttachmentInfo> GetMetadata(Cancellation cancellation = default) =>
        persister.ReadAllMessageInfo(transaction.Connection!, transaction, messageId, cancellation);
}