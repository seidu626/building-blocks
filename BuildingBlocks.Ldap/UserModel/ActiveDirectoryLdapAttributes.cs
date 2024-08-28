// Decompiled with JetBrains decompiler
// Type: Skoruba.Duende.IdentityServer.STS.Identity.Infrastructure.LdapExtension.UserModel.ActiveDirectoryLdapAttributes
// Assembly: Skoruba.Duende.IdentityServer.STS.Identity, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 425C0317-D64B-453B-BC95-043C64DD9F8A
// Assembly location: C:\Users\420919\Repositories\STS\STS\Skoruba.Duende.IdentityServer.STS.Identity.dll

#nullable disable
namespace BuildingBlocks.Ldap.UserModel;

public enum ActiveDirectoryLdapAttributes
{
  [System.ComponentModel.Description("displayName")] DisplayName,
  [System.ComponentModel.Description("givenName")] FirstName,
  [System.ComponentModel.Description("sn")] LastName,
  [System.ComponentModel.Description("description")] Description,
  [System.ComponentModel.Description("cn")] FullName,
  [System.ComponentModel.Description("mobile")] TelephoneNumber,
  [System.ComponentModel.Description("name")] Name,
  [System.ComponentModel.Description("whenCreated")] CreatedOn,
  [System.ComponentModel.Description("whenChanged")] UpdatedOn,
  [System.ComponentModel.Description("sAMAccountName")] UserName,
  [System.ComponentModel.Description("mail")] EMail,
  [System.ComponentModel.Description("title")] Tile,
  [System.ComponentModel.Description("employeeID")] EmployeeNumber,
  [System.ComponentModel.Description("department")] Department,
  [System.ComponentModel.Description("division")] Division,
  [System.ComponentModel.Description("lockoutTime")] LockoutTime,
  [System.ComponentModel.Description("badPwdCount")] BadPwdCount,
  [System.ComponentModel.Description("pwdLastSet")] PwdLastSet,
  [System.ComponentModel.Description("accountExpires")] AccountExpires,
  [System.ComponentModel.Description("memberOf")] MemberOf,
  [System.ComponentModel.Description("employeeType")] EmployeeType,
  [System.ComponentModel.Description("manager")] Manager
}