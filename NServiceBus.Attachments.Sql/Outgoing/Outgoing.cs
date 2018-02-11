using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus.Attachments;

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
    public CancellationToken Cancellation;
}