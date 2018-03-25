using System.IO;
using System.Threading;
using System.Threading.Tasks;

static class FileHelpers
{
    public static Stream OpenWrite(string path)
    {
        return new FileStream(
            path: path,
            mode: FileMode.CreateNew,
            access: FileAccess.Write,
            share: FileShare.None,
            bufferSize: bufferSize,
            useAsync: true);
    }

    public static void PurgeDirectory(string directory)
    {
        foreach (var subDirectory in Directory.EnumerateDirectories(directory))
        {
            Directory.Delete(subDirectory, true);
        }
    }

    public static Stream OpenRead(string path)
    {
        return new FileStream(
            path: path,
            mode: FileMode.Open,
            access: FileAccess.Read,
            share: FileShare.Read,
            bufferSize: bufferSize,
            useAsync: true);
    }

    static int bufferSize = 4096;

    public static async Task CopyTo(Stream target, CancellationToken cancellation, string dataFile)
    {
        using (var fileStream = OpenRead(dataFile))
        {
            await fileStream.CopyToAsync(target, bufferSize, cancellation).ConfigureAwait(false);
        }
    }

    public static async Task<byte[]> ReadBytes(CancellationToken cancellation, string dataFile)
    {
        using (var fileStream = OpenRead(dataFile))
        {
            var bytes = new byte[fileStream.Length];
            await fileStream.ReadAsync(bytes, 0, (int) fileStream.Length, cancellation).ConfigureAwait(false);
            return bytes;
        }
    }
}