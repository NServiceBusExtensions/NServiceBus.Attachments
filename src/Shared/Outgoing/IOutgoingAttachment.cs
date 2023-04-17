namespace NServiceBus.Attachments

#if FileShare
    .FileShare
#elif Sql
    .Sql
#endif
;
public interface IOutgoingAttachment
{
    Encoding? Encoding { get; init; }
    IReadOnlyDictionary<string, string>? Metadata { get; init; }
}