using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Pipeline;

static class ConnectionFactoryHelper
{
    public static void SetConnectionFactory(this IInvokeHandlerContext context, Func<Task<SqlConnection>> factory)
    {
        context.Extensions.Set($"{AssemblyHelper.Name}IncomingConnectionFactory", factory);
    }
    public static Func<Task<SqlConnection>> GetConnectionFactory(this IMessageHandlerContext context)
    {
        return context.Extensions.Get< Func<Task<SqlConnection>>>($"{AssemblyHelper.Name}IncomingConnectionFactory");
    }
}