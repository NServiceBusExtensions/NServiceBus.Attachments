using Microsoft.Data.SqlClient;
using NServiceBus.Attachments.Sql;

class MessageAttachmentsFromSqlTransaction :
    IMessageAttachments
{
    SqlTransaction transaction;
    string messageId;
    IPersister persister;
    public Cancellation Cancellation { get; }

    public MessageAttachmentsFromSqlTransaction(SqlTransaction transaction, string messageId, IPersister persister, Cancellation cancellation)
    {
        this.transaction = transaction;
        this.messageId = messageId;
        this.persister = persister;
        Cancellation = cancellation;
    }

    public Task CopyTo(Stream target) =>
        persister.CopyTo(messageId, "default", transaction.Connection!, transaction, target, Cancellation);

    public Task CopyTo(string name, Stream target) =>
        persister.CopyTo(messageId, name, transaction.Connection!, transaction, target, Cancellation);

    public Task ProcessStream(Func<AttachmentStream, Task> action) =>
        persister.ProcessStream(messageId, "default", transaction.Connection!, transaction, action, Cancellation);

    public Task ProcessStream(string name, Func<AttachmentStream, Task> action) =>
        persister.ProcessStream(messageId, name, transaction.Connection!, transaction, action, Cancellation);

    public Task ProcessStreams(Func<AttachmentStream, Task> action) =>
        persister.ProcessStreams(messageId, transaction.Connection!, transaction, action, Cancellation);

    public Task<AttachmentBytes> GetBytes() =>
        persister.GetBytes(messageId, "default", transaction.Connection!, transaction, Cancellation);

    public Task<MemoryStream> GetMemoryStream() =>
        persister.GetMemoryStream(messageId, "default", transaction.Connection!, transaction, Cancellation);

    public Task<AttachmentBytes> GetBytes(string name) =>
        persister.GetBytes(messageId, name, transaction.Connection!, transaction, Cancellation);

    public Task<MemoryStream> GetMemoryStream(string name) =>
        persister.GetMemoryStream(messageId, name, transaction.Connection!, transaction, Cancellation);

    public Task<AttachmentString> GetString(Encoding? encoding) =>
        persister.GetString(messageId, "default", transaction.Connection!, transaction, encoding, Cancellation);

    public Task<AttachmentString> GetString(string name, Encoding? encoding) =>
        persister.GetString(messageId, name, transaction.Connection!, transaction, encoding, Cancellation);

    public Task<AttachmentStream> GetStream() =>
        persister.GetStream(messageId, "default", transaction.Connection!, transaction, false, Cancellation);

    public Task<AttachmentStream> GetStream(string name) =>
        persister.GetStream(messageId, name, transaction.Connection!, transaction, false, Cancellation);

    public Task CopyToForMessage(string messageId, Stream target) =>
        persister.CopyTo(messageId, "default", transaction.Connection!, transaction, target, Cancellation);

    public Task CopyToForMessage(string messageId, string name, Stream target) =>
        persister.CopyTo(messageId, name, transaction.Connection!, transaction, target, Cancellation);

    public Task ProcessStreamForMessage(string messageId, Func<AttachmentStream, Task> action) =>
        persister.ProcessStream(messageId, "default", transaction.Connection!, transaction, action, Cancellation);

    public Task ProcessStreamForMessage(string messageId, string name, Func<AttachmentStream, Task> action) =>
        persister.ProcessStream(messageId, name, transaction.Connection!, transaction, action, Cancellation);

    public Task ProcessStreamsForMessage(string messageId, Func<AttachmentStream, Task> action) =>
        persister.ProcessStreams(messageId, transaction.Connection!, transaction, action, Cancellation);

    public Task<AttachmentBytes> GetBytesForMessage(string messageId) =>
        persister.GetBytes(messageId, "default", transaction.Connection!, transaction, Cancellation);

    public Task<MemoryStream> GetMemoryStreamForMessage(string messageId) =>
        persister.GetMemoryStream(messageId, "default", transaction.Connection!, transaction, Cancellation);

    public Task<AttachmentBytes> GetBytesForMessage(string messageId, string name) =>
        persister.GetBytes(messageId, name, transaction.Connection!, transaction, Cancellation);

    public Task<MemoryStream> GetMemoryStreamForMessage(string messageId, string name) =>
        persister.GetMemoryStream(messageId, name, transaction.Connection!, transaction, Cancellation);

    public Task<AttachmentString> GetStringForMessage(string messageId, Encoding? encoding) =>
        persister.GetString(messageId, "default", transaction.Connection!, transaction, encoding, Cancellation);

    public Task<AttachmentString> GetStringForMessage(string messageId, string name, Encoding? encoding) =>
        persister.GetString(messageId, name, transaction.Connection!, transaction, encoding, Cancellation);

    public Task<AttachmentStream> GetStreamForMessage(string messageId) =>
        persister.GetStream(messageId, "default", transaction.Connection!, transaction, false, Cancellation);

    public Task<AttachmentStream> GetStreamForMessage(string messageId, string name) =>
        persister.GetStream(messageId, name, transaction.Connection!, transaction, false, Cancellation);

    public IAsyncEnumerable<AttachmentInfo> GetMetadata() =>
        persister.ReadAllMessageInfo(transaction.Connection!, transaction, messageId, Cancellation);
}