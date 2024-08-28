// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Common.LocalRestClient
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable enable
using System.Net;
using BuildingBlocks.Common.Interfaces;
using BuildingBlocks.Extensions;

namespace BuildingBlocks.Common;

public sealed class LocalRestClient : IRestClient, IDisposable
{
  private const int MaxConnectionPerServer = 20;
  private static readonly TimeSpan ConnectionLifeTime = 1.Minutes();
  private readonly 
#nullable disable
    HttpClient _client;

  static LocalRestClient() => LocalRestClient.ConfigureServicePointManager();

  public LocalRestClient(
    IDictionary<string, IEnumerable<string>> defaultRequestHeaders = null,
    HttpMessageHandler handler = null,
    Uri baseAddress = null,
    bool disposeHandler = true,
    TimeSpan? timeout = null,
    ulong? maxResponseContentBufferSize = null)
  {
    this._client = new HttpClient(handler ?? LocalRestClient.GetHandler(), disposeHandler);
    this.AddBaseAddress(baseAddress);
    this.AddDefaultHeaders((IEnumerable<KeyValuePair<string, IEnumerable<string>>>) defaultRequestHeaders);
    this.AddRequestTimeout(timeout);
    this.AddMaxResponseBufferSize(maxResponseContentBufferSize);
  }

  private static HttpMessageHandler GetHandler()
  {
    return (HttpMessageHandler) new SocketsHttpHandler()
    {
      PooledConnectionLifetime = LocalRestClient.ConnectionLifeTime,
      PooledConnectionIdleTimeout = LocalRestClient.ConnectionLifeTime,
      MaxConnectionsPerServer = 20
    };
  }

  public IReadOnlyDictionary<string, string[]> DefaultRequestHeaders
  {
    get
    {
      return (IReadOnlyDictionary<string, string[]>) this._client.DefaultRequestHeaders.ToDictionary<KeyValuePair<string, IEnumerable<string>>, string, string[]>((Func<KeyValuePair<string, IEnumerable<string>>, string>) (x => x.Key), (Func<KeyValuePair<string, IEnumerable<string>>, string[]>) (x => x.Value.ToArray<string>()));
    }
  }

  public TimeSpan Timeout => this._client.Timeout;

  public uint MaxResponseContentBufferSize => (uint) this._client.MaxResponseContentBufferSize;

  public Uri BaseAddress => this._client.BaseAddress;

  public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
  {
    return this.SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None);
  }

  public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cToken)
  {
    return this.SendAsync(request, HttpCompletionOption.ResponseContentRead, cToken);
  }

  public Task<HttpResponseMessage> SendAsync(
    HttpRequestMessage request,
    HttpCompletionOption option)
  {
    return this.SendAsync(request, option, CancellationToken.None);
  }

  public Task<HttpResponseMessage> SendAsync(
    HttpRequestMessage request,
    HttpCompletionOption option,
    CancellationToken cToken)
  {
    Ensure.NotNull<HttpRequestMessage>(request, nameof (request));
    this.AddConnectionLeaseTimeout(request.RequestUri);
    return this._client.SendAsync(request, option, cToken);
  }

  public Task<HttpResponseMessage> GetAsync(string uri)
  {
    return this.SendAsync(new HttpRequestMessage(HttpMethod.Get, uri));
  }

  public Task<HttpResponseMessage> GetAsync(string uri, TimeSpan timeout)
  {
    return this.GetAsync(new Uri(uri, UriKind.RelativeOrAbsolute), timeout);
  }

  public Task<HttpResponseMessage> GetAsync(Uri uri)
  {
    return this.SendAsync(new HttpRequestMessage(HttpMethod.Get, uri));
  }

  public async Task<HttpResponseMessage> GetAsync(Uri uri, TimeSpan timeout)
  {
    HttpResponseMessage async;
    using (CancellationTokenSource cts = new CancellationTokenSource(timeout))
      async = await this.SendAsync(new HttpRequestMessage(HttpMethod.Get, uri), cts.Token).ConfigureAwait(false);
    return async;
  }

  public Task<HttpResponseMessage> GetAsync(string uri, CancellationToken cToken)
  {
    return this.SendAsync(new HttpRequestMessage(HttpMethod.Get, uri), cToken);
  }

  public Task<HttpResponseMessage> GetAsync(Uri uri, CancellationToken cToken)
  {
    return this.SendAsync(new HttpRequestMessage(HttpMethod.Get, uri), cToken);
  }

  public Task<HttpResponseMessage> GetAsync(string uri, HttpCompletionOption option)
  {
    return this.SendAsync(new HttpRequestMessage(HttpMethod.Get, uri), option);
  }

  public Task<HttpResponseMessage> GetAsync(Uri uri, HttpCompletionOption option)
  {
    return this.SendAsync(new HttpRequestMessage(HttpMethod.Get, uri), option);
  }

  public Task<HttpResponseMessage> GetAsync(
    string uri,
    HttpCompletionOption option,
    CancellationToken cToken)
  {
    return this.SendAsync(new HttpRequestMessage(HttpMethod.Get, uri), option, cToken);
  }

  public Task<HttpResponseMessage> GetAsync(
    Uri uri,
    HttpCompletionOption option,
    CancellationToken cToken)
  {
    return this.SendAsync(new HttpRequestMessage(HttpMethod.Get, uri), option, cToken);
  }

  public Task<HttpResponseMessage> PutAsync(string uri, HttpContent content)
  {
    return this.SendAsync(new HttpRequestMessage(HttpMethod.Put, uri)
    {
      Content = content
    });
  }

  public Task<HttpResponseMessage> PutAsync(string uri, HttpContent content, TimeSpan timeout)
  {
    return this.PutAsync(new Uri(uri, UriKind.RelativeOrAbsolute), content, timeout);
  }

  public Task<HttpResponseMessage> PutAsync(Uri uri, HttpContent content)
  {
    return this.SendAsync(new HttpRequestMessage(HttpMethod.Put, uri)
    {
      Content = content
    });
  }

  public async Task<HttpResponseMessage> PutAsync(Uri uri, HttpContent content, TimeSpan timeout)
  {
    HttpResponseMessage httpResponseMessage;
    using (CancellationTokenSource cts = new CancellationTokenSource(timeout))
    {
      LocalRestClient localRestClient = this;
      HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, uri);
      request.Content = content;
      CancellationToken token = cts.Token;
      httpResponseMessage = await localRestClient.SendAsync(request, token).ConfigureAwait(false);
    }
    return httpResponseMessage;
  }

  public Task<HttpResponseMessage> PutAsync(
    Uri uri,
    HttpContent content,
    CancellationToken cToken)
  {
    return this.SendAsync(new HttpRequestMessage(HttpMethod.Put, uri)
    {
      Content = content
    }, cToken);
  }

  public Task<HttpResponseMessage> PutAsync(
    string uri,
    HttpContent content,
    CancellationToken cToken)
  {
    return this.SendAsync(new HttpRequestMessage(HttpMethod.Put, uri)
    {
      Content = content
    }, cToken);
  }

  public Task<HttpResponseMessage> PostAsync(string uri, HttpContent content)
  {
    return this.SendAsync(new HttpRequestMessage(HttpMethod.Post, uri)
    {
      Content = content
    });
  }

  public Task<HttpResponseMessage> PostAsync(string uri, HttpContent content, TimeSpan timeout)
  {
    return this.PostAsync(new Uri(uri, UriKind.RelativeOrAbsolute), content, timeout);
  }

  public Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content)
  {
    return this.SendAsync(new HttpRequestMessage(HttpMethod.Post, uri)
    {
      Content = content
    });
  }

  public async Task<HttpResponseMessage> PostAsync(
    Uri uri,
    HttpContent content,
    TimeSpan timeout)
  {
    HttpResponseMessage httpResponseMessage;
    using (CancellationTokenSource cts = new CancellationTokenSource(timeout))
    {
      LocalRestClient localRestClient = this;
      HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
      request.Content = content;
      CancellationToken token = cts.Token;
      httpResponseMessage = await localRestClient.SendAsync(request, token).ConfigureAwait(false);
    }
    return httpResponseMessage;
  }

  public Task<HttpResponseMessage> PostAsync(
    Uri uri,
    HttpContent content,
    CancellationToken cToken)
  {
    return this.SendAsync(new HttpRequestMessage(HttpMethod.Post, uri)
    {
      Content = content
    }, cToken);
  }

  public Task<HttpResponseMessage> PostAsync(
    string uri,
    HttpContent content,
    CancellationToken cToken)
  {
    return this.SendAsync(new HttpRequestMessage(HttpMethod.Post, uri)
    {
      Content = content
    }, cToken);
  }

  public Task<HttpResponseMessage> DeleteAsync(string uri)
  {
    return this.SendAsync(new HttpRequestMessage(HttpMethod.Delete, uri));
  }

  public Task<HttpResponseMessage> DeleteAsync(string uri, TimeSpan timeout)
  {
    return this.DeleteAsync(new Uri(uri, UriKind.RelativeOrAbsolute), timeout);
  }

  public Task<HttpResponseMessage> DeleteAsync(Uri uri)
  {
    return this.SendAsync(new HttpRequestMessage(HttpMethod.Delete, uri));
  }

  public async Task<HttpResponseMessage> DeleteAsync(Uri uri, TimeSpan timeout)
  {
    HttpResponseMessage httpResponseMessage;
    using (CancellationTokenSource cts = new CancellationTokenSource(timeout))
      httpResponseMessage = await this.SendAsync(new HttpRequestMessage(HttpMethod.Delete, uri), cts.Token).ConfigureAwait(false);
    return httpResponseMessage;
  }

  public Task<HttpResponseMessage> DeleteAsync(string uri, CancellationToken cToken)
  {
    return this.SendAsync(new HttpRequestMessage(HttpMethod.Delete, uri), cToken);
  }

  public Task<HttpResponseMessage> DeleteAsync(Uri uri, CancellationToken cToken)
  {
    return this.SendAsync(new HttpRequestMessage(HttpMethod.Delete, uri), cToken);
  }

  public Task<string> GetStringAsync(string uri)
  {
    Ensure.NotNullOrEmptyOrWhiteSpace(uri);
    return this.GetStringAsync(new Uri(uri, UriKind.RelativeOrAbsolute));
  }

  public Task<string> GetStringAsync(string uri, TimeSpan timeout)
  {
    Ensure.NotNullOrEmptyOrWhiteSpace(uri);
    return this.GetStringAsync(new Uri(uri, UriKind.RelativeOrAbsolute), timeout);
  }

  public Task<string> GetStringAsync(Uri uri)
  {
    Ensure.NotNull<Uri>(uri, nameof (uri));
    this.AddConnectionLeaseTimeout(uri);
    return this._client.GetStringAsync(uri);
  }

  public async Task<string> GetStringAsync(Uri uri, TimeSpan timeout)
  {
    Ensure.NotNull<Uri>(uri, nameof (uri));
    HttpResponseMessage httpResponseMessage = await this.GetAsync(uri, timeout).ConfigureAwait(false);
    httpResponseMessage.EnsureSuccessStatusCode();
    return await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
  }

  public Task<Stream> GetStreamAsync(string uri)
  {
    Ensure.NotNullOrEmptyOrWhiteSpace(uri);
    return this.GetStreamAsync(new Uri(uri, UriKind.RelativeOrAbsolute));
  }

  public Task<Stream> GetStreamAsync(string uri, TimeSpan timeout)
  {
    Ensure.NotNullOrEmptyOrWhiteSpace(uri);
    return this.GetStreamAsync(new Uri(uri, UriKind.RelativeOrAbsolute), timeout);
  }

  public Task<Stream> GetStreamAsync(Uri uri)
  {
    Ensure.NotNull<Uri>(uri, nameof (uri));
    this.AddConnectionLeaseTimeout(uri);
    return this._client.GetStreamAsync(uri);
  }

  public async Task<Stream> GetStreamAsync(Uri uri, TimeSpan timeout)
  {
    Ensure.NotNull<Uri>(uri, nameof (uri));
    HttpResponseMessage httpResponseMessage = await this.GetAsync(uri, timeout).ConfigureAwait(false);
    httpResponseMessage.EnsureSuccessStatusCode();
    return await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
  }

  public Task<byte[]> GetByteArrayAsync(string uri)
  {
    Ensure.NotNullOrEmptyOrWhiteSpace(uri);
    return this.GetByteArrayAsync(new Uri(uri, UriKind.RelativeOrAbsolute));
  }

  public Task<byte[]> GetByteArrayAsync(string uri, TimeSpan timeout)
  {
    Ensure.NotNullOrEmptyOrWhiteSpace(uri);
    return this.GetByteArrayAsync(new Uri(uri, UriKind.RelativeOrAbsolute), timeout);
  }

  public Task<byte[]> GetByteArrayAsync(Uri uri)
  {
    Ensure.NotNull<Uri>(uri, nameof (uri));
    this.AddConnectionLeaseTimeout(uri);
    return this._client.GetByteArrayAsync(uri);
  }

  public async Task<byte[]> GetByteArrayAsync(Uri uri, TimeSpan timeout)
  {
    Ensure.NotNull<Uri>(uri, nameof (uri));
    HttpResponseMessage httpResponseMessage = await this.GetAsync(uri, timeout).ConfigureAwait(false);
    httpResponseMessage.EnsureSuccessStatusCode();
    return await httpResponseMessage.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
  }

  public void CancelPendingRequests() => this._client.CancelPendingRequests();

  public void Dispose() => this._client.Dispose();

  private static void ConfigureServicePointManager()
  {
    ServicePointManager.DnsRefreshTimeout = (int) LocalRestClient.ConnectionLifeTime.TotalMilliseconds;
    ServicePointManager.DefaultConnectionLimit = 20;
  }

  private void AddBaseAddress(Uri uri)
  {
    if ((object) uri == null)
      return;
    this.AddConnectionLeaseTimeout(uri);
    this._client.BaseAddress = uri;
  }

  private void AddDefaultHeaders(
    IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers)
  {
    if (headers == null)
      return;
    foreach (KeyValuePair<string, IEnumerable<string>> header in headers)
      this._client.DefaultRequestHeaders.Add(header.Key, header.Value);
  }

  private void AddRequestTimeout(TimeSpan? timeout)
  {
    this._client.Timeout = timeout ?? System.Threading.Timeout.InfiniteTimeSpan;
  }

  private void AddMaxResponseBufferSize(ulong? size)
  {
    if (!size.HasValue)
      return;
    this._client.MaxResponseContentBufferSize = (long) size.Value;
  }

  private void AddConnectionLeaseTimeout(Uri endpoint)
  {
  }
}