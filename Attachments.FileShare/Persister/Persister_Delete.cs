namespace NServiceBus.Attachments.FileShare
#if Raw
    .Raw
#endif
{
    public partial class Persister
    {
        /// <summary>
        /// Deletes all attachments.
        /// </summary>
        public virtual void DeleteAllAttachments()
        {
            FileHelpers.PurgeDirectory(fileShare);
        }
    }
}