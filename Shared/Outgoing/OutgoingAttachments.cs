using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

#if FileShare
using NServiceBus.Attachments.FileShare;
#elif Sql
using NServiceBus.Attachments.Sql;
#else
using NServiceBus.Attachments;
#endif

class OutgoingAttachments : IOutgoingAttachments
{
    internal Dictionary<string, Outgoing> Inner = new Dictionary<string, Outgoing>(StringComparer.OrdinalIgnoreCase);

    public bool HasPendingAttachments => Inner.Any();

    public IReadOnlyList<string> Names => Inner.Keys.ToList();

    public void Add<T>(Func<Task<T>> streamFactory, GetTimeToKeep timeToKeep = null, Action cleanup = null)
        where T : Stream
    {
        Add("default", streamFactory, timeToKeep, cleanup);
    }

    public void Add<T>(string name, Func<Task<T>> streamFactory, GetTimeToKeep timeToKeep = null, Action cleanup = null)
        where T : Stream
    {
        Guard.AgainstNull(name, nameof(name));
        Guard.AgainstNull(streamFactory, nameof(streamFactory));
        Inner.Add(name, new Outgoing
        {
            AsyncStreamFactory = streamFactory.WrapStreamFuncTaskInCheck(name),
            TimeToKeep = timeToKeep,
            Cleanup = cleanup.WrapCleanupInCheck(name)
        });
    }

    public void Add(Func<Stream> streamFactory, GetTimeToKeep timeToKeep = null, Action cleanup = null)
    {
        Add("default", streamFactory, timeToKeep, cleanup);
    }

    public void Add(Stream stream, GetTimeToKeep timeToKeep = null, Action cleanup = null)
    {
        Add("default", stream, timeToKeep, cleanup);
    }

    public void Add(string name, Func<Stream> streamFactory, GetTimeToKeep timeToKeep = null, Action cleanup = null)
    {
        Guard.AgainstNull(name, nameof(name));
        Guard.AgainstNull(streamFactory, nameof(streamFactory));
        Inner.Add(name, new Outgoing
        {
            StreamFactory = streamFactory.WrapFuncInCheck(name),
            TimeToKeep = timeToKeep,
            Cleanup = cleanup.WrapCleanupInCheck(name)
        });
    }

    public void Add(string name, Stream stream, GetTimeToKeep timeToKeep = null, Action cleanup = null)
    {
        Guard.AgainstNull(name, nameof(name));
        Guard.AgainstNull(stream, nameof(stream));
        Inner.Add(name, new Outgoing
        {
            StreamInstance = stream,
            TimeToKeep = timeToKeep,
            Cleanup = cleanup.WrapCleanupInCheck(name)
        });
    }

    public void AddBytes(Func<byte[]> bytesFactory, GetTimeToKeep timeToKeep = null, Action cleanup = null)
    {
        AddBytes("default", bytesFactory, timeToKeep, cleanup);
    }

    public void AddBytes(byte[] bytes, GetTimeToKeep timeToKeep = null, Action cleanup = null)
    {
        AddBytes("default", bytes, timeToKeep, cleanup);
    }

    public void AddBytes(string name, Func<byte[]> bytesFactory, GetTimeToKeep timeToKeep = null, Action cleanup = null)
    {
        Guard.AgainstNull(name, nameof(name));
        Guard.AgainstNull(bytesFactory, nameof(bytesFactory));
        Inner.Add(name, new Outgoing
        {
            BytesFactory = bytesFactory.WrapFuncInCheck(name),
            TimeToKeep = timeToKeep,
            Cleanup = cleanup.WrapCleanupInCheck(name)
        });
    }

    public void AddBytes(string name, byte[] bytes, GetTimeToKeep timeToKeep = null, Action cleanup = null)
    {
        Guard.AgainstNull(name, nameof(name));
        Guard.AgainstNull(bytes, nameof(bytes));
        Inner.Add(name, new Outgoing
        {
            BytesInstance = bytes,
            TimeToKeep = timeToKeep,
            Cleanup = cleanup.WrapCleanupInCheck(name)
        });
    }

    public void AddBytes(Func<Task<byte[]>> bytesFactory, GetTimeToKeep timeToKeep = null, Action cleanup = null)
    {
        AddBytes("default", bytesFactory, timeToKeep, cleanup);
    }

    public void AddBytes(string name, Func<Task<byte[]>> bytesFactory, GetTimeToKeep timeToKeep = null, Action cleanup = null)
    {
        Guard.AgainstNull(name, nameof(name));
        Guard.AgainstNull(bytesFactory, nameof(bytesFactory));
        Inner.Add(name, new Outgoing
        {
            AsyncBytesFactory = bytesFactory.WrapFuncTaskInCheck(name),
            TimeToKeep = timeToKeep,
            Cleanup = cleanup.WrapCleanupInCheck(name)
        });
    }
}