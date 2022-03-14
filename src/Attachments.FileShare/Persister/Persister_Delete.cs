namespace NServiceBus.Attachments.FileShare
#if Raw
    .Raw
#endif
    ;

public partial class Persister
{
    /// <inheritdoc />
    public virtual void DeleteAllAttachments() =>
        FileHelpers.PurgeDirectory(fileShare);
}