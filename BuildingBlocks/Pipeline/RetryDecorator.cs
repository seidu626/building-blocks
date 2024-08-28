// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Behaviours.RetryDecorator`1
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using Mediator;
using Polly;

namespace BuildingBlocks.Pipeline;

public class RetryDecorator<TNotification> : INotificationHandler<TNotification> where TNotification : INotification
{
  private readonly INotificationHandler<TNotification> _inner;
  private readonly IAsyncPolicy _retryPolicy;

  public RetryDecorator(INotificationHandler<TNotification> inner)
  {
    this._inner = inner;
    this._retryPolicy = (IAsyncPolicy) Policy.Handle<ArgumentOutOfRangeException>().WaitAndRetryAsync(3, (Func<int, TimeSpan>) (i => TimeSpan.FromSeconds((double) i)));
  }

  public ValueTask Handle(TNotification notification, CancellationToken cancellationToken)
  {
    return ValueTask.CompletedTask;
  }
}