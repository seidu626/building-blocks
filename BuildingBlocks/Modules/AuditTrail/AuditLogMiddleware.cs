#nullable enable
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Modules.AuditTrail;

public class AuditLogMiddleware<TDbContext> where TDbContext : 
#nullable disable
  DbContext
{
  private const string ControllerKey = "controller";
  private const string ActionKey = "action";
  private readonly RequestDelegate _next;
  private readonly AuditService<TDbContext> _auditService;

  public AuditLogMiddleware(RequestDelegate next, AuditService<TDbContext> auditService)
  {
    this._next = next;
    this._auditService = auditService;
  }

  public async Task InvokeAsync(HttpContext context)
  {
    await this._next.Invoke(context);
    await this._auditService.InvokeMiddlewareAsync(context, "controller", "action");
  }
}