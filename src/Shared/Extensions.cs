static class Extensions
{
    public static readonly UTF8Encoding Utf8NoBOM = new(false, true);

    public static Encoding Default(this Encoding? value)
    {
        if (value is null)
        {
            return Utf8NoBOM;
        }

        return value;
    }

    public static async Task<List<T>> ToAsyncList<T>(this IAsyncEnumerable<T> enumerable)
    {
        List<T> list = new();
        await foreach (var item in enumerable)
        {
            list.Add(item);
        }

        return list;
    }
    
    public static void MoveToStart(this Stream stream)
    {
        if (stream.CanSeek)
        {
            stream.Position = 0;
        }
    }

    public static byte[] ToBytes(this string value, Encoding encoding)
    {
        using MemoryStream stream = new();
        using StreamWriter writer = new(stream, encoding);
        writer.Write(value);
        writer.Flush();
        stream.Position = 0;
        var array = stream.ToArray();
        return array;
    }
}