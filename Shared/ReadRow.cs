using System;

class ReadRow
{
    public readonly string MessageId;
    public readonly string Name;
    public readonly DateTime Expiry;

    public ReadRow(string messageId, string name, DateTime expiry)
    {
        MessageId = messageId;
        Name = name;
        Expiry = expiry;
    }
}