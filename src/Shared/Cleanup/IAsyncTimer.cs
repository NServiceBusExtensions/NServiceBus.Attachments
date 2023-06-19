interface IAsyncTimer
{
    void Start(Func<DateTime, Cancel, Task> callback, TimeSpan interval, Action<Exception> errorCallback, Func<TimeSpan, Cancel, Task> delayStrategy);
    Task Stop();
}