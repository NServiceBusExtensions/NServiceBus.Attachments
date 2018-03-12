using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus.Attachments.FileShare;

class MessageAttachments : IMessageAttachments
{
    string messageId;
    Persister persister;

    internal MessageAttachments(string messageId, Persister persister)
    {
        this.messageId = messageId;
        this.persister = persister;
    }

    public async Task CopyTo(Stream target, CancellationToken cancellation = default)
    {
        Guard.AgainstNull(target, nameof(target));
        await persister.CopyTo(messageId, "", target, cancellation).ConfigureAwait(false);
    }

    public async Task CopyTo(string name, Stream target, CancellationToken cancellation = default)
    {
        Guard.AgainstNull(name, nameof(name));
        Guard.AgainstNull(target, nameof(target));
        await persister.CopyTo(messageId, name, target, cancellation).ConfigureAwait(false);
    }

    public async Task ProcessStream(Func<Stream, Task> action, CancellationToken cancellation = default)
    {
        Guard.AgainstNull(action, nameof(action));
        await persister.ProcessStream(messageId, "", action).ConfigureAwait(false);
    }

    public async Task ProcessStream(string name, Func<Stream, Task> action, CancellationToken cancellation = default)
    {
        Guard.AgainstNull(name, nameof(name));
        Guard.AgainstNull(action, nameof(action));
        await persister.ProcessStream(messageId, name, action).ConfigureAwait(false);
    }

    public async Task ProcessStreams(Func<string, Stream, Task> action, CancellationToken cancellation = default)
    {
        Guard.AgainstNull(action, nameof(action));
        await persister.ProcessStreams(messageId, action, cancellation).ConfigureAwait(false);
    }

    public async Task<byte[]> GetBytes(CancellationToken cancellation = default)
    {
        return await persister.GetBytes(messageId, "", cancellation).ConfigureAwait(false);
    }

    public async Task<byte[]> GetBytes(string name, CancellationToken cancellation = default)
    {
        Guard.AgainstNull(name, nameof(name));
        return await persister.GetBytes(messageId, name, cancellation).ConfigureAwait(false);
    }

    public Task<Stream> GetStream(CancellationToken cancellation = default)
    {
        return persister.GetStream(messageId, "");
    }

    public Task<Stream> GetStream(string name, CancellationToken cancellation = default)
    {
        Guard.AgainstNull(name, nameof(name));
        return persister.GetStream(messageId, name);
    }

    public async Task CopyToForMessage(string messageId, Stream target, CancellationToken cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
        Guard.AgainstNull(target, nameof(target));
        await persister.CopyTo(messageId, "", target, cancellation).ConfigureAwait(false);
    }

    public async Task CopyToForMessage(string messageId, string name, Stream target, CancellationToken cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
        Guard.AgainstNull(name, nameof(name));
        Guard.AgainstNull(target, nameof(target));
        await persister.CopyTo(messageId, name, target, cancellation).ConfigureAwait(false);
    }

    public async Task ProcessStreamForMessage(string messageId, Func<Stream, Task> action, CancellationToken cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
        Guard.AgainstNull(action, nameof(action));
        await persister.ProcessStream(messageId, "", action).ConfigureAwait(false);
    }

    public async Task ProcessStreamForMessage(string messageId, string name, Func<Stream, Task> action, CancellationToken cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
        Guard.AgainstNull(name, nameof(name));
        Guard.AgainstNull(action, nameof(action));
        await persister.ProcessStream(messageId, name, action).ConfigureAwait(false);
    }

    public async Task ProcessStreamsForMessage(string messageId, Func<string, Stream, Task> action, CancellationToken cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
        Guard.AgainstNull(action, nameof(action));
        await persister.ProcessStreams(messageId, action, cancellation).ConfigureAwait(false);
    }

    public async Task<byte[]> GetBytesForMessage(string messageId, CancellationToken cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
        return await persister.GetBytes(messageId, "", cancellation).ConfigureAwait(false);
    }

    public async Task<byte[]> GetBytesForMessage(string messageId, string name, CancellationToken cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
        Guard.AgainstNull(name, nameof(name));
        return await persister.GetBytes(messageId, name, cancellation).ConfigureAwait(false);
    }

    public Task<Stream> GetStreamForMessage(string messageId, CancellationToken cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
        return persister.GetStream(messageId, "");
    }

    public Task<Stream> GetStreamForMessage(string messageId, string name, CancellationToken cancellation = default)
    {
        Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
        Guard.AgainstNull(name, nameof(name));
        return persister.GetStream(messageId, name);
    }
}