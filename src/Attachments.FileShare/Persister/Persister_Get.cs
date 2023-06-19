namespace NServiceBus.Attachments.FileShare
#if Raw
    .Raw
#endif
    ;

public partial class Persister
{
    /// <inheritdoc />
    public virtual async Task<AttachmentBytes> GetBytes(string messageId, string name, Cancel cancel = default)
    {
        Guard.AgainstNullOrEmpty(messageId);
        Guard.AgainstNullOrEmpty(name);
        var attachmentDirectory = GetAttachmentDirectory(messageId, name);
        var dataFile = GetDataFile(attachmentDirectory);
        ThrowIfFileNotFound(dataFile, messageId, name);
        var bytes = await FileHelpers.ReadBytes(cancel, dataFile);
        var metadata = await ReadMetadata(attachmentDirectory, cancel);
        return new(name, bytes, metadata);
    }

    /// <inheritdoc />
    public virtual async Task<MemoryStream> GetMemoryStream(string messageId, string name, Cancel cancel = default)
    {
        Guard.AgainstNullOrEmpty(messageId);
        Guard.AgainstNullOrEmpty(name);
        var attachmentDirectory = GetAttachmentDirectory(messageId, name);
        var dataFile = GetDataFile(attachmentDirectory);
        ThrowIfFileNotFound(dataFile, messageId, name);
        var bytes = await FileHelpers.ReadBytes(cancel, dataFile);
        return new(bytes);
    }

    /// <inheritdoc />
    public virtual async Task<AttachmentString> GetString(string messageId, string name, Encoding? encoding, Cancel cancel = default)
    {
        Guard.AgainstNullOrEmpty(messageId);
        Guard.AgainstNullOrEmpty(name);
        var attachmentDirectory = GetAttachmentDirectory(messageId, name);
        var dataFile = GetDataFile(attachmentDirectory);
        ThrowIfFileNotFound(dataFile, messageId, name);
        var metadata = await ReadMetadata(attachmentDirectory, cancel);
        encoding = MetadataSerializer.GetEncoding(encoding, metadata);
        var allText = await File.ReadAllTextAsync(dataFile, encoding, cancel);
        return new(name, allText, metadata);
    }

    /// <inheritdoc />
    public virtual Task<AttachmentStream> GetStream(string messageId, string name, Cancel cancel = default)
    {
        Guard.AgainstNullOrEmpty(messageId);
        Guard.AgainstNullOrEmpty(name);
        return OpenAttachmentStream(messageId, name, cancel);
    }

    /// <inheritdoc />
    public virtual async IAsyncEnumerable<AttachmentBytes> GetBytes(string messageId, [EnumeratorCancellation] Cancel cancel = default)
    {
        Guard.AgainstNullOrEmpty(messageId);
        var messageDirectory = GetMessageDirectory(messageId);
        ThrowIfDirectoryNotFound(messageDirectory, messageId);
        foreach (var attachmentDirectory in Directory.EnumerateDirectories(messageDirectory))
        {
            cancel.ThrowIfCancellationRequested();
            var dataFile = GetDataFile(attachmentDirectory);
            var attachmentName = Directory.GetParent(dataFile)!.Name;
            var bytes = await FileHelpers.ReadBytes(cancel, dataFile);
            var metadata = await ReadMetadata(attachmentDirectory, cancel);
            yield return new(attachmentName, bytes, metadata);
        }
    }

    /// <inheritdoc />
    public virtual async IAsyncEnumerable<AttachmentString> GetStrings(string messageId, Encoding? encoding = null, [EnumeratorCancellation] Cancel cancel = default)
    {
        Guard.AgainstNullOrEmpty(messageId);
        var messageDirectory = GetMessageDirectory(messageId);
        ThrowIfDirectoryNotFound(messageDirectory, messageId);
        encoding = encoding.Default();
        foreach (var attachmentDirectory in Directory.EnumerateDirectories(messageDirectory))
        {
            cancel.ThrowIfCancellationRequested();
            var dataFile = GetDataFile(attachmentDirectory);
            var attachmentName = Directory.GetParent(dataFile)!.Name;
            var metadata = await ReadMetadata(attachmentDirectory, cancel);
            var allText = await File.ReadAllTextAsync(dataFile, encoding, cancel);
            yield return new(attachmentName, allText, metadata);
        }
    }

    /// <inheritdoc />
    public virtual async IAsyncEnumerable<AttachmentStream> GetStreams(string messageId, [EnumeratorCancellation] Cancel cancel = default)
    {
        Guard.AgainstNullOrEmpty(messageId);
        var messageDirectory = GetMessageDirectory(messageId);
        ThrowIfDirectoryNotFound(messageDirectory, messageId);
        foreach (var attachmentDirectory in Directory.EnumerateDirectories(messageDirectory))
        {
            cancel.ThrowIfCancellationRequested();
            var dataFile = GetDataFile(attachmentDirectory);
            var attachmentName = Directory.GetParent(dataFile)!.Name;
            await using var read = FileHelpers.OpenRead(dataFile);
            var metadata = await ReadMetadata(attachmentDirectory, cancel);
            yield return new(attachmentName, read, read.Length, metadata);
        }
    }

    async Task<AttachmentStream> OpenAttachmentStream(
        string messageId,
        string name,
        Cancel cancel = default)
    {
        var attachmentDirectory = GetAttachmentDirectory(messageId, name);
        var dataFile = GetDataFile(attachmentDirectory);
        ThrowIfFileNotFound(dataFile, messageId, name);
        var metadata = await ReadMetadata(attachmentDirectory, cancel);
        var read = FileHelpers.OpenRead(dataFile);
        return new(name, read, read.Length, metadata);
    }
}