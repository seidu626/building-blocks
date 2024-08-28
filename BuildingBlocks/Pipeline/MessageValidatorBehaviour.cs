#nullable disable
using Mediator;

namespace BuildingBlocks.Pipeline;

public sealed class MessageValidatorBehaviour<TMessage, TResponse> : 
  IPipelineBehavior<TMessage, TResponse>
  where TMessage : IValidate
{
  public ValueTask<TResponse> Handle(TMessage message, MessageHandlerDelegate<TMessage, TResponse> next, CancellationToken cancellationToken)
  {
    ValidationError error;
    if (!message.IsValid(out error))
      throw new ValidationException(error);
    return next(message, cancellationToken);
  }
}