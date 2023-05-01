namespace NServiceBus.Attachments.FileShare
#if Raw
    .Raw
#endif
    ;

public partial class Persister
{
    /// <inheritdoc />
    public virtual async Task ProcessStreams(string messageId, Func<AttachmentStream, Task> action, Cancellation cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId);
        var messageDirectory = GetMessageDirectory(messageId);
        ThrowIfDirectoryNotFound(messageDirectory, messageId);
        foreach (var attachmentDirectory in Directory.EnumerateDirectories(messageDirectory))
        {
            cancellation.ThrowIfCancellationRequested();
            var dataFile = GetDataFile(attachmentDirectory);
            var attachmentName = Directory.GetParent(dataFile)!.Name;
            var read = FileHelpers.OpenRead(dataFile);
            var metadata = await ReadMetadata(attachmentDirectory, cancellation);
            await using var attachment = new AttachmentStream(attachmentName, read, read.Length, metadata);
            await action(attachment);
        }
    }

    /// <inheritdoc />
    public virtual async Task ProcessStream(string messageId, string name, Func<AttachmentStream, Task> action, Cancellation cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId);
        Guard.AgainstNullOrEmpty(name);
        var attachmentDirectory = GetAttachmentDirectory(messageId, name);
        ThrowIfDirectoryNotFound(attachmentDirectory, messageId);
        cancellation.ThrowIfCancellationRequested();
        var dataFile = GetDataFile(attachmentDirectory);
        ThrowIfFileNotFound(dataFile, messageId, name);
        var read = FileHelpers.OpenRead(dataFile);
        var metadata = await ReadMetadata(attachmentDirectory, cancellation);
        await using var attachment = new AttachmentStream(name, read, read.Length, metadata);
        await action(attachment);
    }
}