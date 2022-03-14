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
        MemoryStream stream = new(bytes);
        return new(attachment.Name, stream, bytes.LongLength, attachment.Metadata, stream);
    }

    public static AttachmentBytes ToAttachmentBytes(this MockAttachment attachment) =>
        new(attachment.Name, attachment.Bytes, attachment.Metadata);

    public static AttachmentString ToAttachmentString(this MockAttachment attachment, Encoding? encoding)
    {
        var value = encoding.Default().GetString(attachment.Bytes);
        return new(attachment.Name, value, attachment.Metadata);
    }
}