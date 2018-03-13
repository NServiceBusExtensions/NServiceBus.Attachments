using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

class Persister
{
    string fileShare;

    public Persister(string fileShare)
    {
        this.fileShare = fileShare;
    }

    public Task SaveStream(string messageId, string name, DateTime expiry, Stream stream, CancellationToken cancellation = default)
    {
        return Save(messageId, name, expiry, fileStream => stream.CopyToAsync(fileStream, 4096, cancellation));
    }

    static Stream OpenRead(string attachmentFilePath)
    {
        return new FileStream(
            path: attachmentFilePath,
            mode: FileMode.Open,
            access: FileAccess.Read,
            share: FileShare.Read,
            bufferSize: bufferSize,
            useAsync: true);
    }

    static int bufferSize = 4096;
    static Stream OpenWrite(string attachmentFilePath)
    {
        return new FileStream(
            path: attachmentFilePath,
            mode: FileMode.CreateNew,
            access: FileAccess.Write,
            share: FileShare.None,
            bufferSize: bufferSize,
            useAsync: true);
    }

    async Task Save(string messageId, string name, DateTime expiry, Func<Stream, Task> action)
    {
        messageId = messageId.ToLowerInvariant();
        if (name == null)
        {
            name = "defalt";
        }

        var attachmentDirectory = GetAttachmentDirectory(messageId, name);
        if (Directory.Exists(attachmentDirectory))
        {
            throw ThrowExists(messageId, name);
        }

        Directory.CreateDirectory(attachmentDirectory);
        var dataFile = Path.Combine(attachmentDirectory, "data");
        expiry = expiry.ToUniversalTime();
        var expiryFile = Path.Combine(attachmentDirectory, $"{expiry:yyyy-MM-ddTHHmm}.expiry");
        using (File.Create(expiryFile))
        {
        }

        using (var fileStream = OpenWrite(dataFile))
        {
            await action(fileStream).ConfigureAwait(false);
        }
    }

    string GetAttachmentDirectory(string messageId, string name)
    {
        var messageDirectory = GetMessageDirectory(messageId);
        return Path.Combine(messageDirectory, name);
    }

    string GetMessageDirectory(string messageId)
    {
        return Path.Combine(fileShare, messageId);
    }

    public Task SaveBytes(string messageId, string name, DateTime expiry, byte[] bytes, CancellationToken cancellation = default)
    {
        return Save(messageId, name, expiry, fileStream => fileStream.WriteAsync(bytes,0, bytes.Length, cancellation));
    }

    public IEnumerable<ReadRow> ReadAllMetadata()
    {
        foreach (var messageDirectory in Directory.EnumerateDirectories(fileShare))
        {
            var messageId = Path.GetFileName(messageDirectory);
            foreach (var attachmentDirectory in Directory.EnumerateDirectories(messageDirectory))
            {
                var expiryFile = Directory.EnumerateFiles(attachmentDirectory,"*.expiry").Single();
                yield return new ReadRow(
                    messageId: messageId,
                    name: Path.GetFileName(attachmentDirectory),
                    expiry: ParseExpiry(Path.GetFileNameWithoutExtension(expiryFile)));
            }
        }
    }

    DateTime ParseExpiry(string value)
    {
        return DateTime.ParseExact(value, dateTimeFormat, null, DateTimeStyles.AssumeUniversal);
    }

    string dateTimeFormat = "yyyy-MM-ddTHHmm";

    public void DeleteAllRows()
    {
        foreach (var directory in Directory.EnumerateDirectories(fileShare))
        {
            Directory.Delete(directory, true);
        }
    }

    public void CleanupItemsOlderThan(DateTime dateTime, CancellationToken cancellation = default)
    {
        foreach (var expiryFile in Directory.EnumerateFiles(fileShare, "*.expiry", SearchOption.AllDirectories))
        {
            if (cancellation.IsCancellationRequested)
            {
                return;
            }
            var expiry = ParseExpiry(Path.GetFileNameWithoutExtension(expiryFile));
            if (expiry > dateTime)
            {
                Directory.GetParent(expiryFile).Delete(true);
            }
        }
    }

    public async Task CopyTo(string messageId, string name, Stream target, CancellationToken cancellation = default)
    {
        var dataFile = GetDataFile(messageId, name);
        using (var fileStream = OpenRead(dataFile))
        {
            await fileStream.CopyToAsync(target, bufferSize, cancellation).ConfigureAwait(false);
        }
    }

    public Task<Stream> OpenAttachmentStream(string messageId, string name)
    {
        var dataFile = GetDataFile(messageId, name);
        return Task.FromResult(OpenRead(dataFile));
    }

    string GetDataFile(string messageId, string name)
    {
        var attachmentDirectory = GetAttachmentDirectory(messageId, name);
        return Path.Combine(attachmentDirectory, "data");
    }

    public async Task<byte[]> GetBytes(string messageId, string name, CancellationToken cancellation = default)
    {
        var dataFile = GetDataFile(messageId, name);
        using (var fileStream = OpenRead(dataFile))
        {
            var bytes = new byte[fileStream.Length];
            await fileStream.ReadAsync(bytes, 0, (int)fileStream.Length, cancellation).ConfigureAwait(false);
            return bytes;
        }
    }

    public Task<Stream> GetStream(string messageId, string name)
    {
       return OpenAttachmentStream(messageId, name);
    }

    public async Task ProcessStreams(string messageId, Func<string, Stream, Task> action, CancellationToken cancellation = default)
    {
        var messageDirectory = GetMessageDirectory(messageId);
        foreach (var dataFile in Directory.EnumerateFiles(messageDirectory, "data", SearchOption.AllDirectories))
        {
            cancellation.ThrowIfCancellationRequested();
            var attachmentName = Directory.GetParent(dataFile).Name;
            using (var fileStream = OpenRead(dataFile))
            {
                await action(attachmentName, fileStream).ConfigureAwait(false);
            }
        }
    }

    public async Task ProcessStream(string messageId, string name, Func<Stream, Task> action)
    {
        var messageDirectory = GetAttachmentDirectory(messageId,name);
        foreach (var dataFile in Directory.EnumerateFiles(messageDirectory, "data", SearchOption.AllDirectories))
        {
            using (var fileStream = OpenRead(dataFile))
            {
                await action(fileStream).ConfigureAwait(false);
            }
        }
    }

    static Exception ThrowNotFound(string messageId, string name)
    {
        return new Exception($"Could not find attachment. MessageId:{messageId}, Name:{name}");
    }

    static Exception ThrowExists(string messageId, string name)
    {
        return new Exception($"Attachment already exists. MessageId:{messageId}, Name:{name}");
    }
}