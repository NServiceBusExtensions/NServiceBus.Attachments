using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class AttachmentCleanerTests
{
    [Fact]
    public async Task If_triggers_critical_action_after_10_failures()
    {
        var criticalActionTriggered = false;
        var timer = new FakeTimer();
        var cleaner = new TestableCleaner(token => Task.CompletedTask,
            (m, e) => criticalActionTriggered = true,  TimeSpan.Zero, timer);

        await cleaner.Start();

        for (var i = 0; i < 9; i++)
        {
            timer.OnError(new Exception("Simulated!"));
        }

        Assert.False(criticalActionTriggered);

        //Trigger the 10th time
        timer.OnError(new Exception("Simulated!"));
        Assert.True(criticalActionTriggered);
        criticalActionTriggered = false;

        //Trigger again -- the counter should be reset
        timer.OnError(new Exception("Simulated!"));
        Assert.False(criticalActionTriggered);
    }

    [Fact]
    public async Task It_resets_the_failure_counter_after_successful_attempt()
    {
        var criticalActionTriggered = false;
        var timer = new FakeTimer();
        var cleaner = new TestableCleaner(token => Task.CompletedTask,
            (m, e) => criticalActionTriggered = true, TimeSpan.Zero, timer);

        await cleaner.Start();

        for (var i = 0; i < 100; i++)
        {
            if (i % 9 == 0) //Succeed every 9th attempt
            {
                await timer.Tick(DateTime.UtcNow, CancellationToken.None);
            }
            else
            {
                timer.OnError(new Exception("Simulated!"));
            }
        }

        Assert.False(criticalActionTriggered);
    }

    class TestableCleaner :
        Cleaner
    {
        public TestableCleaner(Func<CancellationToken, Task> cleanup, Action<string, Exception> criticalError, TimeSpan frequencyToRunCleanup, IAsyncTimer timer)
            : base(cleanup, criticalError, frequencyToRunCleanup, timer)
        {
        }

        public Task Start()
        {
            return OnStart(null);
        }
    }

    class FakeTimer :
        IAsyncTimer
    {
        public Task Tick(DateTime utcTime, CancellationToken token)
        {
            return callback(utcTime, token);
        }

        public void OnError(Exception error)
        {
            errorCallback(error);
        }

        public void Start(Func<DateTime, CancellationToken, Task> callback, TimeSpan interval, Action<Exception> errorCallback, Func<TimeSpan, CancellationToken, Task> delayStrategy)
        {
            this.callback = callback;
            this.errorCallback = errorCallback;
        }

        public Task Stop()
        {
            return Task.CompletedTask;
        }

        Func<DateTime, CancellationToken, Task> callback = null!;
        Action<Exception> errorCallback = null!;
    }
}