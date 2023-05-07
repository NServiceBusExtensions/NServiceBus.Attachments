using Microsoft.Data.SqlClient;

namespace NServiceBus.Attachments.Sql
#if Raw
    .Raw
#endif
    ;

public partial class Persister
{
    /// <inheritdoc />
    public virtual async Task CopyTo(string messageId, string name, SqlConnection connection, SqlTransaction? transaction, Stream target, Cancellation cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId);
        Guard.AgainstNullOrEmpty(name);
        Guard.AgainstLongAttachmentName(name);
        using var command = CreateGetDataCommand(messageId, name, connection, transaction);
        using var reader = await command.ExecuteReaderAsync(SequentialAccess, cancellation);
        if (!await reader.ReadAsync(cancellation))
        {
            throw ThrowNotFound(messageId, name);
        }

        using var data = reader.GetStream(2);
        await data.CopyToAsync(target, 81920, cancellation);
    }
}