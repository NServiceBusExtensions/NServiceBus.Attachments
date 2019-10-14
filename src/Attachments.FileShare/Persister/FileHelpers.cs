using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

static class FileHelpers
{
    static FileOptions fileOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;
    static int bufferSize = 1024 * 64;
    public static FileStream OpenWrite(string path)
    {
        return new FileStream(
            path: path,
            mode: FileMode.CreateNew,
            access: FileAccess.Write,
            share: FileShare.None,
            bufferSize: bufferSize,
            options: fileOptions);
    }

    public static StreamWriter BuildLeaveOpenWriter(this Stream input)
    {
        return new StreamWriter(input, Encoding.UTF8, 1024, leaveOpen: true);
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
            options: fileOptions);
    }

    public static async Task CopyTo(Stream target, CancellationToken cancellation, string dataFile)
    {
        await using var fileStream = OpenRead(dataFile);
        await fileStream.CopyToAsync(target, bufferSize, cancellation);
    }

    public static void Copy(string sourceDirectory, string targetDirectory)
    {
        CopyAll(new DirectoryInfo(sourceDirectory), new DirectoryInfo(targetDirectory));
    }

    public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
    {
        Directory.CreateDirectory(target.FullName);

        // Copy each file into the new directory.
        foreach (var file in source.GetFiles())
        {
            file.CopyTo(Path.Combine(target.FullName, file.Name), true);
        }

        // Copy each subdirectory using recursion.
        foreach (var directory in source.GetDirectories())
        {
            var nextTargetSubDir = target.CreateSubdirectory(directory.Name);
            CopyAll(directory, nextTargetSubDir);
        }
    }

    public static async Task<byte[]> ReadBytes(CancellationToken cancellation, string dataFile)
    {
        await using var fileStream = OpenRead(dataFile);
        var bytes = new byte[fileStream.Length];
        await fileStream.ReadAsync(bytes, 0, (int) fileStream.Length, cancellation);
        return bytes;
    }
}