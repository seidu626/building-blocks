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

    /// <summary>
    /// Timeout in milliseconds
    /// </summary>
    public int Timeout { get; set; } = 120000;

    public int MaxPoolSize { get; set; } = 10;

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