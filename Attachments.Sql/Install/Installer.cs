using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NServiceBus.Attachments.Sql
#if Raw
    .Raw
#endif
{
    /// <summary>
    /// Used to take control over the storage table creation.
    /// </summary>
    public static class Installer
    {
        /// <summary>
        /// Create the attachments storage table.
        /// </summary>
        public static Task CreateTable(string connection, CancellationToken cancellation = default)
        {
            return CreateTable(connection, "MessageAttachments", cancellation);
        }

        /// <summary>
        /// Create the attachments storage table.
        /// </summary>
        public static async Task CreateTable(string connection, Table table, CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(connection, nameof(connection));
            using (var sqlConnection = new SqlConnection(connection))
            {
                await sqlConnection.OpenAsync(cancellation).ConfigureAwait(false);
                await CreateTable(sqlConnection, table, cancellation).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Create the attachments storage table.
        /// </summary>
        public static Task CreateTable(SqlConnection connection, CancellationToken cancellation = default)
        {
            return CreateTable(connection, "MessageAttachments", cancellation);
        }

        /// <summary>
        /// Create the attachments storage table.
        /// </summary>
        public static async Task CreateTable(SqlConnection connection, Table table, CancellationToken cancellation = default)
        {
            Guard.AgainstNull(connection, nameof(connection));
            Guard.AgainstNull(table, nameof(table));
            using (var command = connection.CreateCommand())
            {
                command.CommandText = GetTableSql();
                command.AddParameter("schema", table.Schema);
                command.AddParameter("table", table.TableName);
                await command.ExecuteNonQueryAsync(cancellation).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Get the sql used to create the attachments storage table.
        /// </summary>
        public static string GetTableSql()
        {
            using (var stream = AssemblyHelper.Current.GetManifestResourceStream("Table.sql"))
            using (var streamReader = new StreamReader(stream))
            {
                return streamReader.ReadToEnd();
            }
        }
    }
}