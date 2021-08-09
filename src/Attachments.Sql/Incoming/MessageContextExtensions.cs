﻿using NServiceBus.Attachments.Sql;

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
            var contextBag = context.Extensions;
            // check the context for a IMessageAttachments in case a mocked instance is injected for testing
            if (contextBag.TryGet<IMessageAttachments>(out var attachments))
            {
                return attachments;
            }

            if (contextBag.TryGet<SqlAttachmentState>(out var state))
            {
                if (state.Transaction is not null)
                {
                    return new MessageAttachmentsFromTransaction(state.Transaction, state.GetConnection, context.MessageId, state.Persister);
                }
                if (state.DbTransaction is not null)
                {
                    return new MessageAttachmentsFromDbTransaction(state.DbTransaction, context.MessageId, state.Persister);
                }
                if (state.DbConnection is not null)
                {
                    return new MessageAttachmentsFromDbConnection(state.DbConnection, context.MessageId, state.Persister);
                }
                return new MessageAttachmentsFromSqlFactory(state.GetConnection, context.MessageId, state.Persister);
            }
            throw new($"Attachments used when not enabled. For example IMessageHandlerContext.{nameof(Attachments)}() was used but Attachments was not enabled via EndpointConfiguration.{nameof(SqlAttachmentsExtensions.EnableAttachments)}().");
        }
    }
}