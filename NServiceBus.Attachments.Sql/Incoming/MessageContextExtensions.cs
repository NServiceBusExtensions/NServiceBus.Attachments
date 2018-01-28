using NServiceBus.Attachments;

namespace NServiceBus
{
    //todo: throw if attachments not enabled
    public static partial class MessageContextExtensions
    {
        public static IMessageAttachments IncomingAttachments(this IMessageHandlerContext context)
        {
            return IncomingAttachments(context, context.MessageId);
        }

        public static IMessageAttachment IncomingAttachment(this IMessageHandlerContext context)
        {
            var incomingAttachments = context.IncomingAttachments();
            return new MessageAttachment(incomingAttachments);
        }

        public static IMessageAttachments AttachmentsForMessage(this IMessageHandlerContext context, string messageId)
        {
            return IncomingAttachments(context, messageId);
        }

        public static IMessageAttachment AttachmentForMessage(this IMessageHandlerContext context, string messageId)
        {
            var incomingAttachments = context.AttachmentsForMessage(messageId);
            return new MessageAttachment(incomingAttachments);
        }

        static IMessageAttachments IncomingAttachments(IMessageHandlerContext context, string messageId)
        {
            Guard.AgainstNull(context, "context");
            Guard.AgainstNullOrEmpty(messageId, "messageId");
            var settings = context.Extensions.Get<AttachmentSettings>();
            var connectionFactory = context.GetConnectionFactory();
            return new MessageAttachments(connectionFactory, messageId, settings.Persister);
        }
    }
}