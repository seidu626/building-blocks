// Decompiled with JetBrains decompiler
// Type: Skoruba.Duende.IdentityServer.STS.Identity.Infrastructure.LdapExtension.Extensions.LdapUserResourceOwnerPasswordValidator`1
// Assembly: Skoruba.Duende.IdentityServer.STS.Identity, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 425C0317-D64B-453B-BC95-043C64DD9F8A
// Assembly location: C:\Users\420919\Repositories\STS\STS\Skoruba.Duende.IdentityServer.STS.Identity.dll

#nullable disable
using System.Security.Claims;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Ldap.UserStore;
using CSharpFunctionalExtensions;
using Duende.IdentityServer.Validation;
using Microsoft.AspNetCore.Authentication;

namespace BuildingBlocks.Ldap.Extensions;

public class LdapUserResourceOwnerPasswordValidator<TUser> : IResourceOwnerPasswordValidator
    where TUser : IAppUser, new()
{
    private readonly ILdapUserStore<TUser> _users;
    private readonly ISystemClock _clock;

    public LdapUserResourceOwnerPasswordValidator(ILdapUserStore<TUser> users, ISystemClock clock)
    {
        this._users = users;
        this._clock = clock;
    }

    public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
    {
        Result<TUser, Error> result = this._users.ValidateCredentials(context.UserName, context.Password);
        if ((object)result.Value != null)
        {
            ResourceOwnerPasswordValidationContext validationContext = context;
            GrantValidationResult validationResult = new GrantValidationResult(
                result.Value.SubjectId ?? throw new ArgumentException("Subject ID not set", "SubjectId"), "pwd",
                this._clock.UtcNow.UtcDateTime, (IEnumerable<Claim>)result.Value.Claims);
            validationContext.Result = validationResult;
        }

        return Task.CompletedTask;
    }
}