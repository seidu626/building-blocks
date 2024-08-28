// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Events.EventPublisher
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable enable
using BuildingBlocks.Infrastructure;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Events;

public class EventPublisher : IEventPublisher
{
  public virtual async 
#nullable disable
    Task PublishAsync<TEvent>(TEvent @event)
  {
    foreach (IConsumer<TEvent> consumer in EngineContext.Current.ResolveAll<IConsumer<TEvent>>().ToList<IConsumer<TEvent>>())
    {
      try
      {
        await consumer.HandleEventAsync(@event);
      }
      catch (Exception ex)
      {
        try
        {
          ILogger logger = EngineContext.Current.Resolve<ILogger>();
          if (logger == null)
            break;
          logger.LogError(ex.Message, (object) ex);
        }
        catch
        {
        }
      }
    }
  }
}