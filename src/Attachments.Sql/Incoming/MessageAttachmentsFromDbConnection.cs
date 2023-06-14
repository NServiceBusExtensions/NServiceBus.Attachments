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

    public Task CopyTo(Stream target, Cancellation cancellation = default) =>
        persister.CopyTo(messageId, "default", connection, null, target, cancellation);

    public Task CopyTo(string name, Stream target, Cancellation cancellation = default) =>
        persister.CopyTo(messageId, name, connection, null, target, cancellation);

    public Task ProcessStream(Func<AttachmentStream, Cancellation, Task> action, Cancellation cancellation = default) =>
        persister.ProcessStream(messageId, "default", connection, null, action, cancellation);

    public Task ProcessByteArray(Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancellation = default) =>
        persister.ProcessByteArray(messageId, "default", connection, null, action, cancellation);

    public Task ProcessStream(string name, Func<AttachmentStream, Cancellation, Task> action, Cancellation cancellation = default) =>
        persister.ProcessStream(messageId, name, connection, null, action, cancellation);

    public Task ProcessByteArray(string name, Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancellation = default) =>
        persister.ProcessByteArray(messageId, name, connection, null, action, cancellation);

    public Task ProcessStreams(Func<AttachmentStream, Cancellation, Task> action, Cancellation cancellation = default) =>
        persister.ProcessStreams(messageId, connection, null, action, cancellation);

    public Task ProcessByteArrays(Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancellation = default) =>
        persister.ProcessByteArrays(messageId, connection, null, action, cancellation);

    public Task<AttachmentBytes> GetBytes(Cancellation cancellation = default) =>
        persister.GetBytes(messageId, "default", connection, null, cancellation);

    public Task<MemoryStream> GetMemoryStream(Cancellation cancellation = default) =>
        persister.GetMemoryStream(messageId, "default", connection, null, cancellation);

    public Task<AttachmentBytes> GetBytes(string name, Cancellation cancellation = default) =>
        persister.GetBytes(messageId, name, connection, null, cancellation);

    public Task<MemoryStream> GetMemoryStream(string name, Cancellation cancellation = default) =>
        persister.GetMemoryStream(messageId, name, connection, null, cancellation);

    public Task<AttachmentString> GetString(Encoding? encoding, Cancellation cancellation = default) =>
        persister.GetString(messageId, "default", connection, null, encoding, cancellation);

    public Task<AttachmentString> GetString(string name, Encoding? encoding, Cancellation cancellation = default) =>
        persister.GetString(messageId, name, connection, null, encoding, cancellation);

    public Task<AttachmentStream> GetStream(Cancellation cancellation = default) =>
        persister.GetStream(messageId, "default", connection, null, false, cancellation);

    public Task<AttachmentStream> GetStream(string name, Cancellation cancellation = default) =>
        persister.GetStream(messageId, name, connection, null, false, cancellation);

    public Task CopyToForMessage(string messageId, Stream target, Cancellation cancellation = default) =>
        persister.CopyTo(messageId, "default", connection, null, target, cancellation);

    public Task CopyToForMessage(string messageId, string name, Stream target, Cancellation cancellation = default) =>
        persister.CopyTo(messageId, name, connection, null, target, cancellation);

    public Task ProcessStreamForMessage(string messageId, Func<AttachmentStream, Cancellation, Task> action, Cancellation cancellation = default) =>
        persister.ProcessStream(messageId, "default", connection, null, action, cancellation);

    public Task ProcessByteArrayForMessage(string messageId, Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancellation = default) =>
        persister.ProcessByteArray(messageId, "default", connection, null, action, cancellation);

    public Task ProcessStreamForMessage(string messageId, string name, Func<AttachmentStream, Cancellation, Task> action, Cancellation cancellation = default) =>
        persister.ProcessStream(messageId, name, connection, null, action, cancellation);

    public Task ProcessByteArrayForMessage(string messageId, string name, Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancellation = default) =>
        persister.ProcessByteArray(messageId, name, connection, null, action, cancellation);

    public Task ProcessStreamsForMessage(string messageId, Func<AttachmentStream, Cancellation, Task> action, Cancellation cancellation = default) =>
        persister.ProcessStreams(messageId, connection, null, action, cancellation);

    public Task ProcessByteArraysForMessage(string messageId, Func<AttachmentBytes, Cancellation, Task> action, Cancellation cancellation = default) =>
        persister.ProcessByteArrays(messageId, connection, null, action, cancellation);

    public Task<AttachmentBytes> GetBytesForMessage(string messageId, Cancellation cancellation = default) =>
        persister.GetBytes(messageId, "default", connection, null, cancellation);

    public Task<MemoryStream> GetMemoryStreamForMessage(string messageId, Cancellation cancellation = default) =>
        persister.GetMemoryStream(messageId, "default", connection, null, cancellation);

    public Task<AttachmentBytes> GetBytesForMessage(string messageId, string name, Cancellation cancellation = default) =>
        persister.GetBytes(messageId, name, connection, null, cancellation);

    public Task<MemoryStream> GetMemoryStreamForMessage(string messageId, string name, Cancellation cancellation = default) =>
        persister.GetMemoryStream(messageId, name, connection, null, cancellation);

    public Task<AttachmentString> GetStringForMessage(string messageId, Encoding? encoding, Cancellation cancellation = default) =>
        persister.GetString(messageId, "default", connection, null, encoding, cancellation);

    public Task<AttachmentString> GetStringForMessage(string messageId, string name, Encoding? encoding, Cancellation cancellation = default) =>
        persister.GetString(messageId, name, connection, null, encoding, cancellation);

    public Task<AttachmentStream> GetStreamForMessage(string messageId, Cancellation cancellation = default) =>
        persister.GetStream(messageId, "default", connection, null, false, cancellation);

    public Task<AttachmentStream> GetStreamForMessage(string messageId, string name, Cancellation cancellation = default) =>
        persister.GetStream(messageId, name, connection, null, false, cancellation);

    public IAsyncEnumerable<AttachmentInfo> GetMetadata(Cancellation cancellation = default) =>
        persister.ReadAllMessageInfo(connection, null, messageId, cancellation);
}