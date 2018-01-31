using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus.Attachments;

class OutgoingAttachments: IOutgoingAttachments
{
    internal Dictionary<string, OutgoingStream> Streams = new Dictionary<string, OutgoingStream>(StringComparer.OrdinalIgnoreCase);

    public bool HasPendingAttachments => Streams.Any();

    public IReadOnlyList<string> StreamNames => Streams.Keys.ToList();

    public void Add<T>(Func<Task<T>> streamFactory, GetTimeToKeep timeToKeep = null, Action cleanup = null) where T : Stream
    {
        Guard.AgainstNull(streamFactory, nameof(streamFactory));
        Streams.Add("", new OutgoingStream
        {
            Func = async () => await streamFactory().ConfigureAwait(false),
            TimeToKeep = timeToKeep,
            Cleanup = cleanup
        });
    }

    public void Add(Func<Stream> streamFactory, GetTimeToKeep timeToKeep = null, Action cleanup = null)
    {
        Guard.AgainstNull(streamFactory, nameof(streamFactory));
        Streams.Add("", new OutgoingStream
        {
            Func = () => Task.FromResult(streamFactory()),
            TimeToKeep = timeToKeep,
            Cleanup = cleanup
        });
    }

    public void Add(Stream stream, GetTimeToKeep timeToKeep = null, Action cleanup = null)
    {
        Guard.AgainstNull(stream, nameof(stream));
        Streams.Add("", new OutgoingStream
        {
            Instance = stream,
            TimeToKeep = timeToKeep,
            Cleanup = cleanup
        });
    }
    public void Add<T>(string name, Func<Task<T>> streamFactory, GetTimeToKeep timeToKeep = null, Action cleanup = null) where T : Stream
    {
        Guard.AgainstNull(name, nameof(name));
        Guard.AgainstNull(streamFactory, nameof(streamFactory));
        Streams.Add(name, new OutgoingStream
        {
            Func = async () => await streamFactory().ConfigureAwait(false),
            TimeToKeep = timeToKeep,
            Cleanup = cleanup
        });
    }

    public void Add(string name, Func<Stream> streamFactory, GetTimeToKeep timeToKeep = null, Action cleanup = null)
    {
        Guard.AgainstNull(name, nameof(name));
        Guard.AgainstNull(streamFactory, nameof(streamFactory));
        Streams.Add(name, new OutgoingStream
        {
            Func = () => Task.FromResult(streamFactory()),
            TimeToKeep = timeToKeep,
            Cleanup = cleanup
        });
    }

    public void Add(string name, Stream stream, GetTimeToKeep timeToKeep = null, Action cleanup = null)
    {
        Guard.AgainstNull(name, nameof(name));
        Guard.AgainstNull(stream, nameof(stream));
        Streams.Add(name, new OutgoingStream
        {
            Instance = stream,
            TimeToKeep = timeToKeep,
            Cleanup = cleanup
        });
    }
}