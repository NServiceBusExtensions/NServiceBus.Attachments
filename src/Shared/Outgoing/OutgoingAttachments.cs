using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if FileShare
using NServiceBus.Attachments.FileShare;
#elif Sql
using NServiceBus.Attachments.Sql;
#else
using NServiceBus.Attachments;
#endif

class OutgoingAttachments :
    IOutgoingAttachments
{
    [NonSerialized] public Dictionary<string, Outgoing> Inner = new(StringComparer.OrdinalIgnoreCase);
    public List<Duplicate> Duplicates = new();

    public bool HasPendingAttachments => Inner.Any() ||
                                         DuplicateIncomingAttachments ||
                                         Duplicates.Any();

    public bool DuplicateIncomingAttachments;

    public IReadOnlyList<string> Names => Inner.Keys.ToList();

    public void Add<T>(Func<Task<T>> streamFactory, GetTimeToKeep? timeToKeep = null, Action? cleanup = null, IReadOnlyDictionary<string, string>? metadata = null)
        where T : Stream
    {
        Add("default", streamFactory, timeToKeep, cleanup, metadata);
    }

    public void Add<T>(string name, Func<Task<T>> streamFactory, GetTimeToKeep? timeToKeep = null, Action? cleanup = null, IReadOnlyDictionary<string, string>? metadata = null)
        where T : Stream
    {
        Guard.AgainstNull(name, nameof(name));
        Guard.AgainstNull(streamFactory, nameof(streamFactory));
        Inner.Add(
            name,
            new Outgoing(metadata, timeToKeep, cleanup.WrapCleanupInCheck(name), null)
            {
                AsyncStreamFactory = streamFactory.WrapStreamFuncTaskInCheck(name)
            });
    }

    public void Add(Func<Stream> streamFactory, GetTimeToKeep? timeToKeep = null, Action? cleanup = null, IReadOnlyDictionary<string, string>? metadata = null)
    {
        Add("default", streamFactory, timeToKeep, cleanup, metadata);
    }

    public void Add(Stream stream, GetTimeToKeep? timeToKeep = null, Action? cleanup = null, IReadOnlyDictionary<string, string>? metadata = null)
    {
        Add("default", stream, timeToKeep, cleanup, metadata);
    }

    public void Add(string name, Func<Stream> streamFactory, GetTimeToKeep? timeToKeep = null, Action? cleanup = null, IReadOnlyDictionary<string, string>? metadata = null)
    {
        Guard.AgainstNull(name, nameof(name));
        Guard.AgainstNull(streamFactory, nameof(streamFactory));
        Inner.Add(
            name,
            new Outgoing(metadata, timeToKeep, cleanup.WrapCleanupInCheck(name), null)
            {
                StreamFactory = streamFactory.WrapFuncInCheck(name),
            });
    }

    public void Add(string name, Stream stream, GetTimeToKeep? timeToKeep = null, Action? cleanup = null, IReadOnlyDictionary<string, string>? metadata = null)
    {
        Guard.AgainstNull(name, nameof(name));
        Guard.AgainstNull(stream, nameof(stream));
        Inner.Add(
            name,
            new Outgoing(metadata, timeToKeep, cleanup.WrapCleanupInCheck(name), null)
            {
                StreamInstance = stream
            });
    }

    public void AddBytes(Func<byte[]> bytesFactory, GetTimeToKeep? timeToKeep = null, Action? cleanup = null, IReadOnlyDictionary<string, string>? metadata = null)
    {
        AddBytes("default", bytesFactory, timeToKeep, cleanup, metadata);
    }

    public void AddBytes(byte[] bytes, GetTimeToKeep? timeToKeep = null, Action? cleanup = null, IReadOnlyDictionary<string, string>? metadata = null)
    {
        AddBytes("default", bytes, timeToKeep, cleanup, metadata);
    }

    public void DuplicateIncoming()
    {
        DuplicateIncomingAttachments = true;
    }

    public void DuplicateIncoming(string incomingName, string? outgoingName = null)
    {
        Guard.AgainstNull(incomingName, nameof(incomingName));
        Duplicates.Add(new Duplicate
        (
            from: incomingName,
            to: outgoingName
        ));
    }

    public void AddBytes(string name, Func<byte[]> bytesFactory, GetTimeToKeep? timeToKeep = null, Action? cleanup = null, IReadOnlyDictionary<string, string>? metadata = null)
    {
        Guard.AgainstNull(name, nameof(name));
        Guard.AgainstNull(bytesFactory, nameof(bytesFactory));
        Inner.Add(
            name,
            new Outgoing(metadata, timeToKeep, cleanup.WrapCleanupInCheck(name), null)
            {
                BytesFactory = bytesFactory.WrapFuncInCheck(name)
            });
    }

    public void AddBytes(string name, byte[] bytes, GetTimeToKeep? timeToKeep = null, Action? cleanup = null, IReadOnlyDictionary<string, string>? metadata = null)
    {
        Guard.AgainstNull(name, nameof(name));
        Guard.AgainstNull(bytes, nameof(bytes));
        Inner.Add(
            name,
            new Outgoing(metadata, timeToKeep, cleanup.WrapCleanupInCheck(name), null)
            {
                BytesInstance = bytes,
            });
    }

    public void AddString(string value, Encoding? encoding, GetTimeToKeep? timeToKeep = null, Action? cleanup = null, IReadOnlyDictionary<string, string>? metadata = null)
    {
        AddString("default", value, encoding, timeToKeep, cleanup, metadata);
    }

    public void AddString(string name, string value, Encoding? encoding, GetTimeToKeep? timeToKeep = null, Action? cleanup = null, IReadOnlyDictionary<string, string>? metadata = null)
    {
        Guard.AgainstNull(name, nameof(name));
        Guard.AgainstNull(value, nameof(value));
        Inner.Add(
            name,
            new Outgoing(metadata, timeToKeep, cleanup.WrapCleanupInCheck(name), encoding)
            {
                StringInstance = value,
            });
    }

    public void AddBytes(Func<Task<byte[]>> bytesFactory, GetTimeToKeep? timeToKeep = null, Action? cleanup = null, IReadOnlyDictionary<string, string>? metadata = null)
    {
        AddBytes("default", bytesFactory, timeToKeep, cleanup, metadata);
    }

    public void AddBytes(string name, Func<Task<byte[]>> bytesFactory, GetTimeToKeep? timeToKeep = null, Action? cleanup = null, IReadOnlyDictionary<string, string>? metadata = null)
    {
        Guard.AgainstNull(name, nameof(name));
        Guard.AgainstNull(bytesFactory, nameof(bytesFactory));
        Inner.Add(
            name,
            new Outgoing(metadata, timeToKeep, cleanup.WrapCleanupInCheck(name), null)
            {
                AsyncBytesFactory = bytesFactory.WrapFuncTaskInCheck(name)
            });
    }
}