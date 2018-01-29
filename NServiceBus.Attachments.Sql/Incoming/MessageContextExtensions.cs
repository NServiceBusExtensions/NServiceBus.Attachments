using NServiceBus.Attachments;
using NServiceBus.Attachments.Testing;

namespace NServiceBus
{
    //todo: throw if attachments not enabled
    public static partial class MessageContextExtensions
    {
        public static IMessageAttachments IncomingAttachments(this IMessageHandlerContext context)
        {
            Guard.AgainstNull(context, nameof(context));
            if (context.Extensions.TryGet<MockAttachmentService>(out var attachments))
            {
                return attachments.BuildIncomingAttachments(context);
            }
            return IncomingAttachments(context, context.MessageId);
        }

        public static IMessageAttachment IncomingAttachment(this IMessageHandlerContext context)
        {
            Guard.AgainstNull(context, nameof(context));
            if (context.Extensions.TryGet<MockAttachmentService>(out var attachments))
            {
                return attachments.BuildIncomingAttachment(context);
            }
            var incomingAttachments = context.IncomingAttachments();
            return new MessageAttachment(incomingAttachments);
        }

        public static IMessageAttachments AttachmentsForMessage(this IMessageHandlerContext context, string messageId)
        {
            Guard.AgainstNull(context, nameof(context));
            Guard.AgainstNull(messageId, nameof(messageId));
            if (context.Extensions.TryGet<MockAttachmentService>(out var attachments))
            {
                return attachments.BuildAttachmentsForMessage(context, messageId);
            }
            return IncomingAttachments(context, messageId);
        }

        public static IMessageAttachment AttachmentForMessage(this IMessageHandlerContext context, string messageId)
        {
            Guard.AgainstNull(context, nameof(context));
            Guard.AgainstNull(messageId, nameof(messageId));
            if (context.Extensions.TryGet<MockAttachmentService>(out var attachments))
            {
                return attachments.BuildAttachmentForMessage(context, messageId);
            }
            var incomingAttachments = context.AttachmentsForMessage(messageId);
            return new MessageAttachment(incomingAttachments);
        }

        static IMessageAttachments IncomingAttachments(IMessageHandlerContext context, string messageId)
        {
            var contextBag = context.Extensions;
            var state = contextBag.Get<AttachmentReceiveState>();
            return new MessageAttachments(state.ConnectionFactory, messageId, state.Persister);
        }
    }
}