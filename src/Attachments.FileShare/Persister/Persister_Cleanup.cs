namespace NServiceBus.Attachments.FileShare
#if Raw
    .Raw
#endif
{
    public partial class Persister
    {
        /// <inheritdoc />
        public virtual void CleanupItemsOlderThan(DateTime dateTime, CancellationToken cancellation = default)
        {
            foreach (var expiryFile in Directory.EnumerateFiles(fileShare, "*.expiry", SearchOption.AllDirectories))
            {
                if (cancellation.IsCancellationRequested)
                {
                    return;
                }

                var expiry = ParseExpiry(Path.GetFileNameWithoutExtension(expiryFile));
                if (expiry > dateTime)
                {
                    Directory.GetParent(expiryFile)!.Delete(true);
                }
            }
        }

        /// <inheritdoc />
        public virtual void PurgeItems(CancellationToken cancellation = default)
        {
            foreach (var expiryFile in Directory.EnumerateFiles(fileShare, "*.expiry", SearchOption.AllDirectories))
            {
                if (cancellation.IsCancellationRequested)
                {
                    return;
                }

                Directory.GetParent(expiryFile)!.Delete(true);
            }
        }
    }
}