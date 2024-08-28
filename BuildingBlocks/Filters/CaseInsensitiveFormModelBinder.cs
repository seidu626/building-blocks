// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Filters.CaseInsensitiveFormModelBinder
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.ComponentModel;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BuildingBlocks.Filters;

public class CaseInsensitiveFormModelBinder : IModelBinder
{
  public Task BindModelAsync(ModelBindingContext bindingContext)
  {
    IFormCollection form = bindingContext.HttpContext.Request.Form;
    List<string> list = form.Keys.Select<string, string>((Func<string, string>) (k => k.ToLower())).ToList<string>();
    Type modelType = bindingContext.ModelType;
    PropertyInfo[] properties = modelType.GetProperties((BindingFlags) 20);
    object instance = Activator.CreateInstance(modelType);
    foreach (PropertyInfo propertyInfo in properties)
    {
      string propertyName = ((MemberInfo) propertyInfo).Name.ToLower();
      if (list.Contains(propertyName))
      {
        string str = ((IEnumerable<string>) (object) form[list.First<string>((Func<string, bool>) (n => n == propertyName))]).FirstOrDefault<string>();
        propertyInfo.SetValue(instance, CaseInsensitiveFormModelBinder.ConvertValue(str, propertyInfo.PropertyType));
      }
    }
    bindingContext.Result = ModelBindingResult.Success(instance);
    return Task.CompletedTask;
  }

  private static object ConvertValue(string value, Type targetType)
  {
    return targetType == typeof (string) ? (object) value : TypeDescriptor.GetConverter(targetType).ConvertFromString(value);
  }
}