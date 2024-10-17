using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Common.AOP;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class TimeoutAttribute(int timeoutInSeconds) : Attribute
{
    private TimeSpan Timeout { get; } = TimeSpan.FromSeconds(timeoutInSeconds);

    public async Task ExecuteWithTimeoutAsync(Func<Task> method, ILogger logger)
    {
        using var cts = new CancellationTokenSource(Timeout);   

        try
        {
            var task = method();
            if (await Task.WhenAny(task, Task.Delay(Timeout, cts.Token)) == task)
            {
                await task; // Task completed within the timeout
            }
            else
            {
                throw new TimeoutException($"Method timed out after {Timeout.TotalSeconds} seconds.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Method execution failed: {ex.Message}");
            throw;
        }
    }
}