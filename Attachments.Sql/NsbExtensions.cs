using System;
using NServiceBus;
using NServiceBus.Attachments.Sql;
using NServiceBus.Pipeline;

static class NsbExtensions
{
    public static void SetPersister(this IInvokeHandlerContext context, IPersister persister)
    {
        context.Extensions.Set(persister);
    }

    public static IPersister GetPersister(this IMessageHandlerContext context)
    {
        if (context.Extensions.TryGet<IPersister>(out var persister))
        {
            return persister;
        }
        throw new Exception($"Attachments used when not enabled. For example IMessageHandlerContext.{nameof(SqlAttachmentsMessageContextExtensions.Attachments)}() was used but Attachments was not enabled via EndpointConfiguration.{nameof(SqlAttachmentsExtensions.EnableAttachments)}().");
    }
}