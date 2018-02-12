using System.Transactions;
using NServiceBus.Pipeline;
using NServiceBus.Transport;

static class SqlTransportHelper
{
    public static bool TryReadTransaction(this IOutgoingLogicalMessageContext context, out Transaction transaction)
    {
        if (context.Extensions.TryGet<TransportTransaction>(out var transportTransaction))
        {
            return transportTransaction.TryGet(out transaction);
        }

        transaction = null;
        return false;
    }
}