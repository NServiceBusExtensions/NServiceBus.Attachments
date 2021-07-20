using System.Data.Common;
using System.IO;
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
        public virtual async Task CopyTo(string messageId, string name, DbConnection connection, DbTransaction? transaction, Stream target, CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstLongAttachmentName(name);
            using var command = CreateGetDataCommand(messageId, name, connection, transaction);
            using var reader = await command.ExecuteSequentialReader(cancellation);
            if (!await reader.ReadAsync(cancellation))
            {
                throw ThrowNotFound(messageId, name);
            }

            using var data = reader.GetStream(2);
            await data.CopyToAsync(target, 81920, cancellation);
        }
    }
}