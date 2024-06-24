// ReSharper disable ConvertClosureToMethodGroup
namespace NServiceBus.Attachments.FileShare
#if Raw
    .Raw
#endif
    ;

public partial class Persister
{
    /// <inheritdoc />
    public virtual async IAsyncEnumerable<AttachmentInfo> ReadAllInfo(
        [EnumeratorCancellation] Cancel cancel = default)
    {
        foreach (var messageDirectory in Directory.EnumerateDirectories(fileShare))
        {
            var messageId = Path.GetFileName(messageDirectory);
            await foreach (var info in ReadMessageInfo(messageDirectory, messageId, cancel))
            {
                if (cancel.IsCancellationRequested)
                {
                    yield break;
                }

                yield return info;
            }
        }
    }

    /// <inheritdoc />
    public virtual IEnumerable<string> ReadAllMessageNames(string messageId)
    {
        var messageDirectory = GetMessageDirectory(messageId);
        return Directory.EnumerateDirectories(messageDirectory)
            .Select(_ => Path.GetFileName(_));
    }

    /// <inheritdoc />
    public virtual IAsyncEnumerable<AttachmentInfo> ReadAllMessageInfo(
        string messageId,
        Cancel cancel = default)
    {
        Guard.AgainstNullOrEmpty(messageId);
        var messageDirectory = GetMessageDirectory(messageId);
        return ReadMessageInfo(messageDirectory, messageId, cancel);
    }

    async IAsyncEnumerable<AttachmentInfo> ReadMessageInfo(
        string messageDirectory,
        string messageId,
        [EnumeratorCancellation] Cancel cancel = default)
    {
        foreach (var attachmentDirectory in Directory.EnumerateDirectories(messageDirectory))
        {
            if (cancel.IsCancellationRequested)
            {
                yield break;
            }

            var expiryFile = Directory.EnumerateFiles(attachmentDirectory, "*.expiry").Single();
            var metadata = await ReadMetadata(attachmentDirectory, cancel);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(expiryFile);
            yield return new(
                messageId: messageId,
                name: Path.GetFileName(attachmentDirectory),
                created: Directory.GetCreationTimeUtc(attachmentDirectory),
                expiry: ParseExpiry(fileNameWithoutExtension),
                metadata: metadata);
        }
    }
}