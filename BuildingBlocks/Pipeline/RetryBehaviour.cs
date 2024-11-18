using Mediator;
using Microsoft.Extensions.Logging;
using Polly;

namespace BuildingBlocks.Pipeline
{
    public class RetryBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IRetryableRequest<TRequest, TResponse>> _retryHandlers;
        private readonly ILogger<RetryBehaviour<TRequest, TResponse>> _logger;

        public RetryBehaviour(
            IEnumerable<IRetryableRequest<TRequest, TResponse>> retryHandlers,
            ILogger<RetryBehaviour<TRequest, TResponse>> logger)
        {
            _retryHandlers = retryHandlers;
            _logger = logger;
        }

        public async ValueTask<TResponse> Handle(
            TRequest message,
            MessageHandlerDelegate<TRequest, TResponse> next,
            CancellationToken cancellationToken)
        {
            // Get the first retry handler (if any) to define retry and circuit-breaker policies
            var retryHandler = _retryHandlers.FirstOrDefault();
            if (retryHandler == null)
            {
                return await next(message, cancellationToken);
            }

            // Define retry policy
            var retryPolicy = Policy.Handle<Exception>()
                .WaitAndRetryAsync(
                    retryHandler.RetryAttempts,
                    retryAttempt => CalculateRetryDelay(retryAttempt, retryHandler.RetryDelay, retryHandler.RetryWithExponentialBackoff),
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogDebug("Retrying attempt {RetryCount} after {Delay} due to error: {ExceptionMessage}", retryCount, timeSpan, exception.Message);
                    });

            // Define circuit-breaker policy
            var circuitBreakerPolicy = Policy.Handle<Exception>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: retryHandler.ExceptionsAllowedBeforeCircuitTrip,
                    durationOfBreak: TimeSpan.FromMilliseconds(5000),
                    onBreak: (exception, timespan) =>
                    {
                        _logger.LogWarning("Circuit tripped due to: {ExceptionMessage}. Breaking for {Duration}", exception.Message, timespan);
                    },
                    onReset: () => _logger.LogInformation("Circuit reset"),
                    onHalfOpen: () => _logger.LogInformation("Circuit in half-open state, testing for recovery."));

            // Combine retry and circuit-breaker policies
            var policyWrap = Policy.WrapAsync(retryPolicy, circuitBreakerPolicy);

            // Execute the policy with the next handler in the pipeline, converting ValueTask to Task
            return await policyWrap.ExecuteAsync(() => next(message, cancellationToken).AsTask());
        }

        private TimeSpan CalculateRetryDelay(int retryAttempt, int delay, bool exponentialBackoff)
        {
            var timeSpan = exponentialBackoff
                ? TimeSpan.FromMilliseconds(Math.Pow(2, retryAttempt) * delay)
                : TimeSpan.FromMilliseconds(delay);

            _logger.LogDebug("Retrying, waiting {Delay}...", timeSpan);
            return timeSpan;
        }
    }
}
