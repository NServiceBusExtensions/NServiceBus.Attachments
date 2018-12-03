using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Transactions;
using NServiceBus.Attachments.Sql;

class SqlAttachmentState
{
    Func<Task<SqlConnection>> connectionFactory;
    public IPersister Persister;
    public Transaction Transaction;
    public SqlTransaction SqlTransaction;
    public SqlConnection SqlConnection;

    public SqlAttachmentState(SqlConnection sqlConnection, IPersister persister)
    {
        SqlConnection = sqlConnection;
        Persister = persister;
    }

    public SqlAttachmentState(SqlTransaction sqlTransaction, IPersister persister)
    {
        SqlTransaction = sqlTransaction;
        Persister = persister;
    }

    public SqlAttachmentState(Transaction transaction, Func<Task<SqlConnection>> connectionFactory, IPersister persister)
    {
        this.connectionFactory = connectionFactory;
        Transaction = transaction;
        Persister = persister;
    }

    public SqlAttachmentState(Func<Task<SqlConnection>> connectionFactory, IPersister persister)
    {
        this.connectionFactory = connectionFactory;
        Persister = persister;
    }

    public Task<SqlConnection> GetConnection()
    {
        try
        {
            return connectionFactory();
        }
        catch (Exception exception)
        {
            throw new Exception("Provided ConnectionFactory threw an exception", exception);
        }
    }
}