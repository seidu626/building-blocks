// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.SeedWork.IQueryHandler`2
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
namespace BuildingBlocks.SeedWork;

public interface IQueryHandler<in TRequest, TResponse> : Mediator.IQueryHandler<TRequest, TResponse>
  where TRequest : IQuery<TResponse>
{
}