using System;
using System.IO;
using System.Threading;

namespace NServiceBus.Attachments.FileShare
{
    public partial class Persister
    {
        /// <summary>
        /// Deletes attachments older than <paramref name="dateTime"/>.
        /// </summary>
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
                    Directory.GetParent(expiryFile).Delete(true);
                }
            }
        }
    }
}