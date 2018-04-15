using System;
using System.Globalization;
using System.IO;

namespace NServiceBus.Attachments.FileShare
{
    /// <summary>
    /// Raw access to manipulating attachments outside of the context of the NServiceBus pipeline.
    /// </summary>
    public partial class Persister : IPersister
    {
        string fileShare;

        /// <summary>
        /// Instantiate a new instance of <see cref="Persister"/>.
        /// </summary>
        public Persister(string fileShare)
        {
            Guard.AgainstNullOrEmpty(fileShare, nameof(fileShare));
            this.fileShare = fileShare;
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

        DateTime ParseExpiry(string value)
        {
            return DateTime.ParseExact(value, dateTimeFormat, null, DateTimeStyles.AdjustToUniversal);
        }

        string dateTimeFormat = "yyyy-MM-ddTHHmm";

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