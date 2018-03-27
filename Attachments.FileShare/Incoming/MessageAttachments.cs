using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus.Attachments.FileShare;

class MessageAttachments : IMessageAttachments
{
    string messageId;
    IPersister persister;

    internal MessageAttachments(string messageId, IPersister persister)
    {
        this.messageId = messageId;
        this.persister = persister;
    }

    public Task CopyTo(Stream target, CancellationToken cancellation = default)
    {
        Guard.AgainstNull(target, nameof(target));
        return persister.CopyTo(messageId, "default", target, cancellation);
    }

    public Task CopyTo(string name, Stream target, CancellationToken cancellation = default)
    {
        Guard.AgainstNull(name, nameof(name));
        Guard.AgainstNull(target, nameof(target));
        return persister.CopyTo(messageId, name, target, cancellation);
    }

    public Task ProcessStream(Func<Stream, Task> action, CancellationToken cancellation = default)
    {
        Guard.AgainstNull(action, nameof(action));
        return persister.ProcessStream(messageId, "default", action, cancellation);
    }

    public Task ProcessStream(string name, Func<Stream, Task> action, CancellationToken cancellation = default)
    {
        Guard.AgainstNull(name, nameof(name));
        Guard.AgainstNull(action, nameof(action));
        return persister.ProcessStream(messageId, name, action, cancellation);
    }

    public Task ProcessStreams(Func<string, Stream, Task> action, CancellationToken cancellation = default)
    {
        Guard.AgainstNull(action, nameof(action));
        return persister.ProcessStreams(messageId, action, cancellation);
    }

    public Task<byte[]> GetBytes(CancellationToken cancellation = default)
    {
        return persister.GetBytes(messageId, "default", cancellation);
    }

    public Task<byte[]> GetBytes(string name, CancellationToken cancellation = default)
    {
        Guard.AgainstNull(name, nameof(name));
        return persister.GetBytes(messageId, name, cancellation);
    }

    public Stream GetStream()
    {
        return persister.GetStream(messageId, "default");
    }

    public Stream GetStream(string name)
    {
        Guard.AgainstNull(name, nameof(name));
        return persister.GetStream(messageId, name);
    }

    public Task CopyToForMessage(string messageId, Stream target, CancellationToken cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
        Guard.AgainstNull(target, nameof(target));
        return persister.CopyTo(messageId, "default", target, cancellation);
    }

    public Task CopyToForMessage(string messageId, string name, Stream target, CancellationToken cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
        Guard.AgainstNull(name, nameof(name));
        Guard.AgainstNull(target, nameof(target));
        return persister.CopyTo(messageId, name, target, cancellation);
    }

    public Task ProcessStreamForMessage(string messageId, Func<Stream, Task> action, CancellationToken cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
        Guard.AgainstNull(action, nameof(action));
        return persister.ProcessStream(messageId, "default", action, cancellation);
    }

    public Task ProcessStreamForMessage(string messageId, string name, Func<Stream, Task> action, CancellationToken cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
        Guard.AgainstNull(name, nameof(name));
        Guard.AgainstNull(action, nameof(action));
        return persister.ProcessStream(messageId, name, action, cancellation);
    }

    public Task ProcessStreamsForMessage(string messageId, Func<string, Stream, Task> action, CancellationToken cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
        Guard.AgainstNull(action, nameof(action));
        return persister.ProcessStreams(messageId, action, cancellation);
    }

    public Task<byte[]> GetBytesForMessage(string messageId, CancellationToken cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
        return persister.GetBytes(messageId, "default", cancellation);
    }

    public Task<byte[]> GetBytesForMessage(string messageId, string name, CancellationToken cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
        Guard.AgainstNull(name, nameof(name));
        return persister.GetBytes(messageId, name, cancellation);
    }

    public Stream GetStreamForMessage(string messageId)
    {
        Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
        return persister.GetStream(messageId, "default");
    }

    public Stream GetStreamForMessage(string messageId, string name)
    {
        Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
        Guard.AgainstNull(name, nameof(name));
        return persister.GetStream(messageId, name);
    }
}