using System.Text;
using BuildingBlocks.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Modules.AuditTrail;

public class AuditService<TDbContext> where TDbContext : DbContext
{
    private const string IdKey = "id";
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly BackgroundAuditQueue _auditQueue;
    private readonly IConfiguration _configuration;
    private readonly IIdentityService _identityService;

    public AuditService(
        IServiceScopeFactory scopeFactory,
        BackgroundAuditQueue auditQueue,
        IConfiguration configuration,
        IIdentityService identityService)
    {
        _scopeFactory = scopeFactory;
        _auditQueue = auditQueue;
        _configuration = configuration;
        _identityService = identityService;
    }

    public async Task InvokeMiddlewareAsync(HttpContext context, string controllerKey, string actionKey)
    {
        var auditConfig = _configuration.GetSection("AuditConfig").Get<AuditConfig>();
        var request = context.Request;
        var path = request.Path;

        if (!path.StartsWithSegments("/api"))
        {
            return;
        }

        if (request.RouteValues.TryGetValue(controllerKey, out var controllerNameObj) &&
            request.RouteValues.TryGetValue(actionKey, out var actionNameObj))
        {
            var controllerName = controllerNameObj?.ToString() ?? string.Empty;
            var actionName = actionNameObj?.ToString() ?? string.Empty;
            var changedValue = await GetChangedValues(request).ConfigureAwait(false);
            var username = await _identityService.GetUsernameAsync();

            var entry = new AuditEntry
            {
                EntityName = controllerName,
                UserId = username,
                AuditType = AuditType.Request,
                Metadata = request.Method,
                NewValues = { [actionName] = changedValue }
            };

            if (auditConfig.UseBackgroundQueue)
            {
                _auditQueue.QueueAuditEntry(entry);
            }
            else
            {
                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
                dbContext.Add(entry.ToAudit());
                await dbContext.SaveChangesAsync(new CancellationToken());
            }
        }
    }

    private static async Task<string> GetChangedValues(HttpRequest request)
    {
        return request.Method switch
        {
            "POST" or "PUT" => await ReadRequestBody(request).ConfigureAwait(false),
            "DELETE" => request.RouteValues.TryGetValue(IdKey, out var id) ? id?.ToString() ?? string.Empty : string.Empty,
            _ => string.Empty
        };
    }

    private static async Task<string> ReadRequestBody(HttpRequest request, Encoding? encoding = null)
    {
        request.Body.Position = 0;
        using var reader = new StreamReader(request.Body, encoding ?? Encoding.UTF8);
        var body = await reader.ReadToEndAsync().ConfigureAwait(false);
        request.Body.Position = 0;
        return body;
    }
}