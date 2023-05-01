namespace NServiceBus.Attachments
#if FileShare
.FileShare
#endif
#if Sql
.Sql
#endif
#if Raw
.Raw
#endif
;

/// <summary>
/// Extensions for <see cref="IOutgoingAttachments"/>.
/// </summary>
public static class OutgoingAttachmentsExtensions
{
    /// <summary>
    /// Add a file to the <paramref name="attachments"/>.
    /// </summary>
    public static void AddFile(
        this IOutgoingAttachments attachments,
        string file,
        string? name = default)
    {
        Guard.FileExists(file);
        Guard.AgainstEmpty(name);

        if (name is null)
        {
            attachments.Add(() => File.OpenRead(file));
            return;
        }
        attachments.Add(name, () => File.OpenRead(file));
    }
}