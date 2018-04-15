using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NServiceBus.Attachments.FileShare
{
    public partial class Persister
    {

        /// <summary>
        /// Saves <paramref name="stream"/> as an attachment.
        /// </summary>
        /// <exception cref="TaskCanceledException">If <paramref name="cancellation"/> is <see cref="CancellationToken.IsCancellationRequested"/>.</exception>
        public virtual Task SaveStream(string messageId, string name, DateTime expiry, Stream stream, CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstNull(stream, nameof(stream));
            return Save(messageId, name, expiry, fileStream => stream.CopyToAsync(fileStream, 4096, cancellation));
        }

        async Task Save(string messageId, string name, DateTime expiry, Func<Stream, Task> action)
        {
            if (name == null)
            {
                name = "default";
            }

            var attachmentDirectory = GetAttachmentDirectory(messageId, name);
            ThrowIfDirectoryExists(attachmentDirectory, messageId, name);

            Directory.CreateDirectory(attachmentDirectory);
            var dataFile = Path.Combine(attachmentDirectory, "data");
            expiry = expiry.ToUniversalTime();
            var expiryFile = Path.Combine(attachmentDirectory, $"{expiry:yyyy-MM-ddTHHmm}.expiry");
            using (File.Create(expiryFile))
            {
            }

            using (var fileStream = FileHelpers.OpenWrite(dataFile))
            {
                await action(fileStream).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Saves <paramref name="bytes"/> as an attachment.
        /// </summary>
        /// <exception cref="TaskCanceledException">If <paramref name="cancellation"/> is <see cref="CancellationToken.IsCancellationRequested"/>.</exception>
        public virtual Task SaveBytes(string messageId, string name, DateTime expiry, byte[] bytes, CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstNull(bytes, nameof(bytes));
            return Save(messageId, name, expiry, fileStream => fileStream.WriteAsync(bytes, 0, bytes.Length, cancellation));
        }
    }
}