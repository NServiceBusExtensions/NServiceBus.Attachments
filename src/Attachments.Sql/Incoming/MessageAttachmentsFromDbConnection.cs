using Microsoft.Data.SqlClient;
using NServiceBus.Attachments.Sql;

class MessageAttachmentsFromSqlConnection :
    IMessageAttachments
{
    SqlConnection connection;
    string messageId;
    IPersister persister;

    public MessageAttachmentsFromSqlConnection(SqlConnection connection, string messageId, IPersister persister, CancellationToken cancellation)
    {
        this.connection = connection;
        this.messageId = messageId;
        this.persister = persister;
        this.Cancellation = cancellation;
    }

    public CancellationToken Cancellation { get; set; }

    public Task CopyTo(Stream target) =>
        persister.CopyTo(messageId, "default", connection, null, target, Cancellation);

    public Task CopyTo(string name, Stream target) =>
        persister.CopyTo(messageId, name, connection, null, target, Cancellation);

    public Task ProcessStream(Func<AttachmentStream, Task> action) =>
        persister.ProcessStream(messageId, "default", connection, null, action, Cancellation);

    public Task ProcessStream(string name, Func<AttachmentStream, Task> action) =>
        persister.ProcessStream(messageId, name, connection, null, action, Cancellation);

    public Task ProcessStreams(Func<AttachmentStream, Task> action) =>
        persister.ProcessStreams(messageId, connection, null, action, Cancellation);

    public Task<AttachmentBytes> GetBytes() =>
        persister.GetBytes(messageId, "default", connection, null, Cancellation);

    public Task<MemoryStream> GetMemoryStream() =>
        persister.GetMemoryStream(messageId, "default", connection, null, Cancellation);

    public Task<AttachmentBytes> GetBytes(string name) =>
        persister.GetBytes(messageId, name, connection, null, Cancellation);

    public Task<MemoryStream> GetMemoryStream(string name) =>
        persister.GetMemoryStream(messageId, name, connection, null, Cancellation);

    public Task<AttachmentString> GetString(Encoding? encoding) =>
        persister.GetString(messageId, "default", connection, null, encoding, Cancellation);

    public Task<AttachmentString> GetString(string name, Encoding? encoding) =>
        persister.GetString(messageId, name, connection, null, encoding, Cancellation);

    public Task<AttachmentStream> GetStream() =>
        persister.GetStream(messageId, "default", connection, null, false, Cancellation);

    public Task<AttachmentStream> GetStream(string name) =>
        persister.GetStream(messageId, name, connection, null, false, Cancellation);

    public Task CopyToForMessage(string messageId, Stream target) =>
        persister.CopyTo(messageId, "default", connection, null, target, Cancellation);

    public Task CopyToForMessage(string messageId, string name, Stream target) =>
        persister.CopyTo(messageId, name, connection, null, target, Cancellation);

    public Task ProcessStreamForMessage(string messageId, Func<AttachmentStream, Task> action) =>
        persister.ProcessStream(messageId, "default", connection, null, action, Cancellation);

    public Task ProcessStreamForMessage(string messageId, string name, Func<AttachmentStream, Task> action) =>
        persister.ProcessStream(messageId, name, connection, null, action, Cancellation);

    public Task ProcessStreamsForMessage(string messageId, Func<AttachmentStream, Task> action) =>
        persister.ProcessStreams(messageId, connection, null, action, Cancellation);

    public Task<AttachmentBytes> GetBytesForMessage(string messageId) =>
        persister.GetBytes(messageId, "default", connection, null, Cancellation);

    public Task<MemoryStream> GetMemoryStreamForMessage(string messageId) =>
        persister.GetMemoryStream(messageId, "default", connection, null, Cancellation);

    public Task<AttachmentBytes> GetBytesForMessage(string messageId, string name) =>
        persister.GetBytes(messageId, name, connection, null, Cancellation);

    public Task<MemoryStream> GetMemoryStreamForMessage(string messageId, string name) =>
        persister.GetMemoryStream(messageId, name, connection, null, Cancellation);

    public Task<AttachmentString> GetStringForMessage(string messageId, Encoding? encoding) =>
        persister.GetString(messageId, "default", connection, null, encoding, Cancellation);

    public Task<AttachmentString> GetStringForMessage(string messageId, string name, Encoding? encoding) =>
        persister.GetString(messageId, name, connection, null, encoding, Cancellation);

    public Task<AttachmentStream> GetStreamForMessage(string messageId) =>
        persister.GetStream(messageId, "default", connection, null, false, Cancellation);

    public Task<AttachmentStream> GetStreamForMessage(string messageId, string name) =>
        persister.GetStream(messageId, name, connection, null, false, Cancellation);

    public IAsyncEnumerable<AttachmentInfo> GetMetadata() =>
        persister.ReadAllMessageInfo(connection, null, messageId, Cancellation);
}