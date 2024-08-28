// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Filters.ApiExceptionFilter
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using BuildingBlocks.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BuildingBlocks.Filters;

public class ApiExceptionFilter : ExceptionFilterAttribute
{
  public virtual void OnException(ExceptionContext context)
  {
    ApiError apiError;
    if (context.Exception is ApiException)
    {
      ApiException exception = context.Exception as ApiException;
      context.Exception = (Exception) null;
      apiError = new ApiError(exception.Message);
      apiError.Errors = exception.Errors;
      ((ActionContext) context).HttpContext.Response.StatusCode = exception.StatusCode;
    }
    else if (context.Exception is UnauthorizedAccessException)
    {
      apiError = new ApiError("Unauthorized Access");
      ((ActionContext) context).HttpContext.Response.StatusCode = 401;
    }
    else
    {
      string str = (string) null;
      apiError = new ApiError("An unhandled error occurred.");
      apiError.Detail = str;
      ((ActionContext) context).HttpContext.Response.StatusCode = 500;
    }
    context.Result = (IActionResult) new JsonResult((object) apiError);
    base.OnException(context);
  }
}