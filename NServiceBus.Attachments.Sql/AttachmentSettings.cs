using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using NServiceBus.Attachments;

namespace NServiceBus
{
    public class AttachmentSettings
    {
        internal Func<Task<SqlConnection>> ConnectionBuilder;
        internal bool RunCleanTask = true;
        internal string Schema = "dbo";
        internal string TableName = "Attachments";
        internal bool InstallerDisabled;
        internal GetTimeToKeep TimeToKeep;

        internal AttachmentSettings(Func<Task<SqlConnection>> connectionBuilder,GetTimeToKeep timeToKeep)
        {
            Guard.AgainstNull(connectionBuilder, nameof(connectionBuilder));
            TimeToKeep = timeToKeep;
            ConnectionBuilder = connectionBuilder;
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