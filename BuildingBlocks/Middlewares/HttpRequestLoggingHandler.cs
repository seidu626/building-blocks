using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Middlewares;

public class HttpRequestLoggingHandler : DelegatingHandler
{
    private readonly ILogger<HttpRequestLoggingHandler> _logger;

    public HttpRequestLoggingHandler(HttpMessageHandler innerHandler, ILogger<HttpRequestLoggingHandler> logger)
        : base(innerHandler)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        LogRequest(request);
        var response = await base.SendAsync(request, cancellationToken);
        LogResponse(response);
        return response;
    }

    private async Task LogRequest(HttpRequestMessage request)
    {
        var requestInfo = $"Request HttpMethod: {request.Method}, Request URI: {request.RequestUri}";
        var requestHeaders = FormatHeaders(request.Headers);
        _logger.LogInformation($"{requestInfo}\nHeaders:\n{requestHeaders}");

        if (request.Content != null)
        {
            var requestBody = await request.Content.ReadAsStringAsync();
            _logger.LogInformation($"Request Body:\n{requestBody}");
        }
    }

    private async Task LogResponse(HttpResponseMessage response)
    {
        var responseInfo = $"Response StatusCode: {response.StatusCode}";
        var responseHeaders = FormatHeaders(response.Headers);
        _logger.LogInformation($"{responseInfo}\nHeaders:\n{responseHeaders}");

        if (response.Content != null)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"Response Body:\n{responseBody}");
        }
    }

    private string FormatHeaders(HttpHeaders headers)
    {
        var builder = new StringBuilder();
        foreach (var header in headers)
        {
            builder.AppendLine($"{header.Key}: {string.Join(", ", header.Value)}");
        }
        return builder.ToString();
    }
}