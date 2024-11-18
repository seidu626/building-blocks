using Mediator;

namespace BuildingBlocks.Pipeline
{
    public sealed class MessageValidatorBehaviour<TMessage, TResponse> : IPipelineBehavior<TMessage, TResponse>
        where TMessage : IValidate
    {
        public async ValueTask<TResponse> Handle(
            TMessage message,
            MessageHandlerDelegate<TMessage, TResponse> next,
            CancellationToken cancellationToken)
        {
            if (!message.IsValid(out var error))
            {
                throw new ValidationException(error);
            }

            return await next(message, cancellationToken);
        }
    }
}