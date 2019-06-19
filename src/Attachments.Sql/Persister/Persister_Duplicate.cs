using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace NServiceBus.Attachments.Sql
#if Raw
    .Raw
#endif
{
    public partial class Persister
    {
        /// <summary>
        /// Copies an attachment to a different message.
        /// </summary>
        public virtual async Task Duplicate(string sourceMessageId, string name, SqlConnection connection, SqlTransaction transaction, string targetMessageId, CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(sourceMessageId, nameof(sourceMessageId));
            Guard.AgainstNullOrEmpty(targetMessageId, nameof(targetMessageId));
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstLongAttachmentName(name);
            Guard.AgainstNull(connection, nameof(connection));
            using (var command = CreateGetDuplicateCommand(sourceMessageId, name, targetMessageId, connection, transaction))
            {
                await command.ExecuteNonQueryAsync(cancellation);
            }
        }

        SqlCommand CreateGetDuplicateCommand(string sourceMessageId, string name,string targetMessageId, SqlConnection connection, SqlTransaction transaction)
        {
            var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = $@"
insert into {table}
(
    MessageId,
    Name,
    Expiry,
    Data,
    Metadata
)
select
    @TargetMessageId,
    Name,
    Expiry,
    Data,
    Metadata
from {table}
where MessageId = @SourceMessageId
";
            command.AddParameter("Name", name);
            command.AddParameter("SourceMessageId", sourceMessageId);
            command.AddParameter("TargetMessageId", targetMessageId);
            return command;
        }
    }
}