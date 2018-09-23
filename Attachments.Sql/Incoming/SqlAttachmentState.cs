using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using NServiceBus.Attachments.Sql;

class SqlAttachmentState : IDisposable
{
    public IPersister Persister;
    public SqlConnection Connection;
    Lazy<Func<Task<SqlConnection>>> lazy;
    public SqlTransaction Transaction;
    bool ownedConnection;

    public SqlAttachmentState(SqlConnection connection, IPersister persister)
    {
        Connection = connection;
        ownedConnection = false;
        Persister = persister;
    }

    public SqlAttachmentState(SqlTransaction transaction, IPersister persister)
    {
        Transaction = transaction;
        Persister = persister;
    }

    public SqlAttachmentState(Func<Task<SqlConnection>> connectionFactory, IPersister persister)
    {
        ownedConnection = true;
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
                    return Connection = sqlConnection;
                };
            });
        Persister = persister;
    }

    public Task<SqlConnection> GetConnection()
    {
        if (Connection != null)
        {
            return Task.FromResult(Connection);
        }

        return lazy.Value();
    }

    public void Dispose()
    {
        if (ownedConnection)
        {
            Connection?.Dispose();
        }
    }
}