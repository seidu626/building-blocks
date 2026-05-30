using System.Security.Claims;
using BuildingBlocks.Common;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Ldap;
using BuildingBlocks.Ldap.UserModel;
using BuildingBlocks.Ldap.UserStore;
using CSharpFunctionalExtensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Ldap.Extensions;

public class LdapUserResourceOwnerPasswordValidator<TUser> : IResourceOwnerPasswordValidator
    where TUser : IAppUser, new()
{
    private readonly ILdapUserStore<TUser> _users;
    private readonly ISystemClock _clock;
    private readonly ILogger<LdapUserResourceOwnerPasswordValidator<TUser>> _logger;

    public LdapUserResourceOwnerPasswordValidator(
        ILdapUserStore<TUser> users,
        ISystemClock clock,
        ILogger<LdapUserResourceOwnerPasswordValidator<TUser>> logger)
    {
        _users = users ?? throw new ArgumentNullException(nameof(users));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(context.UserName))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid username.");
                return;
            }

            var identifierType = IdentifierTypeChecker.GetIdentifierType(context.UserName);

            // Validate the user credentials asynchronously.
            Result<TUser, Error> result = await _users.ValidateCredentialsAsync(context.UserName, context.Password, identifierType);

            if (result.IsSuccess && result.Value != null)
            {
                var accountStateError = GetLdapAccountStateError(result.Value);
                if (accountStateError != null)
                {
                    _logger.LogWarning("User {Username} is not allowed to authenticate. Error: {Error}", context.UserName,
                        accountStateError.FriendlyMessage);
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant,
                        accountStateError.FriendlyMessage);
                    return;
                }

                _logger.LogInformation("User {Username} successfully authenticated.", context.UserName);

                // Create the grant validation result with the user's claims.
                context.Result = new GrantValidationResult(
                    result.Value.SubjectId ??
                    throw new ArgumentException("Subject ID not set", nameof(result.Value.SubjectId)),
                    "pwd",
                    _clock.UtcNow.UtcDateTime,
                    result.Value.Claims);
            }
            else
            {
                _logger.LogWarning("Invalid credentials for user {Username}. Error: {Error}", context.UserName,
                    result.Error?.FriendlyMessage);
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid credentials.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while validating the credentials for user {Username}",
                context.UserName);
            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant,
                "An error occurred during authentication.");
        }
    }

    private static Error? GetLdapAccountStateError(IAppUser user)
    {
        if (!user.IsActive)
        {
            return new Error("ACCOUNT_DISABLED", "LDAP account is disabled.");
        }

        if (user is ActiveDirectoryAppUser activeDirectoryUser && activeDirectoryUser.IsSuspended)
        {
            return new Error("ACCOUNT_LOCKED_OUT", "LDAP account is temporarily suspended.");
        }

        return null;
    }
}
