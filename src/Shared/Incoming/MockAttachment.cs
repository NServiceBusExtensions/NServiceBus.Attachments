class MockAttachment(string name, DateTime created, DateTime expiry, byte[] bytes, IReadOnlyDictionary<string, string> metadata)
{
    public string Name = name;
    public DateTime Created = created;
    public DateTime Expiry = expiry;
    public byte[] Bytes = bytes;
    public IReadOnlyDictionary<string, string> Metadata = metadata;
}