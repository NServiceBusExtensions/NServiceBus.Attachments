using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

class AttachmentState: IDisposable
{
    public readonly Persister Persister;
    Task<SqlConnection> connection;
    Lazy<Task<SqlConnection>> lazy;

    public AttachmentState(Func<Task<SqlConnection>> connectionFactory, Persister persister)
    {
        lazy = new Lazy<Task<SqlConnection>>(
            () =>
            {
                 return connection = connectionFactory();
            });
        Persister = persister;
    }

    public Task<SqlConnection> GetConnection()
    {
        return lazy.IsValueCreated ? connection : lazy.Value;
    }

    public void Dispose()
    {
        if (lazy.IsValueCreated)
        {
            connection.Result?.Dispose();
        }
    }
}