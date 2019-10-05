#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System.Collections.Generic;

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
    /// Wraps a <see cref="byte"/> array to provide extra information when reading.
    /// </summary>
    public class AttachmentBytes
    {
        /// <summary>
        /// The attachment bytes.
        /// </summary>
        public readonly byte[] Bytes;

        /// <summary>
        /// The attachment metadata.
        /// </summary>
        public readonly IReadOnlyDictionary<string, string>? Metadata;

        /// <summary>
        /// Initialises a new instance of <see cref="AttachmentStream"/>.
        /// </summary>
        /// <param name="bytes">The <see cref="byte"/>s to wrap.</param>
        /// <param name="metadata">The attachment metadata.</param>
        public AttachmentBytes(byte[] bytes, IReadOnlyDictionary<string, string>? metadata = null)
        {
            Guard.AgainstNull(bytes, nameof(bytes));
            if (metadata == null)
            {
                metadata = MetadataSerializer.EmptyMetadata;
            }
            Bytes = bytes;
            Metadata = metadata;
        }

        public static implicit operator byte[](AttachmentBytes d)
        {
            return d.Bytes;
        }
    }
}