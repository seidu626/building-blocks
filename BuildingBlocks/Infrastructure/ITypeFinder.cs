// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Infrastructure.ITypeFinder
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Reflection;

namespace BuildingBlocks.Infrastructure;

public interface ITypeFinder
{
  IEnumerable<Type> FindClassesOfType<T>(bool onlyConcreteClasses = true);

  IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, bool onlyConcreteClasses = true);

  IList<Assembly> GetAssemblies();
}