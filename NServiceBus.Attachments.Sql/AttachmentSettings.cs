using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using NServiceBus.Attachments;

namespace NServiceBus
{
    public class AttachmentSettings
    {
        internal Func<Task<SqlConnection>> ConnectionFactory;
        internal bool RunCleanTask = true;
        internal string Schema = "dbo";
        internal string TableName = "Attachments";
        internal bool InstallerDisabled;
        internal GetTimeToKeep TimeToKeep;
        internal StreamPersister Persister;

        internal AttachmentSettings(Func<Task<SqlConnection>> connectionFactory, GetTimeToKeep timeToKeep)
        {
            Guard.AgainstNull(connectionFactory, nameof(connectionFactory));
            TimeToKeep = timeToKeep;
            ConnectionFactory = connectionFactory;
        }


        public void DisableCleanupTask()
        {
            RunCleanTask = false;
        }

        public void DisableInstaller()
        {
            InstallerDisabled = true;
        }

        public void SetTimeToKeep(GetTimeToKeep timeToKeep)
        {
            Guard.AgainstNull(timeToKeep, nameof(timeToKeep));
            TimeToKeep = timeToKeep;
        }

        public void UseTableName(string tableName, string schema = "dbo")
        {
            Guard.AgainstNullOrEmpty(tableName, nameof(tableName));
            Guard.AgainstNullOrEmpty(schema, nameof(schema));
            TableName = tableName;
            Schema = schema;
        }
    }
}