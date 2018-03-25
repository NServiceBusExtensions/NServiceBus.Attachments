using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using NServiceBus.Attachments.Sql;

class SqlAttachmentState : IDisposable
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
                    Task<SqlConnection> task;
                    try
                    {
                        task = connectionFactory();
                    }
                    catch (Exception exception)
                    {
                        throw new Exception("Provided ConnectionFactory threw an exception", exception);
                    }

                    Guard.ThrowIfNullReturned(null, null, task);
                    SqlConnection sqlConnection;
                    try
                    {
                        sqlConnection = await task.ConfigureAwait(false);
                    }
                    catch (Exception exception)
                    {
                        throw new Exception("Provided ConnectionFactory threw an exception", exception);
                    }

                    Guard.ThrowIfNullReturned(null, null, sqlConnection);
                    return connection = sqlConnection;
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