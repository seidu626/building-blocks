using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Modules.AuditTrail;

public class BackgroundAuditService<TDbContext> : BackgroundService where TDbContext : DbContext
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly BackgroundAuditQueue _auditQueue;
    private readonly ILogger<BackgroundAuditService<TDbContext>> _logger;

    public BackgroundAuditService(IServiceScopeFactory scopeFactory, BackgroundAuditQueue auditQueue, ILogger<BackgroundAuditService<TDbContext>> logger)
    {
        _scopeFactory = scopeFactory;
        _auditQueue = auditQueue;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (_auditQueue.TryDequeue(out var entry))
                {
                    using var scope = _scopeFactory.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
                    dbContext.Add(entry.ToAudit());
                    await dbContext.SaveChangesAsync(stoppingToken);
                }
                else
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                }
            }
            catch (Exception e)
            {
                var ex = e.Demystify();
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}