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

    public Task CopyTo(Stream target, Cancel cancel = default) =>
        persister.CopyTo(messageId, "default", transaction.Connection!, transaction, target, cancel);

    public Task CopyTo(string name, Stream target, Cancel cancel = default) =>
        persister.CopyTo(messageId, name, transaction.Connection!, transaction, target, cancel);

    public Task ProcessStream(Func<AttachmentStream, Cancel, Task> action, Cancel cancel = default) =>
        persister.ProcessStream(messageId, "default", transaction.Connection!, transaction, action, cancel);

    public Task ProcessByteArray(Func<AttachmentBytes, Cancel, Task> action, Cancel cancel = default) =>
        persister.ProcessByteArray(messageId, "default", transaction.Connection!, transaction, action, cancel);

    public Task ProcessStream(string name, Func<AttachmentStream, Cancel, Task> action, Cancel cancel = default) =>
        persister.ProcessStream(messageId, name, transaction.Connection!, transaction, action, cancel);

    public Task ProcessByteArray(string name, Func<AttachmentBytes, Cancel, Task> action, Cancel cancel = default) =>
        persister.ProcessByteArray(messageId, name, transaction.Connection!, transaction, action, cancel);

    public Task ProcessStreams(Func<AttachmentStream, Cancel, Task> action, Cancel cancel = default) =>
        persister.ProcessStreams(messageId, transaction.Connection!, transaction, action, cancel);

    public Task ProcessByteArrays(Func<AttachmentBytes, Cancel, Task> action, Cancel cancel = default) =>
        persister.ProcessByteArrays(messageId, transaction.Connection!, transaction, action, cancel);

    public Task<AttachmentBytes> GetBytes(Cancel cancel = default) =>
        persister.GetBytes(messageId, "default", transaction.Connection!, transaction, cancel);

    public Task<MemoryStream> GetMemoryStream(Cancel cancel = default) =>
        persister.GetMemoryStream(messageId, "default", transaction.Connection!, transaction, cancel);

    public Task<AttachmentBytes> GetBytes(string name, Cancel cancel = default) =>
        persister.GetBytes(messageId, name, transaction.Connection!, transaction, cancel);

    public Task<MemoryStream> GetMemoryStream(string name, Cancel cancel = default) =>
        persister.GetMemoryStream(messageId, name, transaction.Connection!, transaction, cancel);

    public Task<AttachmentString> GetString(Encoding? encoding, Cancel cancel = default) =>
        persister.GetString(messageId, "default", transaction.Connection!, transaction, encoding, cancel);

    public Task<AttachmentString> GetString(string name, Encoding? encoding, Cancel cancel = default) =>
        persister.GetString(messageId, name, transaction.Connection!, transaction, encoding, cancel);

    public Task<AttachmentStream> GetStream(Cancel cancel = default) =>
        persister.GetStream(messageId, "default", transaction.Connection!, transaction, false, cancel);

    public Task<AttachmentStream> GetStream(string name, Cancel cancel = default) =>
        persister.GetStream(messageId, name, transaction.Connection!, transaction, false, cancel);

    public Task CopyToForMessage(string messageId, Stream target, Cancel cancel = default) =>
        persister.CopyTo(messageId, "default", transaction.Connection!, transaction, target, cancel);

    public Task CopyToForMessage(string messageId, string name, Stream target, Cancel cancel = default) =>
        persister.CopyTo(messageId, name, transaction.Connection!, transaction, target, cancel);

    public Task ProcessStreamForMessage(string messageId, Func<AttachmentStream, Cancel, Task> action, Cancel cancel = default) =>
        persister.ProcessStream(messageId, "default", transaction.Connection!, transaction, action, cancel);

    public Task ProcessByteArrayForMessage(string messageId, Func<AttachmentBytes, Cancel, Task> action, Cancel cancel = default) =>
        persister.ProcessByteArray(messageId, "default", transaction.Connection!, transaction, action, cancel);

    public Task ProcessStreamForMessage(string messageId, string name, Func<AttachmentStream, Cancel, Task> action, Cancel cancel = default) =>
        persister.ProcessStream(messageId, name, transaction.Connection!, transaction, action, cancel);

    public Task ProcessByteArrayForMessage(string messageId, string name, Func<AttachmentBytes, Cancel, Task> action, Cancel cancel = default) =>
        persister.ProcessByteArray(messageId, name, transaction.Connection!, transaction, action, cancel);

    public Task ProcessStreamsForMessage(string messageId, Func<AttachmentStream, Cancel, Task> action, Cancel cancel = default) =>
        persister.ProcessStreams(messageId, transaction.Connection!, transaction, action, cancel);

    public Task ProcessByteArraysForMessage(string messageId, Func<AttachmentBytes, Cancel, Task> action, Cancel cancel = default) =>
        persister.ProcessByteArrays(messageId, transaction.Connection!, transaction, action, cancel);

    public Task<AttachmentBytes> GetBytesForMessage(string messageId, Cancel cancel = default) =>
        persister.GetBytes(messageId, "default", transaction.Connection!, transaction, cancel);

    public Task<MemoryStream> GetMemoryStreamForMessage(string messageId, Cancel cancel = default) =>
        persister.GetMemoryStream(messageId, "default", transaction.Connection!, transaction, cancel);

    public Task<AttachmentBytes> GetBytesForMessage(string messageId, string name, Cancel cancel = default) =>
        persister.GetBytes(messageId, name, transaction.Connection!, transaction, cancel);

    public Task<MemoryStream> GetMemoryStreamForMessage(string messageId, string name, Cancel cancel = default) =>
        persister.GetMemoryStream(messageId, name, transaction.Connection!, transaction, cancel);

    public Task<AttachmentString> GetStringForMessage(string messageId, Encoding? encoding, Cancel cancel = default) =>
        persister.GetString(messageId, "default", transaction.Connection!, transaction, encoding, cancel);

    public Task<AttachmentString> GetStringForMessage(string messageId, string name, Encoding? encoding, Cancel cancel = default) =>
        persister.GetString(messageId, name, transaction.Connection!, transaction, encoding, cancel);

    public Task<AttachmentStream> GetStreamForMessage(string messageId, Cancel cancel = default) =>
        persister.GetStream(messageId, "default", transaction.Connection!, transaction, false, cancel);

    public Task<AttachmentStream> GetStreamForMessage(string messageId, string name, Cancel cancel = default) =>
        persister.GetStream(messageId, name, transaction.Connection!, transaction, false, cancel);

    public IAsyncEnumerable<AttachmentInfo> GetMetadata(Cancel cancel = default) =>
        persister.ReadAllMessageInfo(transaction.Connection!, transaction, messageId, cancel);
}