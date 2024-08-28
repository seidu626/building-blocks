using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BuildingBlocks.Modules.AuditTrail;

public class BackgroundAuditService<TDbContext> : BackgroundService where TDbContext : DbContext
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly BackgroundAuditQueue _auditQueue;

    public BackgroundAuditService(IServiceScopeFactory scopeFactory, BackgroundAuditQueue auditQueue)
    {
        _scopeFactory = scopeFactory;
        _auditQueue = auditQueue;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
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
    }
}