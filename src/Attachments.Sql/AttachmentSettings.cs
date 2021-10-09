using System.Data.Common;
using NServiceBus.Transport;

namespace NServiceBus.Attachments.Sql
{
    /// <summary>
    /// All settings for attachments
    /// </summary>
    public partial class AttachmentSettings
    {
        internal Func<Task<DbConnection>> ConnectionFactory;
        internal Table Table = "MessageAttachments";
        internal bool InstallerDisabled;
        internal bool UseTransport;
        internal bool UseSynchronizedStorage;

        internal AttachmentSettings(Func<Task<DbConnection>> connectionFactory, GetTimeToKeep timeToKeep)
        {
            TimeToKeep = timeToKeep;
            ConnectionFactory = connectionFactory;
        }

        /// <summary>
        /// Use the ambient <see cref="TransportTransaction"/> to obtain a <see cref="DbConnection"/> or <see cref="DbTransaction"/>.
        /// </summary>
        public void UseTransportConnectivity()
        {
            UseTransport = true;
        }

        /// <summary>
        /// Use the ambient <see cref="IMessageHandlerContext.SynchronizedStorageSession"/> to obtain a <see cref="DbConnection"/> or <see cref="DbTransaction"/>.
        /// </summary>
        public void UseSynchronizedStorageSessionConnectivity()
        {
            UseSynchronizedStorage = true;
        }

        /// <summary>
        /// Use a specific <paramref name="table"/> for attachments.
        /// </summary>
        public void UseTable(Table table)
        {
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