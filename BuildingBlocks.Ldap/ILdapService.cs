// Decompiled with JetBrains decompiler
// Type: Skoruba.Duende.IdentityServer.STS.Identity.Infrastructure.LdapExtension.ILdapService`1
// Assembly: Skoruba.Duende.IdentityServer.STS.Identity, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 425C0317-D64B-453B-BC95-043C64DD9F8A
// Assembly location: C:\Users\420919\Repositories\STS\STS\Skoruba.Duende.IdentityServer.STS.Identity.dll

#nullable disable
using BuildingBlocks.Exceptions;
using CSharpFunctionalExtensions;

namespace BuildingBlocks.Ldap;

public interface ILdapService<TUser> where TUser : IAppUser, new()
{
  Result<TUser, Error> Login(string username, string password);

  Result<TUser, Error> Login(string username, string password, string domain);

  Result<TUser, Error> FindUser(string username);

  Result<TUser, Error> FindUser(string username, string domain);

  Result<TUser, Error> FindUserByEmail(string email);

  Result<TUser, Error> FindUserByEmail(string email, string domain);

  Result<List<string>, Error> GetUserAttributes(string attributeName, string attributeValue, string domain);
}