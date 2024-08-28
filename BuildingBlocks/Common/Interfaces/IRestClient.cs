// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Common.Interfaces.IRestClient
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
namespace BuildingBlocks.Common.Interfaces;

public interface IRestClient : IDisposable
{
  IReadOnlyDictionary<string, string[]> DefaultRequestHeaders { get; }

  TimeSpan Timeout { get; }

  uint MaxResponseContentBufferSize { get; }

  Uri BaseAddress { get; }

  Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);

  Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cToken);

  Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption option);

  Task<HttpResponseMessage> SendAsync(
    HttpRequestMessage request,
    HttpCompletionOption option,
    CancellationToken cToken);

  Task<HttpResponseMessage> PutAsync(string uri, HttpContent content);

  Task<HttpResponseMessage> PutAsync(string uri, HttpContent content, TimeSpan timeout);

  Task<HttpResponseMessage> PutAsync(Uri uri, HttpContent content);

  Task<HttpResponseMessage> PutAsync(Uri uri, HttpContent content, TimeSpan timeout);

  Task<HttpResponseMessage> PutAsync(Uri uri, HttpContent content, CancellationToken cToken);

  Task<HttpResponseMessage> PutAsync(string uri, HttpContent content, CancellationToken cToken);

  Task<HttpResponseMessage> PostAsync(string uri, HttpContent content);

  Task<HttpResponseMessage> PostAsync(string uri, HttpContent content, TimeSpan timeout);

  Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content);

  Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content, TimeSpan timeout);

  Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content, CancellationToken cToken);

  Task<HttpResponseMessage> PostAsync(string uri, HttpContent content, CancellationToken cToken);

  Task<string> GetStringAsync(string uri);

  Task<string> GetStringAsync(string uri, TimeSpan timeout);

  Task<string> GetStringAsync(Uri uri);

  Task<string> GetStringAsync(Uri uri, TimeSpan timeout);

  Task<Stream> GetStreamAsync(string uri);

  Task<Stream> GetStreamAsync(string uri, TimeSpan timeout);

  Task<Stream> GetStreamAsync(Uri uri);

  Task<Stream> GetStreamAsync(Uri uri, TimeSpan timeout);

  Task<byte[]> GetByteArrayAsync(string uri);

  Task<byte[]> GetByteArrayAsync(string uri, TimeSpan timeout);

  Task<byte[]> GetByteArrayAsync(Uri uri);

  Task<byte[]> GetByteArrayAsync(Uri uri, TimeSpan timeout);

  Task<HttpResponseMessage> GetAsync(string uri);

  Task<HttpResponseMessage> GetAsync(string uri, TimeSpan timeout);

  Task<HttpResponseMessage> GetAsync(Uri uri);

  Task<HttpResponseMessage> GetAsync(Uri uri, TimeSpan timeout);

  Task<HttpResponseMessage> GetAsync(string uri, CancellationToken cToken);

  Task<HttpResponseMessage> GetAsync(Uri uri, CancellationToken cToken);

  Task<HttpResponseMessage> GetAsync(string uri, HttpCompletionOption option);

  Task<HttpResponseMessage> GetAsync(Uri uri, HttpCompletionOption option);

  Task<HttpResponseMessage> GetAsync(
    string uri,
    HttpCompletionOption option,
    CancellationToken cToken);

  Task<HttpResponseMessage> GetAsync(
    Uri uri,
    HttpCompletionOption option,
    CancellationToken cToken);

  Task<HttpResponseMessage> DeleteAsync(string uri);

  Task<HttpResponseMessage> DeleteAsync(string uri, TimeSpan timeout);

  Task<HttpResponseMessage> DeleteAsync(Uri uri);

  Task<HttpResponseMessage> DeleteAsync(Uri uri, TimeSpan timeout);

  Task<HttpResponseMessage> DeleteAsync(string uri, CancellationToken cToken);

  Task<HttpResponseMessage> DeleteAsync(Uri uri, CancellationToken cToken);

  void CancelPendingRequests();
}