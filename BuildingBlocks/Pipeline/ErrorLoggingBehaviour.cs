using Mediator;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Pipeline
{
    public sealed class ErrorLoggingBehaviour<TMessage, TResponse> : IPipelineBehavior<TMessage, TResponse>
        where TMessage : IMessage
    {
        private readonly ILogger<ErrorLoggingBehaviour<TMessage, TResponse>> _logger;

        public ErrorLoggingBehaviour(ILogger<ErrorLoggingBehaviour<TMessage, TResponse>> logger)
        {
            _logger = logger;
        }

        public async ValueTask<TResponse> Handle(
            TMessage message,
            MessageHandlerDelegate<TMessage, TResponse> next,
            CancellationToken cancellationToken)
        {
            try
            {
                return await next(message, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling message of type {MessageType}: {@Message}", message.GetType().Name,
                    message);
                throw;
            }
        }
    }
}