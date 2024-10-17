// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.AOP.ErrorLoggingBehaviour`2
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable enable
using Mediator;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Pipeline;

public sealed class ErrorLoggingBehaviour<TMessage, TResponse> :
    IPipelineBehavior<
#nullable disable
        TMessage, TResponse>
    where TMessage : IMessage
{
    private readonly ILogger<ErrorLoggingBehaviour<TMessage, TResponse>> _logger;

    public ErrorLoggingBehaviour(
        ILogger<ErrorLoggingBehaviour<TMessage, TResponse>> logger)
    {
        this._logger = logger;
    }


    public async ValueTask<TResponse> Handle(TMessage message, MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken)
    {
        TResponse response;
        try
        {
            response = await next(message, cancellationToken);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error handling message of type {messageType}",
                (object)message.GetType().Name);
            throw;
        }

        return response;
    }
}