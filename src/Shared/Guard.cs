static class Guard
{
    public static void FileExists(string? path, string argumentName)
    {
        AgainstNullOrEmpty(path, argumentName);
        if (!File.Exists(path))
        {
            throw new ArgumentException($"File does not exist: {path}", argumentName);
        }
    }

    public static void AgainstEmpty(string? value, string argumentName)
    {
        if (value is null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentNullException(argumentName);
        }
    }

    public static void AgainstNullOrEmpty(string? value, string argumentName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentNullException(argumentName);
        }
    }

    public static void AgainstLongAttachmentName(string value)
    {
        if (value.Length > 255)
        {
            throw new ArgumentException($"Attachment Name too long. Max length is 255 characters. Value: {value}");
        }
    }

    public static Func<T> WrapFuncInCheck<T>(this Func<T> func, string name) =>
        () => func.EvaluateAndCheck(name);

    static T EvaluateAndCheck<T>(this Func<T> func, string attachmentName)
    {
        var message = $"Provided delegate threw an exception. Attachment name: {attachmentName}.";
        T value;
        try
        {
            value = func();
        }
        catch (Exception exception)
        {
            throw new(message, exception);
        }

        ThrowIfNullReturned(null, attachmentName, value);
        return value;
    }

    public static Action? WrapCleanupInCheck(this Action? cleanup, string attachmentName)
    {
        if (cleanup is null)
        {
            return null;
        }

        return () =>
        {
            try
            {
                cleanup();
            }
            catch (Exception exception)
            {
                throw new($"Cleanup threw an exception. Attachment name: {attachmentName}.", exception);
            }
        };
    }

    public static Func<Task<T>> WrapFuncTaskInCheck<T>(this Func<Task<T>> func, string attachmentName) =>
        async () =>
        {
            var task = func.EvaluateAndCheck(attachmentName);
            ThrowIfNullReturned(null, attachmentName, task);
            var value = await task;
            ThrowIfNullReturned(null, attachmentName, value);
            return value;
        };

    public static Func<Task<Stream>> WrapStreamFuncTaskInCheck<T>(this Func<Task<T>> func, string attachmentName)
        where T : Stream =>
        async () =>
        {
            var task = func.EvaluateAndCheck(attachmentName);
            ThrowIfNullReturned(null, attachmentName, task);
            var value = await task;
            ThrowIfNullReturned(null, attachmentName, value);
            return value;
        };

    public static void ThrowIfNullReturned(object? value)
    {
        if (value is null)
        {
            throw new("Provided delegate returned a null.");
        }
    }

    public static void ThrowIfNullReturned(string? messageId, string? attachmentName, object? value)
    {
        if (value is null)
        {
            if (attachmentName != null && messageId is not null)
            {
                throw new($"Provided delegate returned a null. MessageId: '{messageId}', Attachment: '{attachmentName}'.");
            }

            if (attachmentName is not null)
            {
                throw new($"Provided delegate returned a null. Attachment: '{attachmentName}'.");
            }

            if (messageId is not null)
            {
                throw new($"Provided delegate returned a null. MessageId: '{messageId}'.");
            }

            throw new("Provided delegate returned a null.");
        }
    }
}