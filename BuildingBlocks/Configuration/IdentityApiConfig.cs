#nullable disable
namespace BuildingBlocks.Configuration;

public class IdentityApiConfig : IConfig
{
  public bool ShowPII { get; set; }
  public string ClientId { get; set; }
  
  public string ApiName { get; set; }
  public string ApiVersion { get; set; }

  public string IdentityServerBaseUrl { get; set; }
  public string ApiBaseUrl { get; set; }

  public bool RequireHttpsMetadata { get; set; }
  public string OidcApiName { get; set; }

  public bool CorsAllowAnyOrigin { get; set; }

  public string[] CorsAllowOrigins { get; set; }
  
  public string SecurityScheme { get; set; }

  public Dictionary<string, string> Scopes { get; set; }

  public List<string> Permissions { get; set; }
}