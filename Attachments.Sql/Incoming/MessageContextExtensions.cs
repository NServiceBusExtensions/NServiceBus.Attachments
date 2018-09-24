using System;
using NServiceBus.Attachments.Sql;

namespace NServiceBus
{
    /// <summary>
    /// Contextual extensions to manipulate attachments.
    /// </summary>
    public static partial class SqlAttachmentsMessageContextExtensions
    {
        /// <summary>
        /// Provides an instance of <see cref="IMessageAttachments"/> for reading attachments.
        /// </summary>
        public static IMessageAttachments Attachments(this IMessageHandlerContext context)
        {
            Guard.AgainstNull(context, nameof(context));
            var contextBag = context.Extensions;
            // check the context for a IMessageAttachments in case a mocked instance is injected for testing
            if (contextBag.TryGet<IMessageAttachments>(out var attachments))
            {
                return attachments;
            }

            var persister = context.GetPersister();

            if (contextBag.TryGet<SqlAttachmentState>(out var state))
            {
                if (state.Transaction != null)
                {
                    return new MessageAttachmentsFromTransaction(state.Transaction, context.MessageId, persister);
                }
                if (state.Connection != null)
                {
                    return new MessageAttachmentsFromConnection(state.Connection, context.MessageId, persister);
                }
                return new MessageAttachmentsFromFactory(state.GetConnection, context.MessageId, persister);
            }
            throw new Exception($"Attachments used when not enabled. For example IMessageHandlerContext.{nameof(Attachments)}() was used but Attachments was not enabled via EndpointConfiguration.{nameof(SqlAttachmentsExtensions.EnableAttachments)}().");
        }
    }
}