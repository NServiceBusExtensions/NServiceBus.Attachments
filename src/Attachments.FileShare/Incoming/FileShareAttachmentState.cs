using NServiceBus.Attachments.FileShare;

class FileShareAttachmentState
{
    public readonly IPersister Persister;

    public FileShareAttachmentState(IPersister persister) =>
        Persister = persister;
}