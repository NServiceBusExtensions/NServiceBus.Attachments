using System;
using System.IO;
using System.Threading.Tasks;
using NServiceBus.Attachments;

class OutgoingStream
{
    public Func<Task<Stream>> Func;
    public Stream Instance;
    public GetTimeToKeep TimeToKeep;
    public Action Cleanup;
}