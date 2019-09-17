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
    public DbTransaction DbTransaction;
    public DbConnection DbConnection;

    public SqlAttachmentState(DbConnection connection, IPersister persister)
    {
        DbConnection = connection;
        Persister = persister;
    }

    public SqlAttachmentState(DbTransaction transaction, IPersister persister)
    {
        DbTransaction = transaction;
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