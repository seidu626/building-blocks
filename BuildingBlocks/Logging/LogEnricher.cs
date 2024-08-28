using BuildingBlocks.Http.Extensions;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace BuildingBlocks.Logging;

public static class LogEnricher
{
    public static void EnrichFromRequest(IDiagnosticContext diagnosticContext, HttpContext httpContext)
    {
        if (httpContext == null)
        {
            throw new ArgumentNullException(nameof(httpContext), "HttpContext cannot be null.");
        }

        var clientIP = httpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = httpContext.Request.Headers["User-Agent"].FirstOrDefault();
        var resource = httpContext.GetMetricsCurrentResourceName();  // Assuming implementation exists

        diagnosticContext.Set("ClientIP", clientIP);
        diagnosticContext.Set("UserAgent", userAgent);
        diagnosticContext.Set("Resource", resource);

        // Additional context details can be added here
        diagnosticContext.Set("RequestMethod", httpContext.Request.Method);
        diagnosticContext.Set("RequestPath", httpContext.Request.Path);
    }
}