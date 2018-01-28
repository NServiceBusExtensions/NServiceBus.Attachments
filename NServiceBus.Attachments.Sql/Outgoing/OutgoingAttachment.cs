using System;
using System.IO;
using System.Threading.Tasks;
using NServiceBus.Attachments;

class OutgoingAttachment : IOutgoingAttachment
{
    OutgoingAttachments attachments;

    public OutgoingAttachment(OutgoingAttachments attachments)
    {
        this.attachments = attachments;
    }

    public void Add<T>(Func<Task<T>> stream, GetTimeToKeep timeToKeep = null, Action cleanup = null) where T : Stream
    {
        Guard.AgainstNull(stream, nameof(stream));
        attachments.Add(string.Empty, stream, timeToKeep, cleanup);
    }

    public void Add(Func<Stream> stream, GetTimeToKeep timeToKeep = null, Action cleanup = null)
    {
        Guard.AgainstNull(stream, nameof(stream));
        attachments.Add(string.Empty, stream, timeToKeep, cleanup);
    }

    public void Add(Stream stream, GetTimeToKeep timeToKeep = null, Action cleanup = null)
    {
        Guard.AgainstNull(stream, nameof(stream));
        attachments.Add(string.Empty, stream, timeToKeep, cleanup);
    }
}