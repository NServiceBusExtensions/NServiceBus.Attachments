using System.IO;
using System.Text;
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
        return new AttachmentStream(attachment.Name, stream, bytes.LongLength, attachment.Metadata, stream);
    }

    public static AttachmentBytes ToAttachmentBytes(this MockAttachment attachment)
    {
        return new(attachment.Name, attachment.Bytes, attachment.Metadata);
    }

    public static AttachmentString ToAttachmentString(this MockAttachment attachment, Encoding? encoding)
    {
        var value = encoding.Default().GetString(attachment.Bytes);
        return new AttachmentString(attachment.Name, value, attachment.Metadata);
    }
}