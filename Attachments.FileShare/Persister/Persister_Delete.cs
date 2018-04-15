namespace NServiceBus.Attachments.FileShare
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