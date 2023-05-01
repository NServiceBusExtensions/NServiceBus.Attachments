﻿namespace NServiceBus.Attachments
#if FileShare
    .FileShare
#elif Sql
    .Sql
#endif
;

/// <summary>
/// Provides access to write attachments.
/// </summary>
public interface IOutgoingAttachments
{
    /// <summary>
    /// Returns <code>true</code> if there are pending attachments to be written in the current outgoing pipeline.
    /// </summary>
    bool HasPendingAttachments { get; }

    IReadOnlyList<OutgoingAttachment> Items { get; }

    /// <summary>
    /// Add an attachment with <paramref name="name"/> to the current outgoing pipeline.
    /// </summary>
    void Add<T>(string name, Func<Task<T>> streamFactory, GetTimeToKeep? timeToKeep = null, Action? cleanup = null, IReadOnlyDictionary<string, string>? metadata = null)
        where T : Stream;

    /// <summary>
    /// Add an attachment with <paramref name="name"/> to the current outgoing pipeline.
    /// </summary>
    void Add(AttachmentToAdd attachment, GetTimeToKeep? timeToKeep = null, Action? cleanup = null);

    /// <summary>
    /// Add an attachment with <paramref name="name"/> to the current outgoing pipeline.
    /// </summary>
    void Add(string name, Func<Stream> streamFactory, GetTimeToKeep? timeToKeep = null, Action? cleanup = null, IReadOnlyDictionary<string, string>? metadata = null);

    /// <summary>
    /// Add an attachment with <paramref name="name"/> to the current outgoing pipeline.
    /// </summary>
    void Add(string name, Stream stream, GetTimeToKeep? timeToKeep = null, Action? cleanup = null, IReadOnlyDictionary<string, string>? metadata = null);

    /// <summary>
    /// Add an attachment with the default name of `default` to the current outgoing pipeline.
    /// </summary>
    void Add<T>(Func<Task<T>> streamFactory, GetTimeToKeep? timeToKeep = null, Action? cleanup = null, IReadOnlyDictionary<string, string>? metadata = null)
        where T : Stream;

    /// <summary>
    /// Add an attachment with the default name of `default` to the current outgoing pipeline.
    /// </summary>
    void Add(Func<Stream> streamFactory, GetTimeToKeep? timeToKeep = null, Action? cleanup = null, IReadOnlyDictionary<string, string>? metadata = null);

    /// <summary>
    /// Add an attachment with the default name of `default` to the current outgoing pipeline.
    /// </summary>
    void Add(Stream stream, GetTimeToKeep? timeToKeep = null, Action? cleanup = null, IReadOnlyDictionary<string, string>? metadata = null);

    /// <summary>
    /// Add an attachment with <paramref name="name"/> to the current outgoing pipeline.
    /// </summary>
    /// <remarks>
    /// This should only be used when the data size is know to be small as it causes the full size of the attachment to be allocated in memory.
    /// </remarks>
    void AddBytes(string name, Func<byte[]> byteFactory, GetTimeToKeep? timeToKeep = null, Action? cleanup = null, IReadOnlyDictionary<string, string>? metadata = null);

    /// <summary>
    /// Add an attachment with <paramref name="name"/> to the current outgoing pipeline.
    /// </summary>
    /// <remarks>
    /// This should only be used when the data size is know to be small as it causes the full size of the attachment to be allocated in memory.
    /// </remarks>
    void AddBytes(string name, byte[] bytes, GetTimeToKeep? timeToKeep = null, Action? cleanup = null, IReadOnlyDictionary<string, string>? metadata = null);

    /// <summary>
    /// Add an attachment with <paramref name="name"/> to the current outgoing pipeline.
    /// </summary>
    /// <remarks>
    /// This should only be used when the data size is know to be small as it causes the full size of the attachment to be allocated in memory.
    /// </remarks>
    void AddString(string name, string value, Encoding? encoding = null, GetTimeToKeep? timeToKeep = null, Action? cleanup = null, IReadOnlyDictionary<string, string>? metadata = null);

    /// <summary>
    /// Add an attachment with the default name of `default` to the current outgoing pipeline.
    /// </summary>
    /// <remarks>
    /// This should only be used when the data size is know to be small as it causes the full size of the attachment to be allocated in memory.
    /// </remarks>
    void AddString(string value, Encoding? encoding = null, GetTimeToKeep? timeToKeep = null, Action? cleanup = null, IReadOnlyDictionary<string, string>? metadata = null);

    /// <summary>
    /// Add an attachment with the default name of `default` to the current outgoing pipeline.
    /// </summary>
    /// <remarks>
    /// This should only be used when the data size is know to be small as it causes the full size of the attachment to be allocated in memory.
    /// </remarks>
    void AddBytes(Func<Task<byte[]>> bytesFactory, GetTimeToKeep? timeToKeep = null, Action? cleanup = null, IReadOnlyDictionary<string, string>? metadata = null);

    /// <summary>
    /// Add an attachment with <paramref name="name"/> to the current outgoing pipeline.
    /// </summary>
    /// <remarks>
    /// This should only be used when the data size is know to be small as it causes the full size of the attachment to be allocated in memory.
    /// </remarks>
    void AddBytes(string name, Func<Task<byte[]>> bytesFactory, GetTimeToKeep? timeToKeep = null, Action? cleanup = null, IReadOnlyDictionary<string, string>? metadata = null);

    /// <summary>
    /// Add an attachment with the default name of `default` to the current outgoing pipeline.
    /// </summary>
    /// <remarks>
    /// This should only be used when the data size is know to be small as it causes the full size of the attachment to be allocated in memory.
    /// </remarks>
    void AddBytes(Func<byte[]> byteFactory, GetTimeToKeep? timeToKeep = null, Action? cleanup = null, IReadOnlyDictionary<string, string>? metadata = null);

    /// <summary>
    /// Save an attachment with the default name of `default`.
    /// </summary>
    /// <remarks>
    /// This should only be used when the data size is know to be small as it causes the full size of the attachment to be allocated in memory.
    /// </remarks>
    void AddBytes(byte[] bytes, GetTimeToKeep? timeToKeep = null, Action? cleanup = null, IReadOnlyDictionary<string, string>? metadata = null);

    /// <summary>
    /// Duplicates the incoming attachments to the current outgoing pipeline.
    /// </summary>
    void DuplicateIncoming();

    /// <summary>
    /// Duplicates the incoming attachments to the current outgoing pipeline.
    /// </summary>
    void DuplicateIncoming(string fromName, string toName);
}