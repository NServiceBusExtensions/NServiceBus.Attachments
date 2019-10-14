#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System;
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
        /// An empty <see cref="AttachmentBytes"/> that contains a "default" name a <see cref="Array.Empty{T}"/> of <see cref="byte"/>s as contents.
        /// </summary>
        public static AttachmentBytes Empty = new AttachmentBytes(string.Empty, Array.Empty<byte>());

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
        /// <param name="name">The name of the attachment.</param>
        /// <param name="bytes">The <see cref="byte"/>s to wrap.</param>
        /// <param name="metadata">The attachment metadata.</param>
        public AttachmentBytes(string name, byte[] bytes, IReadOnlyDictionary<string, string>? metadata = null)
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
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