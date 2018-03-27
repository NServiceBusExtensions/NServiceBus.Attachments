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
        internal string TableName = "Attachments";

        internal AttachmentSettings(Func<Task<SqlConnection>> connectionFactory, GetTimeToKeep timeToKeep)
        {
            Guard.AgainstNull(connectionFactory, nameof(connectionFactory));
            TimeToKeep = timeToKeep;
            ConnectionFactory = connectionFactory;
        }

        internal AttachmentSettings(string connection, GetTimeToKeep timeToKeep)
        {
            Guard.AgainstNullOrEmpty(connection, nameof(connection));
            TimeToKeep = timeToKeep;
            ConnectionFactory = () => OpenConnection(connection);
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

        async Task<SqlConnection> OpenConnection(string connectionString)
        {
            var connection = new SqlConnection(connectionString);
            try
            {
                await connection.OpenAsync().ConfigureAwait(false);
                return connection;
            }
            catch
            {
                connection.Dispose();
                throw;
            }
        }
    }
}