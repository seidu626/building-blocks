// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Filters.ValidateModelStateFilter
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable enable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BuildingBlocks.Filters;

public class ValidateModelStateFilter : ActionFilterAttribute
{
  public virtual void OnActionExecuting(
#nullable disable
    ActionExecutingContext context)
  {
    if (((ActionContext) context).ModelState.IsValid)
      return;
    string[] array = ((IEnumerable<string>) (object) ((ActionContext) context).ModelState.Keys).SelectMany<string, ModelError>((Func<string, IEnumerable<ModelError>>) (k => (IEnumerable<ModelError>) ((ActionContext) context).ModelState[k].Errors)).Select<ModelError, string>((Func<ModelError, string>) (e => e.ErrorMessage)).ToArray<string>();
    JsonErrorResponse jsonErrorResponse = new JsonErrorResponse()
    {
      Messages = array
    };
    context.Result = (IActionResult) new BadRequestObjectResult((object) jsonErrorResponse);
  }
}