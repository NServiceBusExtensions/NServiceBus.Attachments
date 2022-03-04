namespace NServiceBus.Attachments.FileShare
#if Raw
    .Raw
#endif
    ;

/// <summary>
/// Raw access to manipulating attachments outside of the context of the NServiceBus pipeline.
/// </summary>
public interface IPersister
{
    /// <summary>
    /// Saves <paramref name="stream" /> as an attachment.
    /// </summary>
    /// <exception cref="TaskCanceledException">If <paramref name="cancellation" /> is <see cref="CancellationToken.IsCancellationRequested" />.</exception>
    Task SaveStream(string messageId, string name, DateTime expiry, Stream stream, IReadOnlyDictionary<string, string>? metadata, CancellationToken cancellation = default);

    /// <summary>
    /// Saves <paramref name="bytes" /> as an attachment.
    /// </summary>
    /// <exception cref="TaskCanceledException">If <paramref name="cancellation" /> is <see cref="CancellationToken.IsCancellationRequested" />.</exception>
    Task SaveBytes(string messageId, string name, DateTime expiry, byte[] bytes, IReadOnlyDictionary<string, string>? metadata, CancellationToken cancellation = default);

    /// <summary>
    /// Saves <paramref name="value" /> as an attachment.
    /// </summary>
    /// <exception cref="TaskCanceledException">If <paramref name="cancellation" /> is <see cref="CancellationToken.IsCancellationRequested" />.</exception>
    Task SaveString(string messageId, string name, DateTime expiry, string value, Encoding? encoding, IReadOnlyDictionary<string, string>? metadata, CancellationToken cancellation = default);

    /// <summary>
    /// Reads the <see cref="AttachmentInfo" /> for all attachments.
    /// </summary>
    IAsyncEnumerable<AttachmentInfo> ReadAllInfo(CancellationToken cancellation = default);

    /// <summary>
    /// Reads the names for all attachments of a specific message.
    /// </summary>
    IEnumerable<string> ReadAllMessageNames(string messageId);

    /// <summary>
    /// Reads the <see cref="AttachmentInfo" /> for all attachments of a specific message.
    /// </summary>
    IAsyncEnumerable<AttachmentInfo> ReadAllMessageInfo(string messageId, CancellationToken cancellation = default);

    /// <summary>
    /// Deletes all attachments.
    /// </summary>
    void DeleteAllAttachments();

    /// <summary>
    /// Deletes attachments older than <paramref name="dateTime" />.
    /// </summary>
    void CleanupItemsOlderThan(DateTime dateTime, CancellationToken cancellation = default);

    /// <summary>
    /// Deletes all items.
    /// </summary>
    void PurgeItems(CancellationToken cancellation = default);

    /// <summary>
    /// Copies an attachment to <paramref name="target" />.
    /// </summary>
    Task CopyTo(string messageId, string name, Stream target, CancellationToken cancellation = default);

    /// <summary>
    /// Copies an attachment to a different message.
    /// </summary>
    Task Duplicate(string sourceMessageId, string name, string targetMessageId, CancellationToken cancellation = default);

    /// <summary>
    /// Copies an attachment to a different message.
    /// </summary>
    Task Duplicate(string sourceMessageId, string name, string targetMessageId, string targetName, CancellationToken cancellation = default);

    /// <summary>
    /// Copies attachments to a different message.
    /// </summary>
    Task<IReadOnlyCollection<string>> Duplicate(string sourceMessageId, string targetMessageId, CancellationToken cancellation = default);

    /// <summary>
    /// Reads an <see cref="AttachmentBytes" /> for an attachment.
    /// </summary>
    Task<AttachmentBytes> GetBytes(string messageId, string name, CancellationToken cancellation = default);

    /// <summary>
    /// Reads an <see cref="AttachmentString" /> for an attachment.
    /// </summary>
    Task<AttachmentString> GetString(string messageId, string name, Encoding? encoding, CancellationToken cancellation = default);

    /// <summary>
    /// Reads an <see cref="AttachmentStream" /> an attachment.
    /// </summary>
    Task<AttachmentStream> GetStream(string messageId, string name, CancellationToken cancellation = default);

    /// <summary>
    /// Processes all attachments for <paramref name="messageId" /> by passing them to <paramref name="action" />.
    /// </summary>
    Task ProcessStreams(string messageId, Func<AttachmentStream, Task> action, CancellationToken cancellation = default);

    /// <summary>
    /// Processes an attachment by passing it to <paramref name="action" />.
    /// </summary>
    Task ProcessStream(string messageId, string name, Func<AttachmentStream, Task> action, CancellationToken cancellation = default);

    /// <summary>
    /// Reads all <see cref="AttachmentBytes" />s for an attachment.
    /// </summary>
    IAsyncEnumerable<AttachmentBytes> GetBytes(string messageId, CancellationToken cancellation = default);

    /// <summary>
    /// Reads all <see cref="AttachmentString" />s for an attachment.
    /// </summary>
    IAsyncEnumerable<AttachmentString> GetStrings(string messageId, Encoding? encoding, CancellationToken cancellation = default);

    /// <summary>
    /// Reads all <see cref="AttachmentStream" />s to an attachment.
    /// </summary>
    IAsyncEnumerable<AttachmentStream> GetStreams(string messageId, CancellationToken cancellation = default);
}