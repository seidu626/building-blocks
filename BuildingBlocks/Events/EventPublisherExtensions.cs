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
  public static async Task EntityInsertedAsync<TEntity, TId>(this IEventPublisher eventPublisher, TEntity entity) where TEntity : Entity<TId>
  {
    await eventPublisher.PublishAsync<EntityInsertedEvent<TEntity, TId>>(new EntityInsertedEvent<TEntity, TId>(entity));
  }

  public static async Task EntityUpdatedAsync<TEntity, TId>(this IEventPublisher eventPublisher, TEntity entity) where TEntity : Entity<TId>
  {
    await eventPublisher.PublishAsync<EntityUpdatedEvent<TEntity, TId>>(new EntityUpdatedEvent<TEntity, TId>(entity));
  }

  public static async Task EntityDeletedAsync<TEntity, TId>(this IEventPublisher eventPublisher, TEntity entity) where TEntity : Entity<TId>
  {
    await eventPublisher.PublishAsync<EntityDeletedEvent<TEntity, TId>>(new EntityDeletedEvent<TEntity, TId>(entity));
  }
}