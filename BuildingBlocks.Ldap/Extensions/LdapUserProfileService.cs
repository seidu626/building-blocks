#nullable enable
using System.Security.Claims;
using System.Security.Principal;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Ldap.UserStore;
using CSharpFunctionalExtensions;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BuildingBlocks.Ldap.Extensions
{
    public class LdapUserProfileService<TUser> : IProfileService where TUser : IAppUser, new()
    {
        protected readonly ILogger Logger;
        protected readonly ILdapUserStore<TUser> Users;

        public LdapUserProfileService(ILdapUserStore<TUser> users, ILogger<LdapUserProfileService<TUser>> logger)
        {
            this.Users = users ?? throw new ArgumentNullException(nameof(users));
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            context.LogProfileRequest(this.Logger);

            try
            {
                if (context.RequestedClaimTypes.Any())
                {
                    // Find the user by subject ID (this is typically the user's unique identifier in the store)
                    var userResult = await Users.FindBySubjectIdAsync(context.Subject.GetSubjectId());

                    if (userResult.IsSuccess)
                    {
                        var user = userResult.Value;

                        // Add the requested claims from the user
                        context.AddRequestedClaims(user.Claims);
                    }
                    else
                    {
                        Logger.LogError("Failed to retrieve user claims for subject ID: {subjectId}. Error: {error}",
                            context.Subject.GetSubjectId(), userResult.Error);
                    }
                }

                context.LogIssuedClaims(this.Logger);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "An error occurred while getting profile data for subject ID: {subjectId}",
                    context.Subject.GetSubjectId());
                throw; // You can choose to rethrow or return a failed result.
            }
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            Logger.LogDebug("IsActive called from: {caller}", context.Caller);

            try
            {
                // Find the user by subject ID
                var userResult = await Users.FindBySubjectIdAsync(context.Subject.GetSubjectId());

                if (userResult.IsSuccess && userResult.Value != null)
                {
                    // Check if the user is active
                    context.IsActive = userResult.Value.IsActive;
                }
                else
                {
                    // Log failure to retrieve user or inactive user
                    Logger.LogWarning("User with subject ID: {subjectId} is not found or inactive. Error: {error}",
                        context.Subject.GetSubjectId(), userResult.Error?.FriendlyMessage);
                    context.IsActive = false;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "An error occurred while determining if user is active for subject ID: {subjectId}",
                    context.Subject.GetSubjectId());
                context.IsActive = false; // Fail safely
            }
        }
    }
}