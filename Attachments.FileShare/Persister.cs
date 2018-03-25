using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NServiceBus.Attachments.FileShare
{
    /// <summary>
    /// Raw access to manipulating attachments outside of the context of the NServiceBus pipeline.
    /// </summary>
  public class Persister
    {
        string fileShare;

        /// <summary>
        /// Instatiate a new instance of <see cref="Persister"/>.
        /// </summary>
        public Persister(string fileShare)
        {
            Guard.AgainstNullOrEmpty(fileShare,nameof(fileShare));
            this.fileShare = fileShare;
        }

        /// <summary>
        /// Saves <paramref name="stream"/> as an attachment.
        /// </summary>
        /// <exception cref="TaskCanceledException">If <paramref name="cancellation"/> is <see cref="CancellationToken.IsCancellationRequested"/>.</exception>
        public Task SaveStream(string messageId, string name, DateTime expiry, Stream stream, CancellationToken cancellation = default)
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

        string GetAttachmentDirectory(string messageId, string name)
        {
            var messageDirectory = GetMessageDirectory(messageId);
            return Path.Combine(messageDirectory, name);
        }

        string GetMessageDirectory(string messageId)
        {
            return Path.Combine(fileShare, messageId);
        }

        /// <summary>
        /// Saves <paramref name="bytes"/> as an attachment.
        /// </summary>
        /// <exception cref="TaskCanceledException">If <paramref name="cancellation"/> is <see cref="CancellationToken.IsCancellationRequested"/>.</exception>
        public Task SaveBytes(string messageId, string name, DateTime expiry, byte[] bytes, CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstNull(bytes, nameof(bytes));
            return Save(messageId, name, expiry, fileStream => fileStream.WriteAsync(bytes, 0, bytes.Length, cancellation));
        }

        /// <summary>
        /// Reads the <see cref="AttachmentMetadata"/> for all attachments.
        /// </summary>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public IEnumerable<AttachmentMetadata> ReadAllMetadata(CancellationToken cancellation = default)
        {
            foreach (var messageDirectory in Directory.EnumerateDirectories(fileShare))
            {
                var messageId = Path.GetFileName(messageDirectory);
                foreach (var attachmentDirectory in Directory.EnumerateDirectories(messageDirectory))
                {
                    cancellation.ThrowIfCancellationRequested();
                    var expiryFile = Directory.EnumerateFiles(attachmentDirectory, "*.expiry").Single();
                    yield return new AttachmentMetadata(
                        messageId: messageId,
                        name: Path.GetFileName(attachmentDirectory),
                        expiry: ParseExpiry(Path.GetFileNameWithoutExtension(expiryFile)));
                }
            }
        }

        DateTime ParseExpiry(string value)
        {
            return DateTime.ParseExact(value, dateTimeFormat, null, DateTimeStyles.AdjustToUniversal);
        }

        string dateTimeFormat = "yyyy-MM-ddTHHmm";

        /// <summary>
        /// Deletes all attachments.
        /// </summary>
        public void DeleteAllAttachments()
        {
            FileHelpers.PurgeDirectory(fileShare);
        }

        /// <summary>
        /// Deletes attachments older than <paramref name="dateTime"/>.
        /// </summary>
        public void CleanupItemsOlderThan(DateTime dateTime, CancellationToken cancellation = default)
        {
            foreach (var expiryFile in Directory.EnumerateFiles(fileShare, "*.expiry", SearchOption.AllDirectories))
            {
                if (cancellation.IsCancellationRequested)
                {
                    return;
                }

                var expiry = ParseExpiry(Path.GetFileNameWithoutExtension(expiryFile));
                if (expiry > dateTime)
                {
                    Directory.GetParent(expiryFile).Delete(true);
                }
            }
        }

        /// <summary>
        /// Copies an attachment to <paramref name="target"/>.
        /// </summary>
        public async Task CopyTo(string messageId, string name, Stream target, CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstNull(target, nameof(target));
            var dataFile = GetDataFile(messageId, name);
            ThrowIfFileNotFound(dataFile, messageId, name);
            await FileHelpers.CopyTo(target, cancellation, dataFile);
        }

         Stream OpenAttachmentStream(string messageId, string name)
        {
            var dataFile = GetDataFile(messageId, name);
            ThrowIfFileNotFound(dataFile, messageId, name);
            return FileHelpers.OpenRead(dataFile);
        }

        string GetDataFile(string messageId, string name)
        {
            var attachmentDirectory = GetAttachmentDirectory(messageId, name);
            return Path.Combine(attachmentDirectory, "data");
        }

        /// <summary>
        /// Reads a byte array for an attachment.
        /// </summary>
        public Task<byte[]> GetBytes(string messageId, string name, CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            Guard.AgainstNullOrEmpty(name, nameof(name));
            var dataFile = GetDataFile(messageId, name);
            ThrowIfFileNotFound(dataFile, messageId, name);
            return FileHelpers.ReadBytes(cancellation, dataFile);
        }

        /// <summary>
        /// Returns an open stream pointing to an attachment.
        /// </summary>
        public Stream GetStream(string messageId, string name)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            Guard.AgainstNullOrEmpty(name, nameof(name));
            return OpenAttachmentStream(messageId, name);
        }

        /// <summary>
        /// Processes all attachments for <paramref name="messageId"/> by passing them to <paramref name="action"/>.
        /// </summary>
        public async Task ProcessStreams(string messageId, Func<string, Stream, Task> action, CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            Guard.AgainstNull(action, nameof(action));
            var messageDirectory = GetMessageDirectory(messageId);
            ThrowIfDirectoryNotFound(messageDirectory, messageId);
            foreach (var dataFile in Directory.EnumerateFiles(messageDirectory, "data", SearchOption.AllDirectories))
            {
                cancellation.ThrowIfCancellationRequested();
                var attachmentName = Directory.GetParent(dataFile).Name;
                using (var fileStream = FileHelpers.OpenRead(dataFile))
                {
                    await action(attachmentName, fileStream).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Processes an attachment by passing it to <paramref name="action"/>.
        /// </summary>
        public async Task ProcessStream(string messageId, string name, Func<Stream, Task> action, CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstNull(action, nameof(action));
            var messageDirectory = GetAttachmentDirectory(messageId, name);
            ThrowIfDirectoryNotFound(messageDirectory, messageId);
            foreach (var dataFile in Directory.EnumerateFiles(messageDirectory, "data", SearchOption.AllDirectories))
            {
                cancellation.ThrowIfCancellationRequested();
                using (var fileStream = FileHelpers.OpenRead(dataFile))
                {
                    await action(fileStream).ConfigureAwait(false);
                }
            }
        }

        static void ThrowIfDirectoryNotFound(string path, string messageId)
        {
            if (Directory.Exists(path))
            {
                return;
            }

            throw new Exception($"Could not find attachment. MessageId:{messageId}, Path:{path}");
        }

        static void ThrowIfFileNotFound(string path, string messageId, string name)
        {
            if (File.Exists(path))
            {
                return;
            }

            throw new Exception($"Could not find attachment. MessageId:{messageId}, Name:{name}, Path:{path}");
        }

        static void ThrowIfDirectoryExists(string path, string messageId, string name)
        {
            if (!Directory.Exists(path))
            {
                return;
            }

            throw new Exception($"Attachment already exists. MessageId:{messageId}, Name:{name}, Path:{path}");
        }
    }
}