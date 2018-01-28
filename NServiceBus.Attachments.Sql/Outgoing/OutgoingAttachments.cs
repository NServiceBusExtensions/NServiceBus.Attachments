using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NServiceBus.Attachments;

class OutgoingAttachments: IOutgoingAttachments
{
    internal Dictionary<string, OutgoingStream> Streams = new Dictionary<string, OutgoingStream>(StringComparer.OrdinalIgnoreCase);

    public void Add<T>(string name, Func<Task<T>> stream, GetTimeToKeep timeToKeep = null, Action cleanup = null) where T : Stream
    {
        Guard.AgainstNull(name, nameof(name));
        Guard.AgainstNull(stream, nameof(stream));
        Streams.Add(name, new OutgoingStream
        {
            Func = async () => await stream(),
            TimeToKeep = timeToKeep,
            Cleanup = cleanup
        });
    }

    public void Add(string name, Func<Stream> stream, GetTimeToKeep timeToKeep = null, Action cleanup = null)
    {
        Guard.AgainstNull(name, nameof(name));
        Guard.AgainstNull(stream, nameof(stream));
        Streams.Add(name, new OutgoingStream
        {
            Func = () => Task.FromResult(stream()),
            TimeToKeep = timeToKeep,
            Cleanup = cleanup
        });
    }
}