namespace BuildingBlocks.Utilities;

/// <summary>
/// This call will be used to send the data after each second to the client
/// https://www.webnethelper.com/2022/01/aspnet-core-6-signalr-creating-real.html
/// </summary>
public class TimeWatcher
{
    private Action? Executor;
    private Timer? timer;
    // we need to auto-reset the event before the execution
    private AutoResetEvent? autoResetEvent;


    public DateTime WatcherStarted { get; set; }

    public bool IsWatcherStarted { get; set; }

    /// <summary>
    /// Method for the Timer Watcher
    /// This will be invoked when the Controller receives the request
    /// {totalExecutionSeconds} Number of seconds to execute the action Default: 60 seconds
    /// </summary>
    public void Watcher(Action execute, int totalExecutionSeconds = 60,
        int callBackDelayBeforeInvokeCallback = 1000, int timeIntervalBetweenInvokeCallback = 2000)
    {
        Executor = execute;
        autoResetEvent = new AutoResetEvent(false);
        timer = new Timer((object? obj) =>
        {
            Executor();
            if ((DateTime.Now - WatcherStarted).TotalSeconds > totalExecutionSeconds)
            {
                IsWatcherStarted = false;
                timer.Dispose();
            }
        }, autoResetEvent, callBackDelayBeforeInvokeCallback, timeIntervalBetweenInvokeCallback);

        WatcherStarted = DateTime.Now;
        IsWatcherStarted = true;
    }
}