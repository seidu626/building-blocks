// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Filters.IdempotencyAttribute
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace BuildingBlocks.Filters;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class IdempotencyAttribute : ActionFilterAttribute
{
  private static readonly ConcurrentDictionary<string, object> IdempotencyTokens = new ConcurrentDictionary<string, object>();

  public virtual void OnActionExecuting(ActionExecutingContext context)
  {
    StringValues source;
    if (((IDictionary<string, StringValues>) ((ActionContext) context).HttpContext.Request.Headers).TryGetValue("Idempotency-Token", out source))
    {
      string key = ((IEnumerable<string>) (object) source).FirstOrDefault<string>();
      if (string.IsNullOrEmpty(key))
      {
        context.Result = (IActionResult) new BadRequestObjectResult((object) "Missing idempotency token");
        return;
      }
      if (IdempotencyAttribute.IdempotencyTokens.ContainsKey(key))
      {
        context.Result = (IActionResult) new ConflictObjectResult((object) "Request already processed");
        return;
      }
      IdempotencyAttribute.IdempotencyTokens.TryAdd(key, (object) null);
    }
    else
      context.Result = (IActionResult) new BadRequestObjectResult((object) "Missing idempotency token");
    base.OnActionExecuting(context);
  }
}