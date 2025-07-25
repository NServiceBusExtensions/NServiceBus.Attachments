﻿using Microsoft.Data.SqlClient;

namespace NServiceBus.Attachments.Sql
#if Raw
    .Raw
#endif
    ;

public partial class Persister
{
    /// <inheritdoc />
    public virtual async Task CopyTo(string messageId, string name, SqlConnection connection, SqlTransaction? transaction, Stream target, Cancel cancel = default)
    {
        Guard.AgainstNullOrEmpty(messageId);
        Guard.AgainstNullOrEmpty(name);
        Guard.AgainstLongAttachmentName(name);
        await using var command = CreateGetDataCommand(messageId, name, connection, transaction);
        await using var reader = await command.ExecuteReaderAsync(SequentialAccess, cancel);
        if (!await reader.ReadAsync(cancel))
        {
            throw ThrowNotFound(messageId, name);
        }

        await using var data = reader.GetStream(2);
        await data.CopyToAsync(target, 81920, cancel);
    }
}