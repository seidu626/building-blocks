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
    Task<Result<TUser, Error>> ValidateCredentialsAsync(string username, string password);

    Task<Result<TUser, Error>> ValidateCredentialsAsync(string username, string password, string domain);

    Task<Result<TUser, Error>> FindBySubjectIdAsync(string subjectId);

    Task<Result<TUser, Error>> FindByUsernameAsync(string username);

    Task<Result<TUser, Error>> FindByEmailAsync(string email);
    Task<Result<TUser, Error>> FindByPhoneAsync(string phone);

    Task<Result<List<string>, Error>> GetUserAttributesAsync(string attributeName, string attributeValue, string domain);

    Task<Result<TUser, Error>> FindByExternalProviderAsync(string provider, string userId);

    Task<Result<TUser, Error>> AutoProvisionUserAsync(string provider, string userId, List<Claim> claims);
}