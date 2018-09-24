using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using NServiceBus.Attachments.Sql;

class SqlAttachmentState : IDisposable
{
    public IPersister Persister;
    Task<SqlConnection> connectionTask;
    Lazy<Task<SqlConnection>> lazy;
    public SqlTransaction Transaction;
    public SqlConnection Connection;

    public SqlAttachmentState(SqlConnection connection, IPersister persister)
    {
        Connection = connection;
        Persister = persister;
    }

    public SqlAttachmentState(SqlTransaction transaction, IPersister persister)
    {
        Transaction = transaction;
        Persister = persister;
    }

    public SqlAttachmentState(Func<Task<SqlConnection>> connectionFactory, IPersister persister)
    {
        lazy = new Lazy<Task<SqlConnection>>(
            () =>
            {
                try
                {
                    connectionTask = connectionFactory();
                }
                catch (Exception exception)
                {
                    throw new Exception("Provided ConnectionFactory threw an exception", exception);
                }

                Guard.ThrowIfNullReturned(connectionTask);
                return connectionTask;
            });
        Persister = persister;
    }

    public Task<SqlConnection> GetConnection()
    {
        if (lazy.IsValueCreated)
        {
            return connectionTask;
        }

        return lazy.Value;
    }

    public void Dispose()
    {
        if (lazy != null && lazy.IsValueCreated)
        {
            connectionTask.Result?.Dispose();
        }
    }
}