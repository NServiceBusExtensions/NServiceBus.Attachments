namespace NServiceBus.Attachments
#if FileShare
    .FileShare
#elif Sql
    .Sql
#endif
;

class Outgoing
{
    public Encoding? Encoding { get; init; }
    public Func<Task<Stream>>? AsyncStreamFactory { get; init; }
    public Func<Stream>? StreamFactory { get; init; }
    public Stream? StreamInstance { get; init; }
    public Func<Task<byte[]>>? AsyncBytesFactory { get; init; }
    public Func<byte[]>? BytesFactory { get; init; }
    public byte[]? BytesInstance { get; init; }
    public string? StringInstance { get; init; }
    public GetTimeToKeep? TimeToKeep { get; init; }
    public Action? Cleanup { get; init; }
    public IReadOnlyDictionary<string, string>? Metadata { get; init; }
}