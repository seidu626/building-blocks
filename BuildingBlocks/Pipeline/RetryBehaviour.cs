#nullable enable
using Mediator;
using Microsoft.Extensions.Logging;
using Polly;

namespace BuildingBlocks.Pipeline;

public class RetryBehaviour<TRequest, TResponse> : IPipelineBehavior<
#nullable disable
    TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IRetryableRequest<TRequest, TResponse>> _retryHandlers;
    private readonly ILogger<RetryBehaviour<TRequest, TResponse>> _logger;

    public RetryBehaviour(
        IEnumerable<IRetryableRequest<TRequest, TResponse>> retryHandlers,
        ILogger<RetryBehaviour<TRequest, TResponse>> logger)
    {
        this._retryHandlers = retryHandlers;
        this._logger = logger;
    }


    public async ValueTask<TResponse> Handle(TRequest message, MessageHandlerDelegate<TRequest, TResponse> next,
        CancellationToken cancellationToken)
    {
        IRetryableRequest<TRequest, TResponse> retryHandler =
            this._retryHandlers.FirstOrDefault<IRetryableRequest<TRequest, TResponse>>();
        if (retryHandler == null)
            return await next(message, cancellationToken);

        // ISSUE: reference to a compiler-generated field
        Policy<TResponse>.Handle<Exception>().CircuitBreakerAsync<TResponse>(
            retryHandler.ExceptionsAllowedBeforeCircuitTrip, TimeSpan.FromMilliseconds(5000.0),
            (Action<DelegateResult<TResponse>, TimeSpan>)((exception, things) =>
                this._logger.LogDebug("Circuit Tripped!")), (Action)(() => { }));
        return await Policy<TResponse>.Handle<Exception>().WaitAndRetryAsync<TResponse>(retryHandler.RetryAttempts,
            (Func<int, TimeSpan>)(retryAttempt =>
            {
                TimeSpan timeSpan = retryHandler.RetryWithExponentialBackoff
                    ? TimeSpan.FromMilliseconds(Math.Pow(2.0, (double)retryAttempt) *
                                                (double)retryHandler.RetryDelay)
                    : TimeSpan.FromMilliseconds((double)retryHandler.RetryDelay);
                return timeSpan;
            })).ExecuteAsync((Func<Task<TResponse>>)(async () => await next(message, cancellationToken)));
    }
}