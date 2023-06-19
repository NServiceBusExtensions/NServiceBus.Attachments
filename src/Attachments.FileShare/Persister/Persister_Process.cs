namespace NServiceBus.Attachments.FileShare
#if Raw
    .Raw
#endif
    ;

public partial class Persister
{
    /// <inheritdoc />
    public virtual async Task ProcessStreams(string messageId, Func<AttachmentStream, Cancel, Task> action, Cancel cancel = default)
    {
        Guard.AgainstNullOrEmpty(messageId);
        var messageDirectory = GetMessageDirectory(messageId);
        ThrowIfDirectoryNotFound(messageDirectory, messageId);
        foreach (var attachmentDirectory in Directory.EnumerateDirectories(messageDirectory))
        {
            cancel.ThrowIfCancellationRequested();
            var dataFile = GetDataFile(attachmentDirectory);
            var attachmentName = Directory.GetParent(dataFile)!.Name;
            var read = FileHelpers.OpenRead(dataFile);
            var metadata = await ReadMetadata(attachmentDirectory, cancel);
            await using var attachment = new AttachmentStream(attachmentName, read, read.Length, metadata);
            await action(attachment, cancel);
        }
    }

    /// <inheritdoc />
    public virtual async Task ProcessStream(string messageId, string name, Func<AttachmentStream, Cancel, Task> action, Cancel cancel = default)
    {
        Guard.AgainstNullOrEmpty(messageId);
        Guard.AgainstNullOrEmpty(name);
        var attachmentDirectory = GetAttachmentDirectory(messageId, name);
        ThrowIfDirectoryNotFound(attachmentDirectory, messageId);
        cancel.ThrowIfCancellationRequested();
        var dataFile = GetDataFile(attachmentDirectory);
        ThrowIfFileNotFound(dataFile, messageId, name);
        var read = FileHelpers.OpenRead(dataFile);
        var metadata = await ReadMetadata(attachmentDirectory, cancel);
        await using var attachment = new AttachmentStream(name, read, read.Length, metadata);
        await action(attachment, cancel);
    }
}