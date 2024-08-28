// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Ldap.Extensions.LdapUserProfileService`1
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Runtime.InteropServices.JavaScript;
using System.Security.Claims;
using System.Security.Principal;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Ldap.UserStore;
using CSharpFunctionalExtensions;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Ldap.Extensions;

public class LdapUserProfileService<TUser> : IProfileService where TUser : IAppUser, new()
{
    protected readonly ILogger Logger;
    protected readonly ILdapUserStore<TUser> Users;

    public LdapUserProfileService(
        ILdapUserStore<TUser> users,
        ILogger<LdapUserProfileService<TUser>> logger)
    {
        this.Users = users;
        this.Logger = (ILogger)logger;
    }

    public Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        context.LogProfileRequest(this.Logger);
        if (context.RequestedClaimTypes.Any<string>())
        {
            Result<TUser, Error> bySubjectId =
                this.Users.FindBySubjectId(((IPrincipal)context.Subject).GetSubjectId());
            context.AddRequestedClaims((IEnumerable<Claim>)bySubjectId.Value.Claims);
        }

        context.LogIssuedClaims(this.Logger);
        return Task.CompletedTask;
    }

    public Task IsActiveAsync(IsActiveContext context)
    {
        this.Logger.LogDebug("IsActive called from: {caller}", (object)context.Caller);
        Result<TUser, Error> bySubjectId =
            this.Users.FindBySubjectId(((IPrincipal)context.Subject).GetSubjectId());
        IsActiveContext isActiveContext = context;
        TUser user = bySubjectId.Value;
        ref TUser local = ref user;
        int num = (object)local != null ? (local.IsActive ? 1 : 0) : 0;
        isActiveContext.IsActive = num != 0;
        return Task.CompletedTask;
    }
}