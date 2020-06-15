using System.Diagnostics.CodeAnalysis;
using System.Transactions;
using NServiceBus.Pipeline;
using NServiceBus.Transport;

static class SqlTransportHelper
{
    public static bool TryReadTransaction(this IOutgoingLogicalMessageContext context, [NotNullWhen(true)] out Transaction? transaction)
    {
        if (context.Extensions.TryGet<TransportTransaction>(out var transportTransaction))
        {
#pragma warning disable 8762
            return transportTransaction.TryGet(out transaction);
#pragma warning restore 8762
        }

        transaction = null;
        return false;
    }
}