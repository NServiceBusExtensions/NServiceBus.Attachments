using System.Data.SqlClient;
using System.IO;

namespace NServiceBus.Attachments
{
    /// <summary>
    /// Used to take control over the storage table creation.
    /// </summary>
    public static class Installer
    {
        /// <summary>
        /// Create the attachments storage table.
        /// </summary>
        public static void CreateTable(SqlConnection connection, string schema = "dbo", string tableName = "Attachments")
        {
            Guard.AgainstNullOrEmpty(schema, nameof(schema));
            Guard.AgainstNullOrEmpty(tableName, nameof(tableName));
            Guard.AgainstSqlDelimiters(schema, nameof(schema));
            Guard.AgainstSqlDelimiters(tableName, nameof(tableName));
            using (var command = connection.CreateCommand())
            {
                command.CommandText = GetTableSql();
                command.Parameters.AddWithValue("schema", schema);
                command.Parameters.AddWithValue("tableName", tableName);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Get the sql used to create the attachments storage table.
        /// </summary>
        public static string GetTableSql()
        {
            using (var stream = AssemblyHelper.Current.GetManifestResourceStream($"{AssemblyHelper.Name}.Table.sql"))
            using (var streamReader = new StreamReader(stream))
            {
                return streamReader.ReadToEnd();
            }
        }
    }
}