﻿#if FileShare
using NServiceBus.Attachments.FileShare;
#elif Sql
using NServiceBus.Attachments.Sql;
#else
using NServiceBus.Attachments;
#endif

class OutgoingAttachments :
    IOutgoingAttachments
{
    internal Dictionary<string, Outgoing> Inner = new(StringComparer.OrdinalIgnoreCase);
    public List<Duplicate> Duplicates = new();

    public bool HasPendingAttachments => Inner.Any() ||
                                         DuplicateIncomingAttachments ||
                                         Duplicates.Any();

    public bool DuplicateIncomingAttachments;

    public IReadOnlyList<string> Names => Inner.Keys.ToList();

    public void Add<T>(Func<Task<T>> streamFactory, GetTimeToKeep? timeToKeep = null, Action? cleanup = null, IReadOnlyDictionary<string, string>? metadata = null)
        where T : Stream =>
        Add("default", streamFactory, timeToKeep, cleanup, metadata);

    public void Add<T>(string name, Func<Task<T>> streamFactory, GetTimeToKeep? timeToKeep = null, Action? cleanup = null, IReadOnlyDictionary<string, string>? metadata = null)
        where T : Stream =>
        Inner.Add(
            name,
            new()
            {
                Metadata = metadata,
                TimeToKeep = timeToKeep,
                Cleanup = cleanup.WrapCleanupInCheck(name),
                AsyncStreamFactory = streamFactory.WrapStreamFuncTaskInCheck(name)
            });

    public void Add(Func<Stream> streamFactory, GetTimeToKeep? timeToKeep = null, Action? cleanup = null, IReadOnlyDictionary<string, string>? metadata = null) =>
        Add("default", streamFactory, timeToKeep, cleanup, metadata);

    public void Add(Stream stream, GetTimeToKeep? timeToKeep = null, Action? cleanup = null, IReadOnlyDictionary<string, string>? metadata = null) =>
        Add("default", stream, timeToKeep, cleanup, metadata);

    public void Add(string name, Func<Stream> streamFactory, GetTimeToKeep? timeToKeep = null, Action? cleanup = null, IReadOnlyDictionary<string, string>? metadata = null) =>
        Inner.Add(
            name,
            new()
            {
                Metadata = metadata,
                TimeToKeep = timeToKeep,
                Cleanup = cleanup.WrapCleanupInCheck(name),
                StreamFactory = streamFactory.WrapFuncInCheck(name),
            });

    public void Add(string name, Stream stream, GetTimeToKeep? timeToKeep = null, Action? cleanup = null, IReadOnlyDictionary<string, string>? metadata = null) =>
        Inner.Add(
            name,
            new()
            {
                Metadata = metadata,
                TimeToKeep = timeToKeep,
                Cleanup = cleanup.WrapCleanupInCheck(name),
                StreamInstance = stream
            });

    public void AddBytes(Func<byte[]> bytesFactory, GetTimeToKeep? timeToKeep = null, Action? cleanup = null, IReadOnlyDictionary<string, string>? metadata = null) =>
        AddBytes("default", bytesFactory, timeToKeep, cleanup, metadata);

    public void AddBytes(byte[] bytes, GetTimeToKeep? timeToKeep = null, Action? cleanup = null, IReadOnlyDictionary<string, string>? metadata = null) =>
        AddBytes("default", bytes, timeToKeep, cleanup, metadata);

    public void DuplicateIncoming() =>
        DuplicateIncomingAttachments = true;

    public void DuplicateIncoming(string incomingName, string? outgoingName = null) =>
        Duplicates.Add(new(from: incomingName, to: outgoingName));

    public void AddBytes(string name, Func<byte[]> bytesFactory, GetTimeToKeep? timeToKeep = null, Action? cleanup = null, IReadOnlyDictionary<string, string>? metadata = null) =>
        Inner.Add(
            name,
            new()
            {
                Metadata = metadata,
                TimeToKeep = timeToKeep,
                Cleanup = cleanup.WrapCleanupInCheck(name),
                BytesFactory = bytesFactory.WrapFuncInCheck(name)
            });

    public void AddBytes(string name, byte[] bytes, GetTimeToKeep? timeToKeep = null, Action? cleanup = null, IReadOnlyDictionary<string, string>? metadata = null) =>
        Inner.Add(
            name,
            new()
            {
                Metadata = metadata,
                TimeToKeep = timeToKeep,
                Cleanup = cleanup.WrapCleanupInCheck(name),
                BytesInstance = bytes,
            });

    public void AddString(string value, Encoding? encoding, GetTimeToKeep? timeToKeep = null, Action? cleanup = null, IReadOnlyDictionary<string, string>? metadata = null) =>
        AddString("default", value, encoding, timeToKeep, cleanup, metadata);

    public void AddString(string name, string value, Encoding? encoding, GetTimeToKeep? timeToKeep = null, Action? cleanup = null, IReadOnlyDictionary<string, string>? metadata = null) =>
        Inner.Add(
            name,
            new()
            {
                Metadata = metadata,
                TimeToKeep = timeToKeep,
                Cleanup = cleanup.WrapCleanupInCheck(name),
                Encoding = encoding,
                StringInstance = value,
            });

    public void AddBytes(Func<Task<byte[]>> bytesFactory, GetTimeToKeep? timeToKeep = null, Action? cleanup = null, IReadOnlyDictionary<string, string>? metadata = null) =>
        AddBytes("default", bytesFactory, timeToKeep, cleanup, metadata);

    public void AddBytes(string name, Func<Task<byte[]>> bytesFactory, GetTimeToKeep? timeToKeep = null, Action? cleanup = null, IReadOnlyDictionary<string, string>? metadata = null) =>
        Inner.Add(
            name,
            new()
            {
                Metadata = metadata,
                Cleanup = cleanup.WrapCleanupInCheck(name),
                AsyncBytesFactory = bytesFactory.WrapFuncTaskInCheck(name)
            });
}