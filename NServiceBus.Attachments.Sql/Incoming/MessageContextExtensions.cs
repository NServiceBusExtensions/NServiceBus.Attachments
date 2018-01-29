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
            if (context.Extensions.TryGet<MockMessageAttachmentService>(out var attachments))
            {
                return attachments.BuildAttachments(context);
            }

            var state = context.AttachmentReceiveState();
            return new MessageAttachments(state.ConnectionFactory, context.MessageId, state.Persister);
        }

        public static IMessageAttachment IncomingAttachment(this IMessageHandlerContext context)
        {
            Guard.AgainstNull(context, nameof(context));
            if (context.Extensions.TryGet<MockMessageAttachmentService>(out var attachments))
            {
                return attachments.BuildAttachment(context);
            }

            var state = context.AttachmentReceiveState();
            var incomingAttachments = new MessageAttachments(state.ConnectionFactory, context.MessageId, state.Persister);
            return new MessageAttachment(incomingAttachments);
        }

        public static IMessageAttachments AttachmentsForMessage(this IMessageHandlerContext context, string messageId)
        {
            Guard.AgainstNull(context, nameof(context));
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            if (context.Extensions.TryGet<MockMessageAttachmentService>(out var attachments))
            {
                return attachments.BuildAttachmentsForMessage(context, messageId);
            }

            var state = context.AttachmentReceiveState();
            return new MessageAttachments(state.ConnectionFactory, messageId, state.Persister);
        }

        public static IMessageAttachment AttachmentForMessage(this IMessageHandlerContext context, string messageId)
        {
            Guard.AgainstNull(context, nameof(context));
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            if (context.Extensions.TryGet<MockMessageAttachmentService>(out var attachments))
            {
                return attachments.BuildAttachmentForMessage(context, messageId);
            }

            var state = context.AttachmentReceiveState();
            var messageAttachments = new MessageAttachments(state.ConnectionFactory, messageId, state.Persister);
            return new MessageAttachment(messageAttachments);
        }

        static IMessageAttachments IncomingAttachments(IMessageHandlerContext context, string messageId)
        {
            Guard.AgainstNull(context, "context");
            Guard.AgainstNullOrEmpty(messageId, "messageId");
            if (context.Extensions.TryGet<MockMessageAttachmentService>(out var attachments))
            {
                return attachments.BuildAttachmentsForMessage(context, messageId);
            }

            var state = context.AttachmentReceiveState();
            return new MessageAttachments(state.ConnectionFactory, messageId, state.Persister);
        }

        static AttachmentReceiveState AttachmentReceiveState(this IMessageHandlerContext context)
        {
            return context.Extensions.Get<AttachmentReceiveState>();
        }
    }
}