using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus.Attachments;

namespace NServiceBus
{
    /// <summary>
    /// All settings for attachments
    /// </summary>
    public class AttachmentSettings
    {
        internal Func<CancellationToken, Task<SqlConnection>> ConnectionFactory;
        internal bool RunCleanTask = true;
        internal string Schema = "dbo";
        internal string TableName = "Attachments";
        internal bool InstallerDisabled;
        internal GetTimeToKeep TimeToKeep;
        internal CancellationToken Cancellation;

        internal AttachmentSettings(Func<CancellationToken, Task<SqlConnection>> connectionFactory, GetTimeToKeep timeToKeep)
        {
            Guard.AgainstNull(connectionFactory, nameof(connectionFactory));
            TimeToKeep = timeToKeep;
            ConnectionFactory = connectionFactory;
        }

        /// <summary>
        /// Disable the attachment cleanup task.
        /// </summary>
        public void DisableCleanupTask()
        {
            RunCleanTask = false;
        }

        /// <summary>
        /// Disable the table creation installer.
        /// </summary>
        public void DisableInstaller()
        {
            InstallerDisabled = true;
        }

        /// <summary>
        /// Use a specific <paramref name="tableName"/> and <paramref name="schema"/> for the attachments table.
        /// </summary>
        public void UseTableName(string tableName, string schema = "dbo")
        {
            Guard.AgainstNullOrEmpty(tableName, nameof(tableName));
            Guard.AgainstNullOrEmpty(schema, nameof(schema));
            TableName = tableName;
            Schema = schema;
        }

        /// <summary>
        /// The root <see cref="CancellationToken"/> for all operations. Defaults to <see cref="System.Threading.CancellationToken.None"/>
        /// </summary>
        public void CancellationToken(CancellationToken cancellation)
        {
            Cancellation = cancellation;
        }
    }
}