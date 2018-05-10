using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace NServiceBus.Attachments.Sql
{
    /// <summary>
    /// All settings for attachments
    /// </summary>
    public partial class AttachmentSettings
    {
        internal Func<Task<SqlConnection>> ConnectionFactory;
        internal string Schema = "dbo";
        internal string TableName = "MessageAttachments";
        internal bool InstallerDisabled;
        internal bool UseMars = true;

        internal AttachmentSettings(Func<Task<SqlConnection>> connectionFactory, GetTimeToKeep timeToKeep)
        {
            TimeToKeep = timeToKeep;
            ConnectionFactory = connectionFactory;
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
        /// Disable MARS.
        /// </summary>
        public void DisableMars()
        {
            UseMars = false;
        }

        /// <summary>
        /// Disable the table creation installer.
        /// </summary>
        public void DisableInstaller()
        {
            InstallerDisabled = true;
        }
    }
}