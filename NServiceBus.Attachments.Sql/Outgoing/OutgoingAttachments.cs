using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus.Attachments;

class OutgoingAttachments: IOutgoingAttachments
{
    internal CancellationToken Cancellation;

    internal Dictionary<string, Outgoing> Streams = new Dictionary<string, Outgoing>(StringComparer.OrdinalIgnoreCase);

    public OutgoingAttachments(CancellationToken cancellation)
    {
        Cancellation = cancellation;
    }

    public bool HasPendingAttachments => Streams.Any();

    public IReadOnlyList<string> StreamNames => Streams.Keys.ToList();

    public void Add<T>(Func<Task<T>> streamFactory, GetTimeToKeep timeToKeep = null, Action cleanup = null, CancellationToken cancellation = default)
        where T : Stream
    {
        Guard.AgainstNull(streamFactory, nameof(streamFactory));
        Streams.Add("", new Outgoing
        {
            AsyncStreamFactory = async () => await streamFactory().ConfigureAwait(false),
            TimeToKeep = timeToKeep,
            Cleanup = cleanup,
            Cancellation = cancellation.Or(Cancellation)
        });
    }

    public void Add<T>(string name, Func<Task<T>> streamFactory, GetTimeToKeep timeToKeep = null, Action cleanup = null, CancellationToken cancellation = default)
        where T : Stream
    {
        Guard.AgainstNull(name, nameof(name));
        Guard.AgainstNull(streamFactory, nameof(streamFactory));
        Streams.Add(name, new Outgoing
        {
            AsyncStreamFactory = async () => await streamFactory().ConfigureAwait(false),
            TimeToKeep = timeToKeep,
            Cleanup = cleanup,
            Cancellation = cancellation.Or(Cancellation)
        });
    }

    public void Add(Func<Stream> streamFactory, GetTimeToKeep timeToKeep = null, Action cleanup = null, CancellationToken cancellation = default)
    {
        Guard.AgainstNull(streamFactory, nameof(streamFactory));
        Streams.Add("", new Outgoing
        {
            StreamFactory = streamFactory,
            TimeToKeep = timeToKeep,
            Cleanup = cleanup,
            Cancellation = cancellation.Or(Cancellation)
        });
    }

    public void Add(Stream stream, GetTimeToKeep timeToKeep = null, Action cleanup = null, CancellationToken cancellation = default)
    {
        Guard.AgainstNull(stream, nameof(stream));
        Streams.Add("", new Outgoing
        {
            StreamInstance = stream,
            TimeToKeep = timeToKeep,
            Cleanup = cleanup,
            Cancellation = cancellation.Or(Cancellation)
        });
    }

    public void Add(string name, Func<Stream> streamFactory, GetTimeToKeep timeToKeep = null, Action cleanup = null, CancellationToken cancellation = default)
    {
        Guard.AgainstNull(name, nameof(name));
        Guard.AgainstNull(streamFactory, nameof(streamFactory));
        Streams.Add(name, new Outgoing
        {
            StreamFactory = streamFactory,
            TimeToKeep = timeToKeep,
            Cleanup = cleanup,
            Cancellation = cancellation.Or(Cancellation)
        });
    }

    public void Add(string name, Stream stream, GetTimeToKeep timeToKeep = null, Action cleanup = null, CancellationToken cancellation = default)
    {
        Guard.AgainstNull(name, nameof(name));
        Guard.AgainstNull(stream, nameof(stream));
        Streams.Add(name, new Outgoing
        {
            StreamInstance = stream,
            TimeToKeep = timeToKeep,
            Cleanup = cleanup,
            Cancellation = cancellation.Or(Cancellation)
        });
    }

    public void AddBytes(Func<byte[]> bytesFactory, GetTimeToKeep timeToKeep = null, Action cleanup = null, CancellationToken cancellation = default)
    {
        Guard.AgainstNull(bytesFactory, nameof(bytesFactory));
        Streams.Add("", new Outgoing
        {
            BytesFactory = bytesFactory,
            TimeToKeep = timeToKeep,
            Cleanup = cleanup,
            Cancellation = cancellation.Or(Cancellation)
        });
    }

    public void AddBytes(byte[] bytes, GetTimeToKeep timeToKeep = null, Action cleanup = null, CancellationToken cancellation = default)
    {
        Guard.AgainstNull(bytes, nameof(bytes));
        Streams.Add("", new Outgoing
        {
            BytesInstance = bytes,
            TimeToKeep = timeToKeep,
            Cleanup = cleanup,
            Cancellation = cancellation.Or(Cancellation)
        });
    }

    public void AddBytes(string name, Func<byte[]> bytesFactory, GetTimeToKeep timeToKeep = null, Action cleanup = null, CancellationToken cancellation = default)
    {
        Guard.AgainstNull(name, nameof(name));
        Guard.AgainstNull(bytesFactory, nameof(bytesFactory));
        Streams.Add(name, new Outgoing
        {
            BytesFactory = bytesFactory,
            TimeToKeep = timeToKeep,
            Cleanup = cleanup,
            Cancellation = cancellation.Or(Cancellation)
        });
    }

    public void AddBytes(string name, byte[] bytes, GetTimeToKeep timeToKeep = null, Action cleanup = null, CancellationToken cancellation = default)
    {
        Guard.AgainstNull(name, nameof(name));
        Guard.AgainstNull(bytes, nameof(bytes));
        Streams.Add(name, new Outgoing
        {
            BytesInstance = bytes,
            TimeToKeep = timeToKeep,
            Cleanup = cleanup,
            Cancellation = cancellation.Or(Cancellation)
        });
    }
}