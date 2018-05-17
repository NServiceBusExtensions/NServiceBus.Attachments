using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NServiceBus.Attachments.Sql
{
    /// <summary>
    /// Used to take control over the storage table creation.
    /// </summary>
    public static class Installer
    {
        /// <summary>
        /// Create the attachments storage table.
        /// </summary>
        public static Task CreateTable(SqlConnection connection, string schema = "dbo", string table = "MessageAttachments", CancellationToken cancellation = default)
        {
            return CreateTable(connection, schema, table, true, cancellation);
        }

        /// <summary>
        /// Create the attachments storage table.
        /// </summary>
        public static async Task CreateTable(SqlConnection connection, string schema, string table, bool sanitize, CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(schema, nameof(schema));
            Guard.AgainstNullOrEmpty(table, nameof(table));
            if (sanitize)
            {
                table = SqlSanitizer.Sanitize(table);
                schema = SqlSanitizer.Sanitize(schema);
            }
            using (var command = connection.CreateCommand())
            {
                command.CommandText = GetTableSql();
                command.AddParameter("schema", schema);
                command.AddParameter("table", table);
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