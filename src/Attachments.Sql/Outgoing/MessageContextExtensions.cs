using NServiceBus.Attachments.Sql;
using NServiceBus.Extensibility;

namespace NServiceBus;

public static partial class SqlAttachmentsMessageContextExtensions
{
    /// <summary>
    /// Provides an instance of <see cref="IOutgoingAttachments" /> for writing attachments.
    /// </summary>
    public static IOutgoingAttachments Attachments(this PublishOptions options) =>
        GetAttachments(options);

    /// <summary>
    /// Provides an instance of <see cref="IOutgoingAttachments" /> for writing attachments.
    /// </summary>
    public static IOutgoingAttachments Attachments(this SendOptions options) =>
        GetAttachments(options);

    /// <summary>
    /// Provides an instance of <see cref="IOutgoingAttachments" /> for writing attachments.
    /// </summary>
    public static IOutgoingAttachments Attachments(this ReplyOptions options) =>
        GetAttachments(options);

    static IOutgoingAttachments GetAttachments(this ExtendableOptions options)
    {
        var contextBag = options.GetExtensions();
        if (contextBag.TryGet<IOutgoingAttachments>(out var attachments))
        {
            return attachments;
        }

        attachments = new OutgoingAttachments();
        contextBag.Set(attachments);
        return attachments;
    }
}