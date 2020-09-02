using System.IO;
using System.Text;

static class IoExtensions
{
    public static readonly Encoding Utf8NoBOM = new UTF8Encoding(false, true);

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
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream, encoding);
        writer.Write(value);
        writer.Flush();
        stream.Position = 0;
        var array = stream.ToArray();
        return array;
    }
}