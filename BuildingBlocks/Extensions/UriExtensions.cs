// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Extensions.UriExtensions
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable enable
using System.Net;
using System.Text.RegularExpressions;
using BuildingBlocks.Common;

namespace BuildingBlocks.Extensions;

public static class UriExtensions
{
  private static readonly 
#nullable disable
    Regex QueryStringRegex = new Regex("[?|&]([%23\\w\\.-]+)=([^?|^&]+)", RegexOptions.Compiled);

  public static IEnumerable<KeyValuePair<string, string>> ParseQueryString(this Uri uri)
  {
    Ensure.NotNull<Uri>(uri, nameof (uri));
    for (Match match = UriExtensions.QueryStringRegex.Match(uri.OriginalString); match.Success; match = match.NextMatch())
      yield return new KeyValuePair<string, string>(WebUtility.UrlDecode(match.Groups[1].Value), WebUtility.UrlDecode(match.Groups[2].Value));
  }

  public static Uri AddParametersToQueryString(this Uri uri, string parameter, string value)
  {
    Ensure.NotNull<Uri>(uri, nameof (uri));
    Ensure.NotNullOrEmptyOrWhiteSpace(parameter);
    Ensure.NotNullOrEmptyOrWhiteSpace(value);
    string query = WebUtility.UrlEncode(parameter) + "=" + WebUtility.UrlEncode(value);
    return UriExtensions.AddOrAppendToQueryString(uri, query);
  }

  public static Uri AddParametersToQueryString(this Uri uri, IDictionary<string, string> pairs)
  {
    Ensure.NotNull<Uri>(uri, nameof (uri));
    Ensure.NotNull<IDictionary<string, string>>(pairs, nameof (pairs));
    if (!pairs.Any<KeyValuePair<string, string>>())
      return uri;
    IEnumerable<string> values = pairs.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (kv => WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value)));
    return UriExtensions.AddOrAppendToQueryString(uri, string.Join("&", values));
  }

  private static Uri AddOrAppendToQueryString(Uri uri, string query)
  {
    UriBuilder uriBuilder = new UriBuilder(uri);
    uriBuilder.Query = uriBuilder.Query.Length <= 1 ? query : uri.Query.Substring(1) + "&" + query;
    return uriBuilder.Uri;
  }
}