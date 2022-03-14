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

    public Task CopyTo(Stream target, CancellationToken cancellation = default) =>
        persister.CopyTo(messageId, "default", connection, null, target, cancellation);

    public Task CopyTo(string name, Stream target, CancellationToken cancellation = default) =>
        persister.CopyTo(messageId, name, connection, null, target, cancellation);

    public Task ProcessStream(Func<AttachmentStream, Task> action, CancellationToken cancellation = default) =>
        persister.ProcessStream(messageId, "default", connection, null, action, cancellation);

    public Task ProcessStream(string name, Func<AttachmentStream, Task> action, CancellationToken cancellation = default) =>
        persister.ProcessStream(messageId, name, connection, null, action, cancellation);

    public Task ProcessStreams(Func<AttachmentStream, Task> action, CancellationToken cancellation = default) =>
        persister.ProcessStreams(messageId, connection, null, action, cancellation);

    public Task<AttachmentBytes> GetBytes(CancellationToken cancellation = default) =>
        persister.GetBytes(messageId, "default", connection, null, cancellation);

    public Task<AttachmentBytes> GetBytes(string name, CancellationToken cancellation = default) =>
        persister.GetBytes(messageId, name, connection, null, cancellation);

    public Task<AttachmentString> GetString(Encoding? encoding, CancellationToken cancellation = default) =>
        persister.GetString(messageId, "default", connection, null, encoding, cancellation);

    public Task<AttachmentString> GetString(string name, Encoding? encoding, CancellationToken cancellation = default) =>
        persister.GetString(messageId, name, connection, null, encoding, cancellation);

    public Task<AttachmentStream> GetStream(CancellationToken cancellation = default) =>
        persister.GetStream(messageId, "default", connection, null, false, cancellation);

    public Task<AttachmentStream> GetStream(string name, CancellationToken cancellation = default) =>
        persister.GetStream(messageId, name, connection, null, false, cancellation);

    public Task CopyToForMessage(string messageId, Stream target, CancellationToken cancellation = default) =>
        persister.CopyTo(messageId, "default", connection, null, target, cancellation);

    public Task CopyToForMessage(string messageId, string name, Stream target, CancellationToken cancellation = default) =>
        persister.CopyTo(messageId, name, connection, null, target, cancellation);

    public Task ProcessStreamForMessage(string messageId, Func<AttachmentStream, Task> action, CancellationToken cancellation = default) =>
        persister.ProcessStream(messageId, "default", connection, null, action, cancellation);

    public Task ProcessStreamForMessage(string messageId, string name, Func<AttachmentStream, Task> action, CancellationToken cancellation = default) =>
        persister.ProcessStream(messageId, name, connection, null, action, cancellation);

    public Task ProcessStreamsForMessage(string messageId, Func<AttachmentStream, Task> action, CancellationToken cancellation = default) =>
        persister.ProcessStreams(messageId, connection, null, action, cancellation);

    public Task<AttachmentBytes> GetBytesForMessage(string messageId, CancellationToken cancellation = default) =>
        persister.GetBytes(messageId, "default", connection, null, cancellation);

    public Task<AttachmentBytes> GetBytesForMessage(string messageId, string name, CancellationToken cancellation = default) =>
        persister.GetBytes(messageId, name, connection, null, cancellation);

    public Task<AttachmentString> GetStringForMessage(string messageId, Encoding? encoding, CancellationToken cancellation = default) =>
        persister.GetString(messageId, "default", connection, null, encoding, cancellation);

    public Task<AttachmentString> GetStringForMessage(string messageId, string name, Encoding? encoding, CancellationToken cancellation = default) =>
        persister.GetString(messageId, name, connection, null, encoding, cancellation);

    public Task<AttachmentStream> GetStreamForMessage(string messageId, CancellationToken cancellation = default) =>
        persister.GetStream(messageId, "default", connection, null, false, cancellation);

    public Task<AttachmentStream> GetStreamForMessage(string messageId, string name, CancellationToken cancellation = default) =>
        persister.GetStream(messageId, name, connection, null, false, cancellation);

    public IAsyncEnumerable<AttachmentInfo> GetMetadata(CancellationToken cancellation = default) =>
        persister.ReadAllMessageInfo(connection, null, messageId, cancellation);
}