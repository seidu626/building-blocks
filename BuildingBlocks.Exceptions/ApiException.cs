// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Exceptions.ApiException
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using Westwind.Utilities;

namespace BuildingBlocks.Exceptions;

public class ApiException : Exception
{
  public int StatusCode { get; set; }

  public ValidationErrorCollection Errors { get; set; }

  public ApiException(string message, int statusCode = 500, ValidationErrorCollection errors = null)
    : base(message)
  {
    this.StatusCode = statusCode;
    this.Errors = errors;
  }

  public ApiException(Exception ex, int statusCode = 500)
    : base(ex.Message)
  {
    this.StatusCode = statusCode;
  }
}