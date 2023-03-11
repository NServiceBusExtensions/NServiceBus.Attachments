interface IAsyncTimer
{
    void Start(Func<DateTime, Cancellation, Task> callback, TimeSpan interval, Action<Exception> errorCallback, Func<TimeSpan, Cancellation, Task> delayStrategy);
    Task Stop();
}