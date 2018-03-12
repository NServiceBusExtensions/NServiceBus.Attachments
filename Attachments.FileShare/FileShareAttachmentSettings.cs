using NServiceBus.Attachments.FileShare;

namespace NServiceBus
{
    /// <summary>
    /// All settings for attachments
    /// </summary>
    public class FileShareAttachmentSettings
    {
        internal bool RunCleanTask = true;
        internal bool InstallerDisabled;
        internal string FileShare;
        internal GetTimeToKeep TimeToKeep;

        internal FileShareAttachmentSettings(string fileShare, GetTimeToKeep timeToKeep)
        {
            Guard.AgainstNullOrEmpty(fileShare, nameof(fileShare));
            FileShare = fileShare;
            TimeToKeep = timeToKeep;
        }

        /// <summary>
        /// Disable the attachment cleanup task.
        /// </summary>
        public void DisableCleanupTask()
        {
            RunCleanTask = false;
        }

        /// <summary>
        /// Disable the table creation installer.
        /// </summary>
        public void DisableInstaller()
        {
            InstallerDisabled = true;
        }
    }
}