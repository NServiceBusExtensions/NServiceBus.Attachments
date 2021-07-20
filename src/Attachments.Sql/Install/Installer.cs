using System.Data.Common;
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
        public static Task CreateTable(DbConnection connection, CancellationToken cancellation = default)
        {
            return CreateTable(connection, "MessageAttachments", cancellation);
        }

        /// <summary>
        /// Create the attachments storage table.
        /// </summary>
        public static async Task CreateTable(DbConnection connection, Table table, CancellationToken cancellation = default)
        {
            using var command = connection.CreateCommand();
            command.CommandText = GetTableSql();
            command.AddParameter("schema", table.Schema);
            command.AddParameter("table", table.TableName);
            await command.ExecuteNonQueryAsync(cancellation);
        }

        /// <summary>
        /// Get the sql used to create the attachments storage table.
        /// </summary>
        public static string GetTableSql()
        {
            using var stream = AssemblyHelper.Current.GetManifestResourceStream("Table.sql")!;
            using StreamReader streamReader = new(stream);
            return streamReader.ReadToEnd();
        }
    }
}