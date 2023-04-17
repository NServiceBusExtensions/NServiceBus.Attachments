namespace NServiceBus.Attachments
#if FileShare
    .FileShare
#elif Sql
    .Sql
#endif
;

/// <summary>
/// Attachment info for testing purposes.
/// </summary>
public readonly record struct OutgoingAttachment
{
    public string Name { get; init; }
    public Encoding? Encoding { get; init; }
    public IReadOnlyDictionary<string, string>? Metadata { get; init; }
}