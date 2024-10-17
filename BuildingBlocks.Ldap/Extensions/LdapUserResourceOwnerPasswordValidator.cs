using System.Security.Claims;
using BuildingBlocks.Exceptions;
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
            // Validate the user credentials asynchronously.
            Result<TUser, Error> result = await _users.ValidateCredentialsAsync(context.UserName, context.Password);

            if (result.IsSuccess && result.Value != null)
            {
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
}