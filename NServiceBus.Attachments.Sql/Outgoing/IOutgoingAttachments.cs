using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace NServiceBus.Attachments
{
    /// <summary>
    /// Provides access to write attachments.
    /// </summary>
    public interface IOutgoingAttachments
    {
        /// <summary>
        /// Returns <code>true</code> if there are pending attachments to be written in the current outgoing pipeline.
        /// </summary>
        bool HasPendingAttachments { get; }

        /// <summary>
        /// All attachment names for the current outgoing pipeline.
        /// </summary>
        IReadOnlyList<string> StreamNames { get; }

        /// <summary>
        /// Add an attachment with <paramref name="name"/> to the current outgoing pipeline.
        /// </summary>
        void Add<T>(string name, Func<Task<T>> streamFactory, GetTimeToKeep timeToKeep = null, Action cleanup = null) where T : Stream;

        /// <summary>
        /// Add an attachment with <paramref name="name"/> to the current outgoing pipeline.
        /// </summary>
        void Add(string name, Func<Stream> streamFactory, GetTimeToKeep timeToKeep = null, Action cleanup = null);

        /// <summary>
        /// Add an attachment with <paramref name="name"/> to the current outgoing pipeline.
        /// </summary>
        void Add(string name, Stream stream, GetTimeToKeep timeToKeep = null, Action cleanup = null);

        /// <summary>
        /// Add an attachment with the default name of <see cref="string.Empty"/> to the current outgoing pipeline.
        /// </summary>
        void Add<T>(Func<Task<T>> streamFactory, GetTimeToKeep timeToKeep = null, Action cleanup = null) where T : Stream;

        /// <summary>
        /// Add an attachment with the default name of <see cref="string.Empty"/> to the current outgoing pipeline.
        /// </summary>
        void Add(Func<Stream> streamFactory, GetTimeToKeep timeToKeep = null, Action cleanup = null);

        /// <summary>
        /// Add an attachment with the default name of <see cref="string.Empty"/> to the current outgoing pipeline.
        /// </summary>
        void Add(Stream stream, GetTimeToKeep timeToKeep = null, Action cleanup = null);
    }
}