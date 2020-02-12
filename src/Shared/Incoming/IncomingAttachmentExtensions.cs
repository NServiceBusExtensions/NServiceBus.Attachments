using System.IO;
using System.Threading;
using System.Threading.Tasks;

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
{
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
            CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(directory, nameof(directory));
            Guard.AgainstEmpty(nameForDefault, nameof(nameForDefault));
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return attachments.ProcessStreams(
                async stream =>
                {
                    var name = stream.Name;
                    if (name == "default" && nameForDefault != null)
                    {
                        name = nameForDefault;
                    }

                    using var fileStream = File.Create(Path.Combine(directory, name));
                    await stream.CopyToAsync(fileStream, 4096, cancellation);
                },
                cancellation);
        }
    }
}