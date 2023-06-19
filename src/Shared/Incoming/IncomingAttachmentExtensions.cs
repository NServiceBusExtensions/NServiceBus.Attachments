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
/// Extensions for <see cref="IMessageAttachments"/>.
/// </summary>
public static class IncomingAttachmentExtensions
{
    /// <summary>
    /// Copies all attachments for the current message to <paramref name="directory"/>.
    /// </summary>
    public static Task CopyToDirectory(
        this IMessageAttachments attachments,
        string directory,
        string? nameForDefault = default,
        Cancel cancel = default)
    {
        Guard.AgainstNullOrEmpty(directory);
        Guard.AgainstEmpty(nameForDefault);
        Directory.CreateDirectory(directory);

        return attachments.ProcessStreams(
            async (stream, cancel) =>
            {
                var name = stream.Name;
                if (name == "default" && nameForDefault is not null)
                {
                    name = nameForDefault;
                }

                name = name.TrimStart('\\', '/');
                var file = Path.Combine(directory, name);
                var fileDirectory = Path.GetDirectoryName(file)!;
                Directory.CreateDirectory(fileDirectory);
                File.Delete(file);
                using var fileStream = File.Create(file);
                await stream.CopyToAsync(fileStream, 4096, cancel);
            },
            cancel);
    }
}