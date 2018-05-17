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
        internal string Schema = "[dbo]";
        internal string Table = "[MessageAttachments]";
        internal bool InstallerDisabled;

        internal AttachmentSettings(Func<Task<SqlConnection>> connectionFactory, GetTimeToKeep timeToKeep)
        {
            TimeToKeep = timeToKeep;
            ConnectionFactory = connectionFactory;
        }

        /// <summary>
        /// Use a specific <paramref name="table"/> and <paramref name="schema"/> for the attachments table.
        /// </summary>
        public void UseTable(string table, string schema = "dbo")
        {
            UseTable(table, schema, true);
        }

        /// <summary>
        /// Use a specific <paramref name="table"/> and <paramref name="schema"/> for the attachments table.
        /// </summary>
        public void UseTable(string table, string schema, bool sanitize)
        {
            Guard.AgainstNullOrEmpty(table, nameof(table));
            Guard.AgainstNullOrEmpty(schema, nameof(schema));
            if (sanitize)
            {
                Table = SqlSanitizer.Sanitize(table);
                Schema = SqlSanitizer.Sanitize(schema);
            }
            else
            {
                Table = table;
                Schema = schema;
            }
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