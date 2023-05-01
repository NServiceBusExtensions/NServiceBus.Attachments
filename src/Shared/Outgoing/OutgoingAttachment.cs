namespace NServiceBus.Attachments
#if FileShare
    .FileShare
#elif Sql
    .Sql
#endif
;

public readonly record struct OutgoingAttachment
{
    public required string Name { get; init; }
    public Encoding? Encoding { get; init; }
    public IReadOnlyDictionary<string, string>? Metadata { get; init; }
}