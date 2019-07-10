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
        [NonSerialized]
        public Func<Task<Stream>> AsyncStreamFactory;
        [NonSerialized]
        public Func<Stream> StreamFactory;
        public Stream StreamInstance;
        [NonSerialized]
        public Func<Task<byte[]>> AsyncBytesFactory;
        [NonSerialized]
        public Func<byte[]> BytesFactory;
        public byte[] BytesInstance;
        public string StringInstance;
        public GetTimeToKeep TimeToKeep;
        [NonSerialized]
        public Action Cleanup;
        public IReadOnlyDictionary<string, string> Metadata;
    }
}