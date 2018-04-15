using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NServiceBus.Attachments.Sql
{
    public partial class Persister
    {
        /// <summary>
        /// Copies an attachment to <paramref name="target"/>.
        /// </summary>
        public virtual async Task CopyTo(string messageId, string name, SqlConnection connection, SqlTransaction transaction, Stream target, CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstNull(connection, nameof(connection));
            Guard.AgainstNull(target, nameof(target));
            using (var command = CreateGetDataCommand(messageId, name, connection, transaction))
            using (var reader = await command.ExecuteSequentialReader(cancellation).ConfigureAwait(false))
            {
                if (!await reader.ReadAsync(cancellation).ConfigureAwait(false))
                {
                    throw ThrowNotFound(messageId, name);
                }

                using (var data = reader.GetStream(1))
                {
                    await data.CopyToAsync(target, 81920, cancellation).ConfigureAwait(false);
                }
            }
        }
    }
}