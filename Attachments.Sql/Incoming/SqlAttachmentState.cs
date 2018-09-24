using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using NServiceBus.Attachments.Sql;

class SqlAttachmentState
{
    Func<Task<SqlConnection>> connectionFactory;
    public IPersister Persister;
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