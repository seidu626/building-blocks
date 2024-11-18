using BuildingBlocks.Extensions;
using Mediator;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Pipeline
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async ValueTask<TResponse> Handle(
            TRequest message,
            MessageHandlerDelegate<TRequest, TResponse> next,
            CancellationToken cancellationToken)
        {
            var commandName = message.GetGenericTypeName();
            
            _logger.LogInformation("----- Handling command {CommandName} ({@Command})", commandName, message);

            TResponse response = await next(message, cancellationToken);

            _logger.LogInformation("----- Command {CommandName} handled - response: {@Response}", commandName, response);

            return response;
        }
    }
}