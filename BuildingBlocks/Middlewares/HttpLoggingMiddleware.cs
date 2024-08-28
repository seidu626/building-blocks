using System.Runtime.ExceptionServices;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Middlewares;

public class HttpLoggingMiddleware
{
    private readonly ILogger<HttpLoggingMiddleware> _logger;
    private readonly RequestDelegate _next;

    public HttpLoggingMiddleware(RequestDelegate next, ILogger<HttpLoggingMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Invoke(HttpContext context)
    {
        var originalBodyStream = context.Response.Body;
        await LogRequestAsync(context.Request);
            
        await using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context);
            await LogResponseAsync(context.Response);
            await responseBody.CopyToAsync(originalBodyStream);
        }
        catch (Exception ex)
        {
            ExceptionDispatchInfo.Capture(ex).Throw();
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }

    private async Task LogRequestAsync(HttpRequest request)
    {
        request.EnableBuffering();
        var buffer = new byte[Convert.ToInt32(request.ContentLength)];
        await request.Body.ReadAsync(buffer.AsMemory(0, buffer.Length));
        var requestBody = Encoding.UTF8.GetString(buffer);
        request.Body.Position = 0;

        var logMessage = $"{request.Scheme} {request.Host}{request.Path} {request.QueryString} {requestBody}";
        _logger.LogInformation(logMessage);
    }

    private async Task LogResponseAsync(HttpResponse response)
    {
        response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(response.Body).ReadToEndAsync();
        response.Body.Seek(0, SeekOrigin.Begin);

        _logger.LogInformation(responseBody);
    }
}