using Microsoft.Data.SqlClient;
using NServiceBus.Attachments.Sql;

class MessageAttachmentsFromSqlTransaction :
    IMessageAttachments
{
    SqlTransaction transaction;
    string messageId;
    IPersister persister;
    CancellationToken cancellation;

    public MessageAttachmentsFromSqlTransaction(SqlTransaction transaction, string messageId, IPersister persister, CancellationToken cancellation)
    {
        this.transaction = transaction;
        this.messageId = messageId;
        this.persister = persister;
        this.cancellation = cancellation;
    }

    public Task CopyTo(Stream target) =>
        persister.CopyTo(messageId, "default", transaction.Connection!, transaction, target, cancellation);

    public Task CopyTo(string name, Stream target) =>
        persister.CopyTo(messageId, name, transaction.Connection!, transaction, target, cancellation);

    public Task ProcessStream(Func<AttachmentStream, Task> action) =>
        persister.ProcessStream(messageId, "default", transaction.Connection!, transaction, action, cancellation);

    public Task ProcessStream(string name, Func<AttachmentStream, Task> action) =>
        persister.ProcessStream(messageId, name, transaction.Connection!, transaction, action, cancellation);

    public Task ProcessStreams(Func<AttachmentStream, Task> action) =>
        persister.ProcessStreams(messageId, transaction.Connection!, transaction, action, cancellation);

    public Task<AttachmentBytes> GetBytes() =>
        persister.GetBytes(messageId, "default", transaction.Connection!, transaction, cancellation);

    public Task<MemoryStream> GetMemoryStream() =>
        persister.GetMemoryStream(messageId, "default", transaction.Connection!, transaction, cancellation);

    public Task<AttachmentBytes> GetBytes(string name) =>
        persister.GetBytes(messageId, name, transaction.Connection!, transaction, cancellation);

    public Task<MemoryStream> GetMemoryStream(string name) =>
        persister.GetMemoryStream(messageId, name, transaction.Connection!, transaction, cancellation);

    public Task<AttachmentString> GetString(Encoding? encoding) =>
        persister.GetString(messageId, "default", transaction.Connection!, transaction, encoding, cancellation);

    public Task<AttachmentString> GetString(string name, Encoding? encoding) =>
        persister.GetString(messageId, name, transaction.Connection!, transaction, encoding, cancellation);

    public Task<AttachmentStream> GetStream() =>
        persister.GetStream(messageId, "default", transaction.Connection!, transaction, false, cancellation);

    public Task<AttachmentStream> GetStream(string name) =>
        persister.GetStream(messageId, name, transaction.Connection!, transaction, false, cancellation);

    public Task CopyToForMessage(string messageId, Stream target) =>
        persister.CopyTo(messageId, "default", transaction.Connection!, transaction, target, cancellation);

    public Task CopyToForMessage(string messageId, string name, Stream target) =>
        persister.CopyTo(messageId, name, transaction.Connection!, transaction, target, cancellation);

    public Task ProcessStreamForMessage(string messageId, Func<AttachmentStream, Task> action) =>
        persister.ProcessStream(messageId, "default", transaction.Connection!, transaction, action, cancellation);

    public Task ProcessStreamForMessage(string messageId, string name, Func<AttachmentStream, Task> action) =>
        persister.ProcessStream(messageId, name, transaction.Connection!, transaction, action, cancellation);

    public Task ProcessStreamsForMessage(string messageId, Func<AttachmentStream, Task> action) =>
        persister.ProcessStreams(messageId, transaction.Connection!, transaction, action, cancellation);

    public Task<AttachmentBytes> GetBytesForMessage(string messageId) =>
        persister.GetBytes(messageId, "default", transaction.Connection!, transaction, cancellation);

    public Task<MemoryStream> GetMemoryStreamForMessage(string messageId) =>
        persister.GetMemoryStream(messageId, "default", transaction.Connection!, transaction, cancellation);

    public Task<AttachmentBytes> GetBytesForMessage(string messageId, string name) =>
        persister.GetBytes(messageId, name, transaction.Connection!, transaction, cancellation);

    public Task<MemoryStream> GetMemoryStreamForMessage(string messageId, string name) =>
        persister.GetMemoryStream(messageId, name, transaction.Connection!, transaction, cancellation);

    public Task<AttachmentString> GetStringForMessage(string messageId, Encoding? encoding) =>
        persister.GetString(messageId, "default", transaction.Connection!, transaction, encoding, cancellation);

    public Task<AttachmentString> GetStringForMessage(string messageId, string name, Encoding? encoding) =>
        persister.GetString(messageId, name, transaction.Connection!, transaction, encoding, cancellation);

    public Task<AttachmentStream> GetStreamForMessage(string messageId) =>
        persister.GetStream(messageId, "default", transaction.Connection!, transaction, false, cancellation);

    public Task<AttachmentStream> GetStreamForMessage(string messageId, string name) =>
        persister.GetStream(messageId, name, transaction.Connection!, transaction, false, cancellation);

    public IAsyncEnumerable<AttachmentInfo> GetMetadata() =>
        persister.ReadAllMessageInfo(transaction.Connection!, transaction, messageId, cancellation);
}