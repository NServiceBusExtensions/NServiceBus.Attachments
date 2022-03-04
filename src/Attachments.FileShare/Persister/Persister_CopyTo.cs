namespace NServiceBus.Attachments.FileShare
#if Raw
    .Raw
#endif
    ;

public partial class Persister
{
    /// <inheritdoc />
    public virtual Task CopyTo(string messageId, string name, Stream target, CancellationToken cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
        Guard.AgainstNullOrEmpty(name, nameof(name));
        var attachmentDirectory = GetAttachmentDirectory(messageId, name);
        var dataFile = GetDataFile(attachmentDirectory);
        ThrowIfFileNotFound(dataFile, messageId, name);
        return FileHelpers.CopyTo(target, cancellation, dataFile);
    }
}