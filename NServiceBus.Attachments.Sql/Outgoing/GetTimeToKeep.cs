using System;

namespace NServiceBus.Attachments.Sql
{
    /// <summary>
    /// Defines a contract for getting a <see cref="TimeSpan"/> to keep an attachment.
    /// </summary>
    public delegate TimeSpan GetTimeToKeep(TimeSpan? messageTimeToBeReceived);
}