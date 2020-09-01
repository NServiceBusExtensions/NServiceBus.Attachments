using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

namespace NServiceBus.Attachments
#if FileShare
    .FileShare
#endif
#if Sql
    .Sql
#endif
{
    class Outgoing
    {
        public Outgoing(IReadOnlyDictionary<string, string>? metadata, GetTimeToKeep? timeToKeep, Action? cleanup, Encoding? encoding)
        {
            Metadata = metadata;
            TimeToKeep = timeToKeep;
            Cleanup = cleanup;
            Encoding = encoding;
        }

        public Encoding? Encoding;
        public Func<Task<Stream>>? AsyncStreamFactory;
        public Func<Stream>? StreamFactory;
        public Stream? StreamInstance;
        public Func<Task<byte[]>>? AsyncBytesFactory;
        public Func<byte[]>? BytesFactory;
        public byte[]? BytesInstance;
        public string? StringInstance;
        public readonly GetTimeToKeep? TimeToKeep;
        public readonly Action? Cleanup;
        public readonly IReadOnlyDictionary<string, string>? Metadata;
    }
}