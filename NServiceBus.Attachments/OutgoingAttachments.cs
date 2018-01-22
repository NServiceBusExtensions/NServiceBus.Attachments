using System;
using System.Collections.Generic;
using System.IO;

namespace NServiceBus.Attachments
{
    public class OutgoingAttachments
    {
        internal Dictionary<string, OutgoingStream> Streams = new Dictionary<string, OutgoingStream>(StringComparer.OrdinalIgnoreCase);

        public void Add(string name, Func<Stream> stream, GetTimeToKeep timeToKeep)
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstNull(stream, nameof(stream));
            Guard.AgainstNull(timeToKeep, nameof(timeToKeep));
            Streams.Add(name, new OutgoingStream
            {
                Func = stream,
                TimeToKeep = timeToKeep
            });
        }
    }
}