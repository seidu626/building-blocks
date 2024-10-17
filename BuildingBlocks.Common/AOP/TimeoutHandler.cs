using System.Reflection;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Common.AOP;

/// <summary>
/// Utility to invoke methods with a timeout.
/// Example usage:
/// <code>
/// var result = await TimeoutHandler.InvokeWithTimeoutAsync<Result<bool, Error>>(
/// _smsSender, nameof(_smsSender.SendAsync), logger, TimeSpan.FromSeconds(10),
/// identifier, otpResult.Message, cancellationToken);
/// </code>
/// </summary>
public static class TimeoutHandler
{
    public static async Task<TResult> InvokeWithTimeoutAsync<TResult>(
        object target, string methodName, ILogger logger, TimeSpan timeout, params object[] parameters)
    {
        var method = target.GetType().GetMethod(methodName);
        if (method == null)
            throw new InvalidOperationException($"Method '{methodName}' not found.");

        var methodParameters = AlignParameters(method, parameters);

        return await ExecuteWithTimeoutAsync<TResult>(method, target, methodParameters, logger, timeout);
    }

    private static async Task<TResult> ExecuteWithTimeoutAsync<TResult>(
        MethodInfo method, object target, object[] parameters, ILogger logger, TimeSpan timeout)
    {
        using var cts = new CancellationTokenSource(timeout);

        // Replace any existing CancellationToken with our new one
        for (int i = 0; i < parameters.Length; i++)
        {
            if (parameters[i] is CancellationToken)
            {
                parameters[i] = cts.Token;
                break;
            }
        }

        try
        {
            var result = method.Invoke(target, parameters);

            // If the result is a Task<TResult>, await it
            if (result is Task<TResult> taskResult)
            {
                return await AwaitWithTimeout(taskResult, timeout, method.Name, logger);
            }

            // If the result is a non-generic Task, await it and return default(TResult)
            if (result is Task nonGenericTask)
            {
                await AwaitWithTimeout(nonGenericTask, timeout, method.Name, logger);
                return default;
            }

            // If the method is synchronous, return the result directly
            return (TResult)result;
        }
        catch (Exception ex) when (ex is TargetInvocationException || ex is TimeoutException)
        {
            logger.LogError(ex, $"Error executing method '{method.Name}': {ex.Message}");
            throw;
        }
    }

    private static async Task AwaitWithTimeout(Task task, TimeSpan timeout, string methodName, ILogger logger)
    {
        if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
        {
            await task; // Task completed within the timeout
        }
        else
        {
            throw new TimeoutException($"Method '{methodName}' timed out after {timeout.TotalSeconds} seconds.");
        }
    }

    private static async Task<TResult> AwaitWithTimeout<TResult>(Task<TResult> task, TimeSpan timeout,
        string methodName, ILogger logger)
    {
        if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
        {
            return await task; // Task completed within the timeout
        }
        else
        {
            throw new TimeoutException($"Method '{methodName}' timed out after {timeout.TotalSeconds} seconds.");
        }
    }

    /// <summary>
    /// Aligns the provided parameters with the method's expected parameters.
    /// </summary>
    private static object[] AlignParameters(MethodInfo method, object[] providedParameters)
    {
        var methodParams = method.GetParameters();

        // Ensure that the provided parameters match the expected parameter count
        if (providedParameters.Length != methodParams.Length)
            throw new TargetParameterCountException($"Parameter count mismatch for method '{method.Name}'.");

        return providedParameters;
    }
}