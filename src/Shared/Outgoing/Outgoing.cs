namespace NServiceBus.Attachments
#if FileShare
.FileShare
#endif
#if Sql
.Sql
#endif
;

class Outgoing
{
    public Outgoing(IReadOnlyDictionary<string, string>? metadata, GetTimeToKeep? timeToKeep, Action? cleanup, Encoding? encoding)
    {
        Metadata = metadata;
        TimeToKeep = timeToKeep;
        Cleanup = cleanup;
        Encoding = encoding;
    }

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