using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

static class SqlExtenstions
{
    public static string GetStringOrNull(this SqlDataReader dataReader, int index)
    {
        if (dataReader.IsDBNull(index))
        {
            return null;
        }

        return dataReader.GetString(index);
    }

    public static void AddParameter(this SqlCommand command, string name, string value)
    {
        var parameter = command.Parameters.Add(name, SqlDbType.NVarChar);
        if (value != null)
        {
            parameter.Value = value;
        }
        else
        {
            parameter.Value = DBNull.Value;
        }
    }

    public static void AddBinary(this SqlCommand command, string name, object value)
    {
        command.Parameters.Add(name, SqlDbType.Binary, -1).Value = value;
    }

    public static void AddParameter(this SqlCommand command, string name, DateTime value)
    {
        command.Parameters.Add(name, SqlDbType.DateTime2).Value = value;
    }

    // The reader needs to be executed with SequentialAccess to enable network streaming
    // Otherwise ReadAsync will buffer the entire BLOB in memory which can cause scalability issues or OutOfMemoryExceptions
    public static Task<SqlDataReader> ExecuteSequentialReader(this SqlCommand command, CancellationToken cancellation = default)
    {
        return command.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellation);
    }
}