#nullable disable
namespace BuildingBlocks.Ldap;

public class ExtensionConfig
{
  public string Redis { get; set; }

  public ICollection<LdapConfig> Connections { get; set; }

  public uint RefreshClaimsInSeconds { get; set; } = 1800;
}