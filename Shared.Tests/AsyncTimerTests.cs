﻿using System;
using System.Threading.Tasks;
using Xunit;

public class AsyncTimerTests
{
    [Fact]
    public async Task It_calls_error_callback()
    {
        var errorCallbackInvoked = new TaskCompletionSource<bool>();

        var timer = new AsyncTimer();
        timer.Start(
            callback: (time, token) => throw new Exception("Simulated!"),
            interval: TimeSpan.Zero,
            errorCallback: e => { errorCallbackInvoked.SetResult(true); },
            delayStrategy: Task.Delay);

        Assert.True(await errorCallbackInvoked.Task.ConfigureAwait(false));
    }

    [Fact]
    public async Task It_continues_to_run_after_an_error()
    {
        var callbackInvokedAfterError = new TaskCompletionSource<bool>();

        var fail = true;
        var exceptionThrown = false;
        var timer = new AsyncTimer();
        timer.Start(
            callback: (time, token) =>
            {
                if (fail)
                {
                    fail = false;
                    throw new Exception("Simulated!");
                }

                Assert.True(exceptionThrown);
                callbackInvokedAfterError.SetResult(true);
                return Task.FromResult(0);
            },
            interval: TimeSpan.Zero,
            errorCallback: e => { exceptionThrown = true; },
            delayStrategy: Task.Delay);

        Assert.True(await callbackInvokedAfterError.Task.ConfigureAwait(false));
    }

    [Fact]
    public async Task Stop_cancels_token_while_waiting()
    {
        var timer = new AsyncTimer();
        var waitCancelled = false;
        var delayStarted = new TaskCompletionSource<bool>();

        timer.Start(
            callback: (time, token) => throw new Exception("Simulated!"),
            interval: TimeSpan.FromDays(7),
            errorCallback: e =>
            {
                //noop
            },
            delayStrategy: async (delayTime, token) =>
            {
                delayStarted.SetResult(true);
                try
                {
                    await Task.Delay(delayTime, token).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    waitCancelled = true;
                }
            });
        await delayStarted.Task.ConfigureAwait(false);
        await timer.Stop().ConfigureAwait(false);

        Assert.True(waitCancelled);
    }

    [Fact]
    public async Task Stop_cancels_token_while_in_callback()
    {
        var timer = new AsyncTimer();
        var callbackCancelled = false;
        var callbackStarted = new TaskCompletionSource<bool>();
        var stopInitiated = new TaskCompletionSource<bool>();

        timer.Start(
            callback: async (time, token) =>
            {
                callbackStarted.SetResult(true);
                await stopInitiated.Task.ConfigureAwait(false);
                if (token.IsCancellationRequested)
                {
                    callbackCancelled = true;
                }
            },
            interval: TimeSpan.Zero,
            errorCallback: e =>
            {
                //noop
            },
            delayStrategy: Task.Delay);

        await callbackStarted.Task.ConfigureAwait(false);
        var stopTask = timer.Stop();
        stopInitiated.SetResult(true);
        await stopTask.ConfigureAwait(false);
        Assert.True(callbackCancelled);
    }

    [Fact]
    public async Task Stop_waits_for_callback_to_complete()
    {
        var timer = new AsyncTimer();

        var callbackCompleted = new TaskCompletionSource<bool>();
        var callbackTaskStarted = new TaskCompletionSource<bool>();

        timer.Start(
            callback: (time, token) =>
            {
                callbackTaskStarted.SetResult(true);
                return callbackCompleted.Task;
            },
            interval: TimeSpan.Zero,
            errorCallback: e =>
            {
                //noop
            },
            delayStrategy: Task.Delay);

        await callbackTaskStarted.Task.ConfigureAwait(false);

        var stopTask = timer.Stop();
        var delayTask = Task.Delay(1000);

        var firstToComplete = await Task.WhenAny(stopTask, delayTask).ConfigureAwait(false);
        Assert.Equal(delayTask, firstToComplete);
        callbackCompleted.SetResult(true);

        await stopTask.ConfigureAwait(false);
    }
}