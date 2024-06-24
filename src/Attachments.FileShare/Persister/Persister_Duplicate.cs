// ReSharper disable ConvertClosureToMethodGroup
namespace NServiceBus.Attachments.FileShare
#if Raw
    .Raw
#endif
    ;

public partial class Persister
{
    /// <inheritdoc />
    public virtual Task<IReadOnlyList<string>> Duplicate(string sourceMessageId, string targetMessageId, Cancel cancel = default)
    {
        Guard.AgainstNullOrEmpty(sourceMessageId);
        Guard.AgainstNullOrEmpty(targetMessageId);
        var sourceDirectory = GetMessageDirectory(sourceMessageId);
        var targetDirectory = GetMessageDirectory(targetMessageId);
        FileHelpers.Copy(sourceDirectory, targetDirectory);
        var names = new List<string>(Directory.EnumerateDirectories(targetDirectory).Select(s => Path.GetFileName(s)));
        return Task.FromResult<IReadOnlyList<string>>(names);
    }

    /// <inheritdoc />
    public virtual Task Duplicate(string sourceMessageId, string name, string targetMessageId, string targetName, Cancel cancel = default)
    {
        Guard.AgainstNullOrEmpty(sourceMessageId);
        Guard.AgainstNullOrEmpty(targetMessageId);
        Guard.AgainstNullOrEmpty(targetName);
        Guard.AgainstNullOrEmpty(name);
        var sourceDirectory = GetAttachmentDirectory(sourceMessageId, name);
        var targetDirectory = GetAttachmentDirectory(targetMessageId, targetName);
        var sourceDataFile = GetDataFile(sourceDirectory);
        ThrowIfFileNotFound(sourceDataFile, sourceMessageId, name);
        FileHelpers.Copy(sourceDirectory, targetDirectory);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual Task Duplicate(string sourceMessageId, string name, string targetMessageId, Cancel cancel = default)
    {
        Guard.AgainstNullOrEmpty(sourceMessageId);
        Guard.AgainstNullOrEmpty(targetMessageId);
        Guard.AgainstNullOrEmpty(name);
        var sourceDirectory = GetAttachmentDirectory(sourceMessageId, name);
        var targetDirectory = GetAttachmentDirectory(targetMessageId, name);
        var sourceDataFile = GetDataFile(sourceDirectory);
        ThrowIfFileNotFound(sourceDataFile, sourceMessageId, name);
        FileHelpers.Copy(sourceDirectory, targetDirectory);
        return Task.CompletedTask;
    }
}