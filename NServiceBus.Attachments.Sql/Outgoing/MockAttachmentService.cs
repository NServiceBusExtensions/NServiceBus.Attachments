using NServiceBus.Extensibility;

namespace NServiceBus.Attachments.Testing
{
    public partial class MockAttachmentService
    {
        public virtual MockOutgoingAttachment RootBuildMockOutgoingAttachment(IMessageSession context, ExtendableOptions options)
        {
            return new MockOutgoingAttachment(context, options);
        }
        public virtual MockOutgoingAttachment RootBuildMockOutgoingAttachment(IMessageHandlerContext context, ExtendableOptions options)
        {
            return new MockOutgoingAttachment(context, options);
        }

        public virtual IOutgoingAttachments RootBuildMockOutgoingAttachments(IMessageSession context, ExtendableOptions options)
        {
            return new MockOutgoingAttachments(context, options);
        }

        public virtual IOutgoingAttachments RootBuildMockOutgoingAttachments(IMessageHandlerContext context, ExtendableOptions options)
        {
            return new MockOutgoingAttachments(context, options);
        }

        protected virtual IOutgoingAttachments OutgoingAttachments(IMessageHandlerContext context, ExtendableOptions options)
        {
            Guard.AgainstNull(context, nameof(context));
            Guard.AgainstNull(options, nameof(options));
            return RootBuildMockOutgoingAttachments(context, options);
        }

        protected virtual IOutgoingAttachment OutgoingAttachment(IMessageHandlerContext context, ExtendableOptions options)
        {
            Guard.AgainstNull(context, nameof(context));
            Guard.AgainstNull(options, nameof(options));
            return RootBuildMockOutgoingAttachment(context, options);
        }

        protected virtual IOutgoingAttachments OutgoingAttachments(IMessageSession context, ExtendableOptions options)
        {
            Guard.AgainstNull(context, nameof(context));
            Guard.AgainstNull(options, nameof(options));
            return RootBuildMockOutgoingAttachments(context, options);
        }

        protected virtual IOutgoingAttachment OutgoingAttachment(IMessageSession context, ExtendableOptions options)
        {
            Guard.AgainstNull(context, nameof(context));
            Guard.AgainstNull(options, nameof(options));
            return RootBuildMockOutgoingAttachment(context, options);
        }

        public virtual IOutgoingAttachments BuildOutgoingAttachments(IMessageSession context, PublishOptions options)
        {
            Guard.AgainstNull(context, nameof(context));
            Guard.AgainstNull(options, nameof(options));
            return RootBuildMockOutgoingAttachments(context, options);
        }

        public virtual IOutgoingAttachments BuildOutgoingAttachments(IMessageSession context, ReplyOptions options)
        {
            Guard.AgainstNull(context, nameof(context));
            Guard.AgainstNull(options, nameof(options));
            return RootBuildMockOutgoingAttachments(context, options);
        }

        public virtual IOutgoingAttachments BuildOutgoingAttachments(IMessageSession context, SendOptions options)
        {
            Guard.AgainstNull(context, nameof(context));
            Guard.AgainstNull(options, nameof(options));
            return RootBuildMockOutgoingAttachments(context, options);
        }

        public virtual IOutgoingAttachment BuildOutgoingAttachment(IMessageSession context, PublishOptions options)
        {
            Guard.AgainstNull(context, nameof(context));
            Guard.AgainstNull(options, nameof(options));
            return RootBuildMockOutgoingAttachment(context, options);
        }

        public virtual IOutgoingAttachment BuildOutgoingAttachment(IMessageSession context, ReplyOptions options)
        {
            Guard.AgainstNull(context, nameof(context));
            Guard.AgainstNull(options, nameof(options));
            return RootBuildMockOutgoingAttachment(context, options);
        }


        public virtual IOutgoingAttachment BuildOutgoingAttachment(IMessageSession context, SendOptions options)
        {
            Guard.AgainstNull(context, nameof(context));
            Guard.AgainstNull(options, nameof(options));
            return RootBuildMockOutgoingAttachment(context, options);
        }

        public virtual IOutgoingAttachments BuildOutgoingAttachments(IMessageHandlerContext context, PublishOptions options)
        {
            Guard.AgainstNull(context, nameof(context));
            Guard.AgainstNull(options, nameof(options));
            return RootBuildMockOutgoingAttachments(context, options);
        }

        public virtual IOutgoingAttachments BuildOutgoingAttachments(IMessageHandlerContext context, ReplyOptions options)
        {
            Guard.AgainstNull(context, nameof(context));
            Guard.AgainstNull(options, nameof(options));
            return RootBuildMockOutgoingAttachments(context, options);
        }

        public virtual IOutgoingAttachments BuildOutgoingAttachments(IMessageHandlerContext context, SendOptions options)
        {
            Guard.AgainstNull(context, nameof(context));
            Guard.AgainstNull(options, nameof(options));
            return RootBuildMockOutgoingAttachments(context, options);
        }

        public virtual IOutgoingAttachment BuildOutgoingAttachment(IMessageHandlerContext context, PublishOptions options)
        {
            Guard.AgainstNull(context, nameof(context));
            Guard.AgainstNull(options, nameof(options));
            return RootBuildMockOutgoingAttachment(context, options);
        }

        public virtual IOutgoingAttachment BuildOutgoingAttachment(IMessageHandlerContext context, ReplyOptions options)
        {
            Guard.AgainstNull(context, nameof(context));
            Guard.AgainstNull(options, nameof(options));
            return RootBuildMockOutgoingAttachment(context, options);
        }

        public virtual IOutgoingAttachment BuildOutgoingAttachment(IMessageHandlerContext context, SendOptions options)
        {
            Guard.AgainstNull(context, nameof(context));
            Guard.AgainstNull(options, nameof(options));
            return RootBuildMockOutgoingAttachment(context, options);
        }
    }
}