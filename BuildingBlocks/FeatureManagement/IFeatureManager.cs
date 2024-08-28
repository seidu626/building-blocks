// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.FeatureManagement.IFeatureManager
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
namespace BuildingBlocks.FeatureManagement;

public interface IFeatureManager
{
  IAsyncEnumerable<string> GetFeatureFlagNamesAsync(CancellationToken cancellationToken = default (CancellationToken));

  Task<bool> IsEnabledAsync(string featureFlag, CancellationToken cancellationToken = default (CancellationToken));

  Task<bool> IsEnabledAsync<TContext>(
    string featureFlag,
    TContext context,
    CancellationToken cancellationToken = default (CancellationToken));
}