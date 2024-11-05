using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Timeout;
using System.Net.Http;

namespace BuildingBlocks.Polly;

public static class PollyClientExtensions
{
    public static IHttpClientBuilder AddPollyPolicies(this IHttpClientBuilder builder, IConfiguration configuration)
    {
        builder.Services.Configure<PollySettings>(configuration.GetSection("Polly"));

        builder.AddPolicyHandler((serviceProvider, request) =>
        {
            var pollySettings = serviceProvider.GetRequiredService<IOptions<PollySettings>>().Value;
            var logger = serviceProvider.GetRequiredService<ILogger<BaseClient>>();

            // Retry Policy
            var retryPolicy = Policy
                .Handle<HttpRequestException>()
                .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .WaitAndRetryAsync(
                    pollySettings.RetryPolicy.RetryCount,
                    retryAttempt =>
                        TimeSpan.FromSeconds(Math.Pow(pollySettings.RetryPolicy.BaseDelaySeconds, retryAttempt)),
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        logger.LogWarning(
                            $"Retry {retryAttempt} after {timespan.TotalSeconds} seconds due to {outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString()}");
                    });

            // Circuit Breaker Policy
            var circuitBreakerPolicy = Policy
                .Handle<HttpRequestException>()
                .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .CircuitBreakerAsync(
                    pollySettings.CircuitBreakerPolicy.HandledEventsAllowedBeforeBreaking,
                    TimeSpan.FromSeconds(pollySettings.CircuitBreakerPolicy.DurationOfBreakSeconds),
                    onBreak: (outcome, breakDelay) =>
                    {
                        logger.LogWarning($"Circuit breaker opened for {breakDelay.TotalSeconds} seconds.");
                    },
                    onReset: () => logger.LogInformation("Circuit breaker reset."),
                    onHalfOpen: () => logger.LogInformation("Circuit breaker is half-open."));

            // Timeout Policy for Each Individual Request
            var requestTimeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(
                TimeSpan.FromSeconds(pollySettings.Timeout),
                TimeoutStrategy.Pessimistic,
                onTimeoutAsync: (context, timespan, task) =>
                {
                    logger.LogWarning($"Request timed out after {timespan.TotalSeconds} seconds.");
                    return Task.CompletedTask;
                });

            // Bulkhead Policy to Limit Parallel Requests
            var bulkheadPolicy = Policy.BulkheadAsync<HttpResponseMessage>(
                maxParallelization: 10,
                maxQueuingActions: 20,
                onBulkheadRejectedAsync: async context =>
                {
                    logger.LogWarning("Bulkhead limit reached. Request rejected.");
                });

            // Fallback Policy for Unavailable Service
            var fallbackPolicy = Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .OrResult(msg => !msg.IsSuccessStatusCode)
                .FallbackAsync(
                    new HttpResponseMessage(System.Net.HttpStatusCode.ServiceUnavailable)
                    {
                        Content = new StringContent("Service is unavailable. Please try again later.")
                    },
                    onFallbackAsync: async (outcome, context) =>
                    {
                        logger.LogWarning("Fallback triggered: {Reason}",
                            outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString());
                    });

            // **Global Timeout Policy** to Limit Entire Execution Time of All Policies
            var globalTimeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(
                TimeSpan.FromSeconds(pollySettings.GlobalTimeout),
                TimeoutStrategy.Optimistic,
                onTimeoutAsync: (context, timespan, task) =>
                {
                    logger.LogError($"Total execution time exceeded {timespan.TotalSeconds} seconds. Exiting.");
                    return Task.CompletedTask;
                });

            // **Wrap all Policies** with Global Timeout
            return Policy.WrapAsync(
                globalTimeoutPolicy, 
                fallbackPolicy, 
                bulkheadPolicy, 
                circuitBreakerPolicy, 
                requestTimeoutPolicy, 
                retryPolicy);
        });

        return builder;
    }
}
