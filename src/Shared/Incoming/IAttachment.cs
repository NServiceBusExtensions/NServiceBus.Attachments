namespace NServiceBus.Attachments
#if FileShare
.FileShare
#endif
#if Sql
.Sql
#endif
#if Raw
.Raw
#endif
;

public interface IAttachment
{
    string Name { get; }
    IReadOnlyDictionary<string, string> Metadata { get; }
}