// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Filters.RestrictToLocalhostAttribute
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BuildingBlocks.Filters;

public class RestrictToLocalhostAttribute : ActionFilterAttribute
{
  public virtual void OnActionExecuting(ActionExecutingContext context)
  {
    if (!IPAddress.IsLoopback(((ActionContext) context).HttpContext.Connection.RemoteIpAddress))
      context.Result = (IActionResult) new UnauthorizedResult();
    else
      base.OnActionExecuting(context);
  }
}