// Decompiled with JetBrains decompiler
// Type: Skoruba.Duende.IdentityServer.STS.Identity.Infrastructure.LdapExtension.UserModel.OpenLdapAppUser
// Assembly: Skoruba.Duende.IdentityServer.STS.Identity, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 425C0317-D64B-453B-BC95-043C64DD9F8A
// Assembly location: C:\Users\420919\Repositories\STS\STS\Skoruba.Duende.IdentityServer.STS.Identity.dll

#nullable disable
using System.Security.Claims;
using BuildingBlocks.Ldap.Extensions;
using Novell.Directory.Ldap;

namespace BuildingBlocks.Ldap.UserModel;

public class OpenLdapAppUser : IAppUser
{
  private string _subjectId;

  public string SubjectId
  {
    get => this._subjectId ?? this.Username;
    set => this._subjectId = value;
  }

  public string ProviderSubjectId { get; set; }

  public string ProviderName { get; set; }

  public string DisplayName { get; set; }

  public string Username { get; set; }

  public string Fullname { get; set; }

  public string Email { get; set; }

  public bool IsActive
  {
    get => true;
    set
    {
    }
  }

  public ICollection<Claim> Claims { get; set; }

  public string[] LdapAttributes => Enum<OpenLdapAttributes>.Descriptions;

  public void FillClaims(LdapEntry user, LdapEntry? manager = null)
  {
    this.Claims = (ICollection<Claim>) new List<Claim>()
    {
      this.GetClaimFromLdapAttributes(user, "name", OpenLdapAttributes.DisplayName),
      this.GetClaimFromLdapAttributes(user, "family_name", OpenLdapAttributes.LastName),
      this.GetClaimFromLdapAttributes(user, "given_name", OpenLdapAttributes.FirstName),
      this.GetClaimFromLdapAttributes(user, "email", OpenLdapAttributes.EMail),
      this.GetClaimFromLdapAttributes(user, "phone_number", OpenLdapAttributes.TelephoneNumber)
    };
    try
    {
      IEnumerator<string> stringValues = user.GetAttribute(OpenLdapAttributes.MemberOf.ToDescriptionString()).StringValues;
      while (stringValues.MoveNext())
        this.Claims.Add(new Claim("role", stringValues.Current.ToString()));
    }
    catch (Exception ex)
    {
    }
  }

  public static string[] RequestedLdapAttributes() => throw new NotImplementedException();

  internal Claim GetClaimFromLdapAttributes(
    LdapEntry user,
    string claim,
    OpenLdapAttributes ldapAttribute)
  {
    string str = string.Empty;
    try
    {
      str = user.GetAttribute(ldapAttribute.ToDescriptionString()).StringValue;
      return new Claim(claim, str);
    }
    catch (Exception ex)
    {
    }
    return new Claim(claim, str);
  }

  public void SetBaseDetails(LdapEntry ldapEntry, LdapEntry manager = null, string providerName = "")
  {
    this.DisplayName = ldapEntry.GetAttribute(OpenLdapAttributes.DisplayName.ToDescriptionString()).StringValue;
    this.Username = ldapEntry.GetAttribute(OpenLdapAttributes.UserName.ToDescriptionString()).StringValue;
    try
    {
      this.Email = ldapEntry.GetAttribute(OpenLdapAttributes.EMail.ToDescriptionString()).StringValue;
    }
    catch (Exception ex)
    {
      this.Email = this.Username + "@mtn.com";
    }
    this.ProviderName = providerName;
    this.SubjectId = this.Username;
    this.ProviderSubjectId = this.Username;
    this.FillClaims(ldapEntry);
  }
}