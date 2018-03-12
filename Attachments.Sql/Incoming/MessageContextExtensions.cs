using System;
using NServiceBus.Attachments;

namespace NServiceBus
{
    /// <summary>
    /// Contextual extensions to manipulate attachments.
    /// </summary>
    public static partial class MessageContextExtensions
    {
        /// <summary>
        /// Provides an instance of <see cref="IMessageAttachments"/> for reading attachments.
        /// </summary>
        public static IMessageAttachments Attachments(this IMessageHandlerContext context)
        {
            Guard.AgainstNull(context, nameof(context));
            var contextBag = context.Extensions;
            if (contextBag.TryGet<IMessageAttachments>(out var attachments))
            {
                return attachments;
            }

            if (!contextBag.TryGet<SqlAttachmentState>(out var state))
            {
                throw new Exception($"Attachments used when not enabled. For example IMessageHandlerContext.{nameof(Attachments)}() was used but Attachments was not enabled via EndpointConfiguration.{nameof(AttachmentsConfigurationExtensions.EnableAttachments)}().");
            }
            return new MessageAttachments(state.GetConnection, context.MessageId, state.Persister);
        }
    }
}