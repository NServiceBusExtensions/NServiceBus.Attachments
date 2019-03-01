using System.IO;
// ReSharper disable once RedundantUsingDirective
using NServiceBus.Attachments;

#if FileShare
using NServiceBus.Attachments.FileShare;
#endif
#if Sql
using NServiceBus.Attachments.Sql;
#endif

static class MockAttachmentExtensions
{
    public static AttachmentStream ToAttachmentStream(this MockAttachment attachment)
    {
        var bytes = attachment.Bytes;
        var stream = new MemoryStream(bytes);
        return new AttachmentStream(stream, bytes.LongLength, attachment.Metadata, stream);
    }

    public static AttachmentBytes ToAttachmentBytes(this MockAttachment attachment)
    {
        return new AttachmentBytes(attachment.Bytes, attachment.Metadata);
    }
}