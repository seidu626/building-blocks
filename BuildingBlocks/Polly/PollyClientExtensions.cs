using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;

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

            // Retry policy
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

            // Circuit breaker policy
            var circuitBreakerPolicy = Policy
                .Handle<HttpRequestException>()
                .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .CircuitBreakerAsync(
                    pollySettings.CircuitBreakerPolicy.HandledEventsAllowedBeforeBreaking,
                    TimeSpan.FromSeconds(pollySettings.CircuitBreakerPolicy.DurationOfBreakSeconds),
                    onBreak: (outcome, breakDelay) =>
                    {
                        logger.LogWarning(
                            $"Circuit breaker opened for {breakDelay.TotalSeconds} seconds due to {outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString()}");
                    },
                    onReset: () => { logger.LogInformation("Circuit breaker reset."); },
                    onHalfOpen: () =>
                    {
                        logger.LogInformation("Circuit breaker is half-open. Next call is a trial.");
                    });

            // Timeout Policy: Consider adding a timeout policy to prevent requests from hanging indefinitely.
            var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(30));

            var bulkheadPolicy = Policy.BulkheadAsync<HttpResponseMessage>(maxParallelization: 10,
                maxQueuingActions: 20,
                onBulkheadRejectedAsync: async context =>
                {
                    logger.LogWarning("Bulkhead limit reached. Request rejected.");
                });

            var fallbackPolicy = Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .OrResult(msg => !msg.IsSuccessStatusCode)
                .FallbackAsync(new HttpResponseMessage(System.Net.HttpStatusCode.ServiceUnavailable)
                    {
                        Content = new StringContent("Service is unavailable. Please try again later.")
                    },
                    onFallbackAsync: async (outcome, context) =>
                    {
                        logger.LogWarning("Fallback triggered due to {Reason}",
                            outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString());
                    });

            return Policy.WrapAsync(retryPolicy, circuitBreakerPolicy, timeoutPolicy, bulkheadPolicy, fallbackPolicy);
        });
        
        return builder;
    }
}