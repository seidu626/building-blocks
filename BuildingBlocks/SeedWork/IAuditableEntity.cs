// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.SeedWork.IAuditableEntity`1
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
namespace BuildingBlocks.SeedWork;

public class IAuditableEntity : IAuditableEntity<string>
{
}

public class IAuditableEntity<T>
{
    public T CreatedBy { get; set; }

    public DateTime CreatedOn { get; }

    public T LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }
}