using Microsoft.Data.SqlClient;
using NServiceBus.Attachments.Sql;

class MessageAttachmentsFromSqlConnection :
    IMessageAttachments
{
    SqlConnection connection;
    string messageId;
    IPersister persister;

    public MessageAttachmentsFromSqlConnection(SqlConnection connection, string messageId, IPersister persister)
    {
        this.connection = connection;
        this.messageId = messageId;
        this.persister = persister;
    }

    public Task CopyTo(Stream target, Cancellation cancel = default) =>
        persister.CopyTo(messageId, "default", connection, null, target, cancel);

    public Task CopyTo(string name, Stream target, Cancellation cancel = default) =>
        persister.CopyTo(messageId, name, connection, null, target, cancel);

    public Task ProcessStream(Func<AttachmentStream, Cancellation, Task> action, Cancellation cancel = default) =>
        persister.ProcessStream(messageId, "default", connection, null, action, cancel);

    public Task ProcessByteArray(Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancel = default) =>
        persister.ProcessByteArray(messageId, "default", connection, null, action, cancel);

    public Task ProcessStream(string name, Func<AttachmentStream, Cancellation, Task> action, Cancellation cancel = default) =>
        persister.ProcessStream(messageId, name, connection, null, action, cancel);

    public Task ProcessByteArray(string name, Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancel = default) =>
        persister.ProcessByteArray(messageId, name, connection, null, action, cancel);

    public Task ProcessStreams(Func<AttachmentStream, Cancellation, Task> action, Cancellation cancel = default) =>
        persister.ProcessStreams(messageId, connection, null, action, cancel);

    public Task ProcessByteArrays(Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancel = default) =>
        persister.ProcessByteArrays(messageId, connection, null, action, cancel);

    public Task<AttachmentBytes> GetBytes(Cancellation cancel = default) =>
        persister.GetBytes(messageId, "default", connection, null, cancel);

    public Task<MemoryStream> GetMemoryStream(Cancellation cancel = default) =>
        persister.GetMemoryStream(messageId, "default", connection, null, cancel);

    public Task<AttachmentBytes> GetBytes(string name, Cancellation cancel = default) =>
        persister.GetBytes(messageId, name, connection, null, cancel);

    public Task<MemoryStream> GetMemoryStream(string name, Cancellation cancel = default) =>
        persister.GetMemoryStream(messageId, name, connection, null, cancel);

    public Task<AttachmentString> GetString(Encoding? encoding, Cancellation cancel = default) =>
        persister.GetString(messageId, "default", connection, null, encoding, cancel);

    public Task<AttachmentString> GetString(string name, Encoding? encoding, Cancellation cancel = default) =>
        persister.GetString(messageId, name, connection, null, encoding, cancel);

    public Task<AttachmentStream> GetStream(Cancellation cancel = default) =>
        persister.GetStream(messageId, "default", connection, null, false, cancel);

    public Task<AttachmentStream> GetStream(string name, Cancellation cancel = default) =>
        persister.GetStream(messageId, name, connection, null, false, cancel);

    public Task CopyToForMessage(string messageId, Stream target, Cancellation cancel = default) =>
        persister.CopyTo(messageId, "default", connection, null, target, cancel);

    public Task CopyToForMessage(string messageId, string name, Stream target, Cancellation cancel = default) =>
        persister.CopyTo(messageId, name, connection, null, target, cancel);

    public Task ProcessStreamForMessage(string messageId, Func<AttachmentStream, Cancellation, Task> action, Cancellation cancel = default) =>
        persister.ProcessStream(messageId, "default", connection, null, action, cancel);

    public Task ProcessByteArrayForMessage(string messageId, Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancel = default) =>
        persister.ProcessByteArray(messageId, "default", connection, null, action, cancel);

    public Task ProcessStreamForMessage(string messageId, string name, Func<AttachmentStream, Cancellation, Task> action, Cancellation cancel = default) =>
        persister.ProcessStream(messageId, name, connection, null, action, cancel);

    public Task ProcessByteArrayForMessage(string messageId, string name, Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancel = default) =>
        persister.ProcessByteArray(messageId, name, connection, null, action, cancel);

    public Task ProcessStreamsForMessage(string messageId, Func<AttachmentStream, Cancellation, Task> action, Cancellation cancel = default) =>
        persister.ProcessStreams(messageId, connection, null, action, cancel);

    public Task ProcessByteArraysForMessage(string messageId, Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancel = default) =>
        persister.ProcessByteArrays(messageId, connection, null, action, cancel);

    public Task<AttachmentBytes> GetBytesForMessage(string messageId, Cancellation cancel = default) =>
        persister.GetBytes(messageId, "default", connection, null, cancel);

    public Task<MemoryStream> GetMemoryStreamForMessage(string messageId, Cancellation cancel = default) =>
        persister.GetMemoryStream(messageId, "default", connection, null, cancel);

    public Task<AttachmentBytes> GetBytesForMessage(string messageId, string name, Cancellation cancel = default) =>
        persister.GetBytes(messageId, name, connection, null, cancel);

    public Task<MemoryStream> GetMemoryStreamForMessage(string messageId, string name, Cancellation cancel = default) =>
        persister.GetMemoryStream(messageId, name, connection, null, cancel);

    public Task<AttachmentString> GetStringForMessage(string messageId, Encoding? encoding, Cancellation cancel = default) =>
        persister.GetString(messageId, "default", connection, null, encoding, cancel);

    public Task<AttachmentString> GetStringForMessage(string messageId, string name, Encoding? encoding, Cancellation cancel = default) =>
        persister.GetString(messageId, name, connection, null, encoding, cancel);

    public Task<AttachmentStream> GetStreamForMessage(string messageId, Cancellation cancel = default) =>
        persister.GetStream(messageId, "default", connection, null, false, cancel);

    public Task<AttachmentStream> GetStreamForMessage(string messageId, string name, Cancellation cancel = default) =>
        persister.GetStream(messageId, name, connection, null, false, cancel);

    public IAsyncEnumerable<AttachmentInfo> GetMetadata(Cancellation cancel = default) =>
        persister.ReadAllMessageInfo(connection, null, messageId, cancel);
}