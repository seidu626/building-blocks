// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Exceptions.ApiError
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Westwind.Utilities;

namespace BuildingBlocks.Exceptions;

public class ApiError
{
  public string Message { get; set; }

  public bool IsError { get; set; }

  public string Detail { get; set; }

  public ValidationErrorCollection Errors { get; set; }

  public ApiError(string message)
  {
    this.Message = message;
    this.IsError = true;
  }

  public ApiError(ModelStateDictionary modelState)
  {
    this.IsError = true;
    if (modelState == null || !((IEnumerable<KeyValuePair<string, ModelStateEntry>>) modelState).Any<KeyValuePair<string, ModelStateEntry>>((Func<KeyValuePair<string, ModelStateEntry>, bool>) (m => ((Collection<ModelError>) m.Value.Errors).Count > 0)))
      return;
    this.Message = "Please correct the specified errors and try again.";
  }
}