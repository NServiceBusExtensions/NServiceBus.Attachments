namespace NServiceBus.Attachments.FileShare
#if Raw
    .Raw
#endif
    ;

public partial class Persister
{
    /// <inheritdoc />
    public virtual Task SaveStream(string messageId, string name, DateTime expiry, Stream stream, IReadOnlyDictionary<string, string>? metadata = null, Cancellation cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId);
        Guard.AgainstNullOrEmpty(name);
        stream.MoveToStart();
        return Save(messageId, name, expiry, metadata, fileStream => stream.CopyToAsync(fileStream, 4096, cancellation), cancellation);
    }

    /// <inheritdoc />
    public virtual Task SaveBytes(string messageId, string name, DateTime expiry, byte[] bytes, IReadOnlyDictionary<string, string>? metadata = null, Cancellation cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId);
        Guard.AgainstNullOrEmpty(name);
        return Save(messageId, name, expiry, metadata, fileStream => fileStream.WriteAsync(bytes, 0, bytes.Length, cancellation), cancellation);
    }

    /// <inheritdoc />
    public virtual Task SaveString(string messageId, string name, DateTime expiry, string value, Encoding? encoding = null, IReadOnlyDictionary<string, string>? metadata = null, Cancellation cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId);
        Guard.AgainstNullOrEmpty(name);
        encoding = encoding.Default();
        var dictionary = MetadataSerializer.AppendEncoding(encoding, metadata);
        return Save(
            messageId,
            name,
            expiry,
            dictionary,
            async fileStream =>
            {
                await using var writer = fileStream.BuildLeaveOpenWriter(encoding);
                await writer.WriteAsync(value);
            },
            cancellation);
    }

    async Task Save(
        string messageId,
        string? name,
        DateTime expiry,
        IReadOnlyDictionary<string, string>? metadata,
        Func<FileStream, Task> action,
        Cancellation cancellation = default)
    {
        name ??= "default";

        var attachmentDirectory = GetAttachmentDirectory(messageId, name);
        ThrowIfDirectoryExists(attachmentDirectory, messageId, name);

        Directory.CreateDirectory(attachmentDirectory);
        var dataFile = Path.Combine(attachmentDirectory, "data");
        expiry = expiry.ToUniversalTime();
        var expiryFile = Path.Combine(attachmentDirectory, $"{expiry:yyyy-MM-ddTHHmm}.expiry");
        await using (File.Create(expiryFile))
        {
        }

        await WriteMetadata(attachmentDirectory, metadata, cancellation);

        await using var fileStream = FileHelpers.OpenWrite(dataFile);
        await action(fileStream);
    }
}