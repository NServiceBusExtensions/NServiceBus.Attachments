using System;
using System.Data.SqlClient;

class ConnectionAndTransaction: IDisposable
{
    public readonly SqlConnection Connection;
    public readonly SqlTransaction Transaction;
    bool isOwned;

    public ConnectionAndTransaction(SqlConnection connection, SqlTransaction transaction, bool isOwned)
    {
        Connection = connection;
        Transaction = transaction;
        this.isOwned = isOwned;
    }

    public void Dispose()
    {
        if (isOwned)
        {
            Transaction?.Dispose();
            Connection?.Dispose();
        }
    }
}