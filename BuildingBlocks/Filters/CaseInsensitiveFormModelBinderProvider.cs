// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Filters.CaseInsensitiveFormModelBinderProvider
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable enable
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BuildingBlocks.Filters;

public class CaseInsensitiveFormModelBinderProvider : IModelBinderProvider
{
  public IModelBinder? GetBinder(
#nullable disable
    ModelBinderProviderContext context)
  {
    if (context == null)
      throw new ArgumentNullException(nameof (context));
    BindingSource bindingSource = context.Metadata.BindingSource;
    return (bindingSource != null ? (bindingSource.CanAcceptDataFrom(BindingSource.Form) ? 1 : 0) : 0) != 0 ? (IModelBinder) new CaseInsensitiveFormModelBinder() : (IModelBinder) null;
  }
}