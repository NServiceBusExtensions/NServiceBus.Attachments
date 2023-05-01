namespace NServiceBus.Attachments
#if FileShare
    .FileShare
#elif Sql
    .Sql
#endif
;

public readonly record struct AttachmentToAdd
{
    public required string Name { get; init; }
    public required Stream Stream { get; init; }
    public IReadOnlyDictionary<string, string>? Metadata { get; init; }
}