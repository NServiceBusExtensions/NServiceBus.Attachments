using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

class SqlAttachmentState : IDisposable
{
    Task<SqlConnection> connectionTask;
    Lazy<Task<SqlConnection>> lazy;
    public SqlTransaction Transaction;
    public SqlConnection Connection;

    public SqlAttachmentState(SqlConnection connection)
    {
        Connection = connection;
    }

    public SqlAttachmentState(SqlTransaction transaction)
    {
        Transaction = transaction;
    }

    public SqlAttachmentState(Func<Task<SqlConnection>> connectionFactory)
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