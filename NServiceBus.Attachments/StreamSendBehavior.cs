using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using NServiceBus.Attachments;
using NServiceBus.DeliveryConstraints;
using NServiceBus.Performance.TimeToBeReceived;
using NServiceBus.Pipeline;

class StreamSendBehavior :
    Behavior<IOutgoingLogicalMessageContext>
{
    Func<SqlConnection> connectionBuilder;

    public StreamSendBehavior(Func<SqlConnection> connectionBuilder)
    {
        this.connectionBuilder = connectionBuilder;
    }

    public override async Task Invoke(IOutgoingLogicalMessageContext context, Func<Task> next)
    {
        var extensions = context.Extensions;
        if (!extensions.TryGet<OutgoingAttachments>(out var attachments))
        {
            return;
        }

        var timeToBeReceived = TimeSpan.MaxValue;

        if (extensions.TryGetDeliveryConstraint<DiscardIfNotReceivedBefore>(out var constraint))
        {
            timeToBeReceived = constraint.MaxTime;
        }

        using (var sqlConnection = connectionBuilder())
        {
            await sqlConnection.OpenAsync();
            foreach (var attachmentsStream in attachments.Streams)
            {
                var outgoingStream = attachmentsStream.Value;
                var streamTimeToKeep = outgoingStream.TimeToKeep(timeToBeReceived);
                var stream = outgoingStream.Func();
                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = @"
INSERT INTO Attachments
(
    MessageId,
    Name,
    Expiry,
    Data
)
VALUES
(
    @MessageId,
    @Name,
    @Expiry,
    @Data
)";
                    var parameters = command.Parameters;
                    parameters.Add("@MessageId", SqlDbType.NVarChar).Value = context.MessageId;
                    parameters.Add("@Name", SqlDbType.NVarChar).Value = attachmentsStream.Key;
                    parameters.Add("@Expiry", SqlDbType.DateTime).Value = DateTime.UtcNow.Add(streamTimeToKeep);
                    parameters.Add("@Data", SqlDbType.Binary, -1).Value = stream;

                    // Send the data to the server asynchronously
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        await next()
            .ConfigureAwait(false);
    }
}