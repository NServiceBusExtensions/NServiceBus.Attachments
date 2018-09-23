using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using NServiceBus.Transport;

namespace NServiceBus.Attachments.Sql
{
    /// <summary>
    /// All settings for attachments
    /// </summary>
    public partial class AttachmentSettings
    {
        internal Func<Task<SqlConnection>> ConnectionFactory;
        internal Table Table = "MessageAttachments";
        internal bool InstallerDisabled;
        internal bool UseTransportSqlConnectivity;

        internal AttachmentSettings(Func<Task<SqlConnection>> connectionFactory, GetTimeToKeep timeToKeep)
        {
            TimeToKeep = timeToKeep;
            ConnectionFactory = connectionFactory;
        }

        /// <summary>
        /// Use the ambient <see cref="TransportTransaction"/> to obtain a <see cref="SqlConnection"/> or <see cref="SqlTransaction"/>.
        /// </summary>
        public void UseTransportConnectivity()
        {
            UseTransportSqlConnectivity = true;
        }

        /// <summary>
        /// Use a specific <paramref name="table"/> for attachments.
        /// </summary>
        public void UseTable(Table table)
        {
            Guard.AgainstNull(table, nameof(table));
            Table = table;
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