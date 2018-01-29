using NServiceBus.Attachments;
using NServiceBus.Attachments.Testing;
using NServiceBus.Extensibility;

namespace NServiceBus
{
    //todo: throw if attachments not enabled
    public static partial class MessageContextExtensions
    {
        public static MockAttachmentService MessageSessionMock { get; set; }

        public static IOutgoingAttachment OutgoingAttachmentFor(this IMessageSession context, PublishOptions options)
        {
            Guard.AgainstNull(context, nameof(context));
            Guard.AgainstNull(options, nameof(options));
            if (MessageSessionMock != null)
            {
                return MessageSessionMock.BuildOutgoingAttachment(context, options);
            }

            return GetOutgoingAttachment(options);
        }

        public static IOutgoingAttachment OutgoingAttachmentFor(this IMessageSession context, SendOptions options)
        {
            Guard.AgainstNull(context, nameof(context));
            Guard.AgainstNull(options, nameof(options));
            if (MessageSessionMock != null)
            {
                return MessageSessionMock.BuildOutgoingAttachment(context, options);
            }

            return GetOutgoingAttachment(options);
        }

        public static IOutgoingAttachment OutgoingAttachmentFor(this IMessageSession context, ReplyOptions options)
        {
            Guard.AgainstNull(context, nameof(context));
            Guard.AgainstNull(options, nameof(options));
            if (MessageSessionMock != null)
            {
                return MessageSessionMock.BuildOutgoingAttachment(context, options);
            }

            return GetOutgoingAttachment(options);
        }

        public static IOutgoingAttachments OutgoingAttachmentsFor(this IMessageSession context, PublishOptions options)
        {
            Guard.AgainstNull(context, nameof(context));
            Guard.AgainstNull(options, nameof(options));
            if (MessageSessionMock != null)
            {
                return MessageSessionMock.BuildOutgoingAttachments(context, options);
            }

            return GetOutgoingAttachments(options);
        }

        public static IOutgoingAttachments OutgoingAttachmentsFor(this IMessageSession context, SendOptions options)
        {
            Guard.AgainstNull(context, nameof(context));
            Guard.AgainstNull(options, nameof(options));
            if (MessageSessionMock != null)
            {
                return MessageSessionMock.BuildOutgoingAttachments(context, options);
            }

            return GetOutgoingAttachments(options);
        }

        public static IOutgoingAttachments OutgoingAttachmentsFor(this IMessageSession context, ReplyOptions options)
        {
            Guard.AgainstNull(context, nameof(context));
            Guard.AgainstNull(options, nameof(options));
            if (MessageSessionMock != null)
            {
                return MessageSessionMock.BuildOutgoingAttachments(context, options);
            }

            return GetOutgoingAttachments(options);
        }

        public static IOutgoingAttachment OutgoingAttachmentFor(this IMessageHandlerContext context, PublishOptions options)
        {
            Guard.AgainstNull(context, nameof(context));
            Guard.AgainstNull(options, nameof(options));
            if (context.Extensions.TryGet<MockAttachmentService>(out var attachments))
            {
                return attachments.BuildOutgoingAttachment(context, options);
            }

            return GetOutgoingAttachment(options);
        }

        public static IOutgoingAttachment OutgoingAttachmentFor(this IMessageHandlerContext context, SendOptions options)
        {
            Guard.AgainstNull(context, nameof(context));
            Guard.AgainstNull(options, nameof(options));
            if (context.Extensions.TryGet<MockAttachmentService>(out var attachments))
            {
                return attachments.BuildOutgoingAttachment(context, options);
            }

            return GetOutgoingAttachment(options);
        }

        public static IOutgoingAttachment OutgoingAttachmentFor(this IMessageHandlerContext context, ReplyOptions options)
        {
            Guard.AgainstNull(context, nameof(context));
            Guard.AgainstNull(options, nameof(options));
            if (context.Extensions.TryGet<MockAttachmentService>(out var attachments))
            {
                return attachments.BuildOutgoingAttachment(context, options);
            }

            return GetOutgoingAttachment(options);
        }

        public static IOutgoingAttachments OutgoingAttachmentsFor(this IMessageHandlerContext context, PublishOptions options)
        {
            Guard.AgainstNull(context, nameof(context));
            Guard.AgainstNull(options, nameof(options));
            if (context.Extensions.TryGet<MockAttachmentService>(out var attachments))
            {
                return attachments.BuildOutgoingAttachments(context, options);
            }

            return GetOutgoingAttachments(options);
        }

        public static IOutgoingAttachments OutgoingAttachmentsFor(this IMessageHandlerContext context, SendOptions options)
        {
            Guard.AgainstNull(context, nameof(context));
            Guard.AgainstNull(options, nameof(options));
            if (context.Extensions.TryGet<MockAttachmentService>(out var attachments))
            {
                return attachments.BuildOutgoingAttachments(context, options);
            }

            return GetOutgoingAttachments(options);
        }

        public static IOutgoingAttachments OutgoingAttachmentsFor(this IMessageHandlerContext context, ReplyOptions options)
        {
            Guard.AgainstNull(context, nameof(context));
            Guard.AgainstNull(options, nameof(options));
            if (context.Extensions.TryGet<MockAttachmentService>(out var attachments))
            {
                return attachments.BuildOutgoingAttachments(context, options);
            }

            return GetOutgoingAttachments(options);
        }

        static IOutgoingAttachments GetOutgoingAttachments(ExtendableOptions options)
        {
            IOutgoingAttachments attachments = new OutgoingAttachments();
            options.GetExtensions().Set(attachments);
            return attachments;
        }

        static IOutgoingAttachment GetOutgoingAttachment(ExtendableOptions options)
        {
            return new OutgoingAttachment(GetOutgoingAttachments(options));
        }
    }
}