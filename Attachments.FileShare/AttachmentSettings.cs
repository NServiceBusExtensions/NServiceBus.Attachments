namespace NServiceBus.Attachments.FileShare
{
    /// <summary>
    /// All settings for attachments
    /// </summary>
    public partial class AttachmentSettings
    {
        internal string FileShare;

        internal AttachmentSettings(string fileShare, GetTimeToKeep timeToKeep)
        {
            Guard.AgainstNullOrEmpty(fileShare, nameof(fileShare));
            FileShare = fileShare;
            TimeToKeep = timeToKeep;
        }
    }
}