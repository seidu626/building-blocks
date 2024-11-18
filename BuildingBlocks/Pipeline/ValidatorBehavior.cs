using BuildingBlocks.Exceptions;
using FluentValidation;
using Mediator;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Pipeline
{
    public class ValidatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<ValidatorBehavior<TRequest, TResponse>> _logger;
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidatorBehavior(
            IEnumerable<IValidator<TRequest>> validators,
            ILogger<ValidatorBehavior<TRequest, TResponse>> logger)
        {
            _validators = validators;
            _logger = logger;
        }

        public async ValueTask<TResponse> Handle(
            TRequest message,
            MessageHandlerDelegate<TRequest, TResponse> next,
            CancellationToken cancellationToken)
        {
            string commandType = message.GetType().Name;
            _logger.LogInformation("----- Validating command {CommandType}", commandType);

            var validationFailures = _validators
                .Select(v => v.Validate(message))
                .SelectMany(result => result.Errors)
                .Where(error => error != null)
                .ToList();

            if (!validationFailures.Any())
            {
                return await next(message, cancellationToken);
            }

            _logger.LogWarning("Validation errors - {CommandType} - Command: {@Command} - Errors: {@ValidationErrors}", 
                commandType, message, validationFailures);

            throw new DomainException(
                $"Command Validation Errors for type {typeof(TRequest).Name}", 
                new ValidationException("Validation exception", validationFailures));
        }
    }
}