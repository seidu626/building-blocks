// Decompiled with JetBrains decompiler
// Type: Skoruba.Duende.IdentityServer.STS.Identity.Infrastructure.LdapExtension.UserModel.OpenLdapAttributes
// Assembly: Skoruba.Duende.IdentityServer.STS.Identity, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 425C0317-D64B-453B-BC95-043C64DD9F8A
// Assembly location: C:\Users\420919\Repositories\STS\STS\Skoruba.Duende.IdentityServer.STS.Identity.dll

#nullable disable
namespace BuildingBlocks.Ldap.UserModel;

public enum OpenLdapAttributes
{
  [System.ComponentModel.Description("displayName")] DisplayName,
  [System.ComponentModel.Description("givenName")] FirstName,
  [System.ComponentModel.Description("sn")] LastName,
  [System.ComponentModel.Description("description")] Description,
  [System.ComponentModel.Description("telephoneNumber")] TelephoneNumber,
  [System.ComponentModel.Description("uid")] Name,
  [System.ComponentModel.Description("uid")] UserName,
  [System.ComponentModel.Description("mail")] EMail,
  [System.ComponentModel.Description("memberOf")] MemberOf,
}