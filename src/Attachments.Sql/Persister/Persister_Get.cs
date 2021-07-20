using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NServiceBus.Attachments.Sql
#if Raw
    .Raw
#endif
{
    public partial class Persister
    {
        /// <inheritdoc />
        public virtual async Task<AttachmentString> GetString(string messageId, string name, DbConnection connection, DbTransaction? transaction, Encoding? encoding = null, CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstLongAttachmentName(name);
            encoding = encoding.Default();
            using var command = CreateGetDataCommand(messageId, name, connection, transaction);
            using var reader = await command.ExecuteSequentialReader(cancellation);
            if (await reader.ReadAsync(cancellation))
            {
                var metadataString = reader.GetStringOrNull(1);
                var metadata = MetadataSerializer.Deserialize(metadataString);
                //TODO: read string directly
                var bytes = (byte[]) reader[2];
                return new AttachmentString(name, encoding.GetString(bytes), metadata);
            }
            throw ThrowNotFound(messageId, name);
        }

        /// <inheritdoc />
        public virtual async Task<AttachmentBytes> GetBytes(string messageId, string name, DbConnection connection, DbTransaction? transaction, CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstLongAttachmentName(name);
            using var command = CreateGetDataCommand(messageId, name, connection, transaction);
            using var reader = await command.ExecuteSequentialReader(cancellation);
            if (await reader.ReadAsync(cancellation))
            {
                var metadataString = reader.GetStringOrNull(1);
                var metadata = MetadataSerializer.Deserialize(metadataString);
                var bytes = (byte[]) reader[2];

                return new AttachmentBytes(name, bytes, metadata);
            }
            throw ThrowNotFound(messageId, name);
        }

        /// <inheritdoc />
        public virtual async Task<AttachmentStream> GetStream(
            string messageId,
            string name,
            DbConnection connection,
            DbTransaction? transaction,
            bool disposeConnectionOnStreamDispose,
            CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstLongAttachmentName(name);
            DbCommand? command = null;
            DbDataReader? reader = null;
            try
            {
                command = CreateGetDataCommand(messageId, name, connection, transaction);
                reader = await command.ExecuteSequentialReader(cancellation);
                if (!await reader.ReadAsync(cancellation))
                {
                    #if NETSTANDARD2_1
                    await reader.DisposeAsync();
                    await command.DisposeAsync();
                    #else
                    reader.Dispose();
                    command.Dispose();
                    #endif
                    throw ThrowNotFound(messageId, name);
                }

                return InnerGetStream(name, reader, command, disposeConnectionOnStreamDispose);
            }
            catch (Exception)
            {
#if NETSTANDARD2_1
                if (reader != null)
                {
                    await reader.DisposeAsync();
                }
                if (command != null)
                {
                    await command.DisposeAsync();
                }
#else
                reader?.Dispose();
                command?.Dispose();
#endif
                throw;
            }
        }

        /// <inheritdoc />
        public virtual async IAsyncEnumerable<AttachmentStream> GetStreams(
            string messageId,
            DbConnection connection,
            DbTransaction? transaction,
            [EnumeratorCancellation] CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            using var command = CreateGetDatasCommand(messageId, connection, transaction);
            using var reader = await command.ExecuteSequentialReader(cancellation);
            while (await reader.ReadAsync(cancellation))
            {
                cancellation.ThrowIfCancellationRequested();
                var name = reader.GetString(0);
                var length = reader.GetInt64(1);
                var metadata = MetadataSerializer.Deserialize(reader.GetStringOrNull(2));
                using var sqlStream = reader.GetStream(3);
                yield return new AttachmentStream(name, sqlStream, length, metadata);
            }
        }

        /// <inheritdoc />
        public virtual async IAsyncEnumerable<AttachmentBytes> GetBytes(
            string messageId,
            DbConnection connection,
            DbTransaction? transaction,
            [EnumeratorCancellation] CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            using var command = CreateGetDatasCommand(messageId, connection, transaction);
            using var reader = await command.ExecuteSequentialReader(cancellation);
            while (await reader.ReadAsync(cancellation))
            {
                cancellation.ThrowIfCancellationRequested();
                var name = reader.GetString(0);
                var metadata = MetadataSerializer.Deserialize(reader.GetStringOrNull(2));
                var bytes = (byte[]) reader[3];
                yield return new AttachmentBytes(name, bytes, metadata);
            }
        }

        /// <inheritdoc />
        public virtual async IAsyncEnumerable<AttachmentString> GetStrings(string messageId, DbConnection connection, DbTransaction? transaction, Encoding? encoding = null, [EnumeratorCancellation] CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            encoding = encoding.Default();
            using var command = CreateGetDatasCommand(messageId, connection, transaction);
            using var reader = await command.ExecuteSequentialReader(cancellation);
            while (await reader.ReadAsync(cancellation))
            {
                cancellation.ThrowIfCancellationRequested();
                var name = reader.GetString(0);
                var metadata = MetadataSerializer.Deserialize(reader.GetStringOrNull(2));
                //TODO: read string directly
                var bytes = (byte[]) reader[3];
                yield return new AttachmentString(name, encoding.GetString(bytes), metadata);
            }
        }

        static AttachmentStream InnerGetStream(string name, DbDataReader reader, DbCommand command, bool disposeConnection)
        {
            var length = reader.GetInt64(0);
            var metadataString = reader.GetStringOrNull(1);
            var sqlStream = reader.GetStream(2);
            var metadata = MetadataSerializer.Deserialize(metadataString);
            if (disposeConnection)
            {
                return new(name, sqlStream, length, metadata, command, reader, command.Connection);
            }
            return new(name, sqlStream, length, metadata, command, reader);
        }

        DbCommand CreateGetDataCommand(string messageId, string name, DbConnection connection, DbTransaction? transaction)
        {
            var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = $@"
select
    datalength(Data),
    Metadata,
    Data
from {table}
where
    NameLower = lower(@Name) and
    MessageIdLower = lower(@MessageId)";
            command.AddParameter("Name", name);
            command.AddParameter("MessageId", messageId);
            return command;
        }

        DbCommand CreateGetDatasCommand(string messageId, DbConnection connection, DbTransaction? transaction)
        {
            var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = $@"
select
    Name,
    datalength(Data),
    Metadata,
    Data
from {table}
where
    MessageIdLower = lower(@MessageId)";
            command.AddParameter("MessageId", messageId);
            return command;
        }
    }
}