using System.IO;
using System.Text;

static class IoExtensions
{
    public static readonly UTF8Encoding Utf8NoBOM = new(false, true);

    public static Encoding Default(this Encoding? value)
    {
        if (value == null)
        {
            return Utf8NoBOM;
        }

        return value;
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