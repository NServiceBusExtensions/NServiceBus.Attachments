using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

static class SqlExtensions
{
    public static string GetStringOrNull(this SqlDataReader dataReader, int index)
    {
        if (dataReader.IsDBNull(index))
        {
            return null;
        }

        return dataReader.GetString(index);
    }

    public static void AddParameter(this IDbCommand command, string name, string value)
    {
        var parameter = command.CreateParameter();
        parameter.DbType = DbType.String;
        parameter.ParameterName = name;
        if (value == null)
        {
            parameter.Value = DBNull.Value;
        }
        else
        {
            parameter.Value = value;
        }

        command.Parameters.Add(parameter);
    }

    public static void AddBinary(this IDbCommand command, string name, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.DbType = DbType.Binary;
        parameter.Value = value;
        command.Parameters.Add(parameter);
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