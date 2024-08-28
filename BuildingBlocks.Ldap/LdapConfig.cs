// Decompiled with JetBrains decompiler
// Type: Skoruba.Duende.IdentityServer.STS.Identity.Infrastructure.LdapExtension.LdapConfig
// Assembly: Skoruba.Duende.IdentityServer.STS.Identity, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 425C0317-D64B-453B-BC95-043C64DD9F8A
// Assembly location: C:\Users\420919\Repositories\STS\STS\Skoruba.Duende.IdentityServer.STS.Identity.dll

#nullable disable
using System.Text.RegularExpressions;

namespace BuildingBlocks.Ldap;

public class LdapConfig
{
  private Regex _preFilterRegex;
  private string _preFilterRegexString;

  public string ValidationCertificatePfxFilePath { get; set; }

  public string ValidationCertificatePfxFilePassword { get; set; }

  public string FriendlyName { get; set; }

  public string Url { get; set; }

  public int Port { get; set; } = 389;

  public bool Ssl { get; set; }

  public Extensions.UserStore UserStore { get; set; }

  public string BindDn { get; set; }

  public string BindCredentials { get; set; }

  public string SearchBase { get; set; }

  public string SearchFilter { get; set; }
  public string UserSearchFilter { get; set; }
  public string ManagerSearchFilter { get; set; }

  public string Redis { get; set; }

  public string PreFilterRegex
  {
    get => this._preFilterRegexString;
    set
    {
      this._preFilterRegex = new Regex(value, RegexOptions.Compiled);
      this._preFilterRegexString = value;
    }
  }

  public uint? RefreshClaimsInSeconds { get; set; }

  public string[] ExtraAttributes { get; set; }

  internal bool IsConcerned(string username)
  {
    return this._preFilterRegex == null || this._preFilterRegex.IsMatch(username);
  }

  internal int FinalLdapConnectionPort
  {
    get
    {
      if (this.Port != 0)
        return this.Port;
      return !this.Ssl ? 389 : 636;
    }
  }
}