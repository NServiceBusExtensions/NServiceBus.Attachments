using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

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
        public Func<Task<Stream>> AsyncStreamFactory;
        public Func<Stream> StreamFactory;
        public Stream StreamInstance;
        public Func<Task<byte[]>> AsyncBytesFactory;
        public Func<byte[]> BytesFactory;
        public byte[] BytesInstance;
        public GetTimeToKeep TimeToKeep;
        public Action Cleanup;
        public IReadOnlyDictionary<string, string> Metadata;
    }
}