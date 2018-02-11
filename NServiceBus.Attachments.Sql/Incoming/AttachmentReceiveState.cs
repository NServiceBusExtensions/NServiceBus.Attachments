using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

class AttachmentReceiveState
{
    public Persister Persister;
    public Func<CancellationToken, Task<SqlConnection>> ConnectionFactory;
}