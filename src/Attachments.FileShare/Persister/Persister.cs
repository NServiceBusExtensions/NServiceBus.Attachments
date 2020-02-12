using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace NServiceBus.Attachments.FileShare
#if Raw
    .Raw
#endif
{
    /// <summary>
    /// Raw access to manipulating attachments outside of the context of the NServiceBus pipeline.
    /// </summary>
    public partial class Persister :
        IPersister
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

        static string GetDataFile(string attachmentDirectory)
        {
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

        static async Task<IReadOnlyDictionary<string, string>> ReadMetadata(string attachmentDirectory)
        {
            var metadataFile = GetMetadataFile(attachmentDirectory);
            if (!File.Exists(metadataFile))
            {
                return MetadataSerializer.EmptyMetadata;
            }

            using var stream = FileHelpers.OpenRead(metadataFile);
            return await MetadataSerializer.Deserialize(stream);
        }

        static string GetMetadataFile(string attachmentDirectory)
        {
            return Path.Combine(attachmentDirectory, "metadata.json");
        }

        static void WriteMetadata(string attachmentDirectory, IReadOnlyDictionary<string, string>? metadata)
        {
            if (metadata == null)
            {
                return;
            }

            var metadataFile = GetMetadataFile(attachmentDirectory);
            using var stream = FileHelpers.OpenWrite(metadataFile);
            MetadataSerializer.Serialize(stream, metadata);
        }
    }
}