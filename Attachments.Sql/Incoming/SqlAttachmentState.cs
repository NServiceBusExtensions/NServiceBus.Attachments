using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using NServiceBus.Attachments.Sql;

class SqlAttachmentState: IDisposable
{
    public readonly Persister Persister;
    SqlConnection connection;
    Lazy<Func<Task<SqlConnection>>> lazy;

    public SqlAttachmentState(Func<Task<SqlConnection>> connectionFactory, Persister persister)
    {
        lazy = new Lazy<Func<Task<SqlConnection>>>(
            () =>
            {
                return async () =>
                {
                    return connection = await connectionFactory().ConfigureAwait(false);
                };
            });
        Persister = persister;
    }

    public Task<SqlConnection> GetConnection()
    {
        if (lazy.IsValueCreated)
        {
            return Task.FromResult(connection);
        }

        return lazy.Value();
    }

    public void Dispose()
    {
        connection?.Dispose();
    }
}