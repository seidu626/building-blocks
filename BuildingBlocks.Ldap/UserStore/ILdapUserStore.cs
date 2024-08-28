// Decompiled with JetBrains decompiler
// Type: Skoruba.Duende.IdentityServer.STS.Identity.Infrastructure.LdapExtension.UserStore.ILdapUserStore
// Assembly: Skoruba.Duende.IdentityServer.STS.Identity, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 425C0317-D64B-453B-BC95-043C64DD9F8A
// Assembly location: C:\Users\420919\Repositories\STS\STS\Skoruba.Duende.IdentityServer.STS.Identity.dll

#nullable disable
using System.Security.Claims;
using BuildingBlocks.Exceptions;
using CSharpFunctionalExtensions;

namespace BuildingBlocks.Ldap.UserStore;

public interface ILdapUserStore<TUser> where TUser : IAppUser, new()
{
    Result<TUser, Error> ValidateCredentials(string username, string password);

    Result<TUser, Error> ValidateCredentials(string username, string password, string domain);

    Result<TUser, Error> FindBySubjectId(string subjectId);

    Result<TUser, Error> FindByUsername(string username);

    Result<TUser, Error> FindByEmail(string email);

    Result<List<string>, Error> GetUserAttributes(string attributeName, string attributeValue, string domain);

    Result<TUser, Error> FindByExternalProvider(string provider, string userId);

    Result<TUser, Error> AutoProvisionUser(string provider, string userId, List<Claim> claims);
}