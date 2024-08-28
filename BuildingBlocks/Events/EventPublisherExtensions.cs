// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Events.EventPublisherExtensions
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable enable
using BuildingBlocks.SeedWork;

namespace BuildingBlocks.Events;

public static class EventPublisherExtensions
{
  public static async 
#nullable disable
    Task EntityInsertedAsync<T>(this IEventPublisher eventPublisher, T entity) where T : Entity
  {
    await eventPublisher.PublishAsync<EntityInsertedEvent<T>>(new EntityInsertedEvent<T>(entity));
  }

  public static async Task EntityUpdatedAsync<T>(this IEventPublisher eventPublisher, T entity) where T : Entity
  {
    await eventPublisher.PublishAsync<EntityUpdatedEvent<T>>(new EntityUpdatedEvent<T>(entity));
  }

  public static async Task EntityDeletedAsync<T>(this IEventPublisher eventPublisher, T entity) where T : Entity
  {
    await eventPublisher.PublishAsync<EntityDeletedEvent<T>>(new EntityDeletedEvent<T>(entity));
  }
}