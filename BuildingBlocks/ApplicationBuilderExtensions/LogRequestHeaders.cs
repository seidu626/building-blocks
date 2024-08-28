using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace BuildingBlocks.ApplicationBuilderExtensions;

public static class LogRequestHeaders
{
    public static void Log(this IApplicationBuilder app, ILoggerFactory loggerFactory)
    {
        ILogger logger = loggerFactory.CreateLogger("Request Headers");
        app.Use(async (context, next) =>
        {
            StringBuilder logBuilder = new StringBuilder(Environment.NewLine);
            foreach (KeyValuePair<string, StringValues> header in context.Request.Headers)
            {
                logBuilder.AppendLine($"{header.Key}: {header.Value}");
            }
            logger.LogTrace(logBuilder.ToString());
            await next.Invoke();
        });
    }
}