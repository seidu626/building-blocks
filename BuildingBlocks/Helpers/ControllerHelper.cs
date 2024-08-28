// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Helpers.ControllerHelper
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable enable
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Helpers;

public static class ControllerHelper
{
  public static async 
#nullable disable
    Task ExecuteResultAsync<T>(
      this T controller,
      ActionContext context,
      string actionName,
      string controllerName,
      string displayName)
    where T : ControllerBase
  {
    context.RouteData.Values[nameof (controller)] = (object) controllerName;
    context.RouteData.Values["action"] = (object) actionName;
    ActionContext actionContext1 = new ActionContext(context);
    ControllerActionDescriptor actionDescriptor = new ControllerActionDescriptor();
    actionDescriptor.ActionName = actionName;
    actionDescriptor.ControllerName = controllerName;
    actionDescriptor.ControllerTypeInfo = IntrospectionExtensions.GetTypeInfo(((object) (T) controller).GetType());
    ((ActionDescriptor) actionDescriptor).DisplayName = displayName;
    actionContext1.ActionDescriptor = (ActionDescriptor) actionDescriptor;
    ActionContext actionContext2 = actionContext1;
    await ServiceProviderServiceExtensions.GetRequiredService<IActionInvokerFactory>(context.HttpContext.RequestServices).CreateInvoker(actionContext2).InvokeAsync();
  }
}