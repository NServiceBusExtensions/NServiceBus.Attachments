using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

static class SqlExtensions
{
    public static string? GetStringOrNull(this IDataReader dataReader, int index)
    {
        if (dataReader.IsDBNull(index))
        {
            return null;
        }

        return dataReader.GetString(index);
    }

    public static void AddParameter(this DbCommand command, string name, string? value)
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

    public static void AddBinary(this DbCommand command, string name, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.DbType = DbType.Binary;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }

    public static void AddParameter(this DbCommand command, string name, DateTime value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.DbType = DbType.DateTime2;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }

    // The reader needs to be executed with SequentialAccess to enable network streaming
    // Otherwise ReadAsync will buffer the entire BLOB in memory which can cause scalability issues or OutOfMemoryExceptions
    public static Task<DbDataReader> ExecuteSequentialReader(this DbCommand command, CancellationToken cancellation = default)
    {
        return command.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellation);
    }
}