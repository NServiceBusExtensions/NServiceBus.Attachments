using System.Transactions;
using Microsoft.Data.SqlClient;
using NServiceBus.Attachments.Sql;

class SqlAttachmentState
{
    Func<Cancellation, Task<SqlConnection>>? connectionFactory;
    public IPersister Persister;
    public Transaction? Transaction;
    public SqlTransaction? SqlTransaction;
    public SqlConnection? SqlConnection;

    public SqlAttachmentState(SqlConnection connection, IPersister persister)
    {
        SqlConnection = connection;
        Persister = persister;
    }

    public SqlAttachmentState(SqlTransaction transaction, IPersister persister)
    {
        SqlTransaction = transaction;
        Persister = persister;
    }

    public SqlAttachmentState(Transaction transaction, Func<Cancellation, Task<SqlConnection>> connectionFactory, IPersister persister)
    {
        this.connectionFactory = connectionFactory;
        Transaction = transaction;
        Persister = persister;
    }

    public SqlAttachmentState(Func<Cancellation, Task<SqlConnection>> connectionFactory, IPersister persister)
    {
        this.connectionFactory = connectionFactory;
        Persister = persister;
    }

    public Task<SqlConnection> GetConnection(Cancellation cancel)
    {
        try
        {
            return connectionFactory!(cancel);
        }
        catch (Exception exception)
        {
            throw new("Provided ConnectionFactory threw an exception", exception);
        }
    }
}