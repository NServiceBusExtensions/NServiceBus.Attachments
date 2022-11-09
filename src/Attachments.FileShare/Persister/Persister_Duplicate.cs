// ReSharper disable ConvertClosureToMethodGroup
namespace NServiceBus.Attachments.FileShare
#if Raw
    .Raw
#endif
    ;

public partial class Persister
{
    /// <inheritdoc />
    public virtual Task<IReadOnlyCollection<string>> Duplicate(string sourceMessageId, string targetMessageId, CancellationToken cancellation = default)
    {
        Guard.AgainstNullOrEmpty(sourceMessageId, nameof(sourceMessageId));
        Guard.AgainstNullOrEmpty(targetMessageId, nameof(targetMessageId));
        var sourceDirectory = GetMessageDirectory(sourceMessageId);
        var targetDirectory = GetMessageDirectory(targetMessageId);
        FileHelpers.Copy(sourceDirectory, targetDirectory);
        var names = new List<string>(Directory.EnumerateDirectories(targetDirectory).Select(s => Path.GetFileName(s)));
        return Task.FromResult<IReadOnlyCollection<string>>(names);
    }

    /// <inheritdoc />
    public virtual Task Duplicate(string sourceMessageId, string name, string targetMessageId, string targetName, CancellationToken cancellation = default)
    {
        Guard.AgainstNullOrEmpty(sourceMessageId, nameof(sourceMessageId));
        Guard.AgainstNullOrEmpty(targetMessageId, nameof(targetMessageId));
        Guard.AgainstNullOrEmpty(targetName, nameof(targetName));
        Guard.AgainstNullOrEmpty(name, nameof(name));
        var sourceDirectory = GetAttachmentDirectory(sourceMessageId, name);
        var targetDirectory = GetAttachmentDirectory(targetMessageId, targetName);
        var sourceDataFile = GetDataFile(sourceDirectory);
        ThrowIfFileNotFound(sourceDataFile, sourceMessageId, name);
        FileHelpers.Copy(sourceDirectory, targetDirectory);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual Task Duplicate(string sourceMessageId, string name, string targetMessageId, CancellationToken cancellation = default)
    {
        Guard.AgainstNullOrEmpty(sourceMessageId, nameof(sourceMessageId));
        Guard.AgainstNullOrEmpty(targetMessageId, nameof(targetMessageId));
        Guard.AgainstNullOrEmpty(name, nameof(name));
        var sourceDirectory = GetAttachmentDirectory(sourceMessageId, name);
        var targetDirectory = GetAttachmentDirectory(targetMessageId, name);
        var sourceDataFile = GetDataFile(sourceDirectory);
        ThrowIfFileNotFound(sourceDataFile, sourceMessageId, name);
        FileHelpers.Copy(sourceDirectory, targetDirectory);
        return Task.CompletedTask;
    }
}