using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

class AttachmentReceiveState
{
    public StreamPersister Persister;
    public Func<Task<SqlConnection>> ConnectionFactory;
}