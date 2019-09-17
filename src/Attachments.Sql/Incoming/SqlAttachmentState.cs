using System;
using System.Data.Common;
using System.Threading.Tasks;
using System.Transactions;
using NServiceBus.Attachments.Sql;

class SqlAttachmentState
{
    Func<Task<DbConnection>> connectionFactory;
    public IPersister Persister;
    public Transaction Transaction;
    public DbTransaction SqlTransaction;
    public DbConnection SqlConnection;

    public SqlAttachmentState(DbConnection sqlConnection, IPersister persister)
    {
        SqlConnection = sqlConnection;
        Persister = persister;
    }

    public SqlAttachmentState(DbTransaction sqlTransaction, IPersister persister)
    {
        SqlTransaction = sqlTransaction;
        Persister = persister;
    }

    public SqlAttachmentState(Transaction transaction, Func<Task<DbConnection>> connectionFactory, IPersister persister)
    {
        this.connectionFactory = connectionFactory;
        Transaction = transaction;
        Persister = persister;
    }

    public SqlAttachmentState(Func<Task<DbConnection>> connectionFactory, IPersister persister)
    {
        this.connectionFactory = connectionFactory;
        Persister = persister;
    }

    public Task<DbConnection> GetConnection()
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