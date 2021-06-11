using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

static class Extensions
{
    public static readonly UTF8Encoding Utf8WithBOM = new(true, false);

    public static Encoding Default(this Encoding? value)
    {
        if (value == null)
        {
            return Utf8WithBOM;
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