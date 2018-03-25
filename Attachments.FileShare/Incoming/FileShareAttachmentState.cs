using NServiceBus.Attachments.FileShare;

class FileShareAttachmentState
{
    public readonly Persister Persister;

    public FileShareAttachmentState(Persister persister)
    {
        Persister = persister;
    }
}