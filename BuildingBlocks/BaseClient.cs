using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BuildingBlocks;

public class BaseClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BaseClient> _logger;

    public BaseClient(HttpClient httpClient, ILogger<BaseClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    private async Task<T> SendRequestAsync<T>(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending request: {Method} {Uri}", request.Method, request.RequestUri);

        HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        string content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Received response: {StatusCode}", response.StatusCode);

        if (!response.IsSuccessStatusCode)
        {
            string errorMessage = $"Request failed with status code {response.StatusCode}. Content: {content}";
            _logger.LogError(errorMessage);
            throw new HttpRequestException(errorMessage);
        }

        try
        {
            return JsonConvert.DeserializeObject<T>(content);
        }
        catch (JsonException jsonEx)
        {
            string errorMessage = $"Error deserializing response content. Content: {content}";
            _logger.LogError(jsonEx, errorMessage);
            throw new InvalidOperationException(errorMessage, jsonEx);
        }
    }

    public async Task<T> GetAsync<T>(
        CancellationToken cancellationToken,
        Dictionary<string, string> parameters,
        string url)
    {
        var query = string.Join("&", parameters);
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{url}?{query}");
        return await SendRequestAsync<T>(request, cancellationToken);
    }

    public async Task<TResult> PostAsync<T, TResult>(
        T data,
        CancellationToken cancellationToken,
        string url)
    {
        string jsonData = JsonConvert.SerializeObject(data);
        HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = content
        };
        return await SendRequestAsync<TResult>(request, cancellationToken);
    }

    public async Task<TResult> PutAsync<T, TResult>(
        int id,
        T data,
        CancellationToken cancellationToken,
        string url)
    {
        string jsonData = JsonConvert.SerializeObject(data);
        string requestUrl = $"{url}/{id}";
        HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, requestUrl)
        {
            Content = content
        };
        return await SendRequestAsync<TResult>(request, cancellationToken);
    }

    public async Task<string> DeleteAsync(
        int id,
        CancellationToken cancellationToken,
        string url)
    {
        string requestUrl = $"{url}/{id}";
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, requestUrl);
        return await SendRequestAsync<string>(request, cancellationToken);
    }
}