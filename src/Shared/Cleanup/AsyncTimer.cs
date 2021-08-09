using System;
using System.Threading;
using System.Threading.Tasks;

class AsyncTimer :
    IAsyncTimer
{
    public void Start(Func<DateTime, CancellationToken, Task> callback, TimeSpan interval, Action<Exception> errorCallback, Func<TimeSpan, CancellationToken, Task> delayStrategy)
    {
        tokenSource = new();
        var token = tokenSource.Token;

        task = Task.Run(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var utcNow = DateTime.UtcNow;
                    await delayStrategy(interval, token);
                    await callback(utcNow, token);
                }
                catch (OperationCanceledException)
                {
                    // noop
                }
                catch (Exception ex)
                {
                    errorCallback(ex);
                }
            }
        }, CancellationToken.None);
    }

    public Task Stop()
    {
        if (tokenSource is null)
        {
            return Task.CompletedTask;
        }

        tokenSource.Cancel();
        tokenSource.Dispose();

        return task ?? Task.CompletedTask;
    }

    Task? task;
    CancellationTokenSource? tokenSource;
}