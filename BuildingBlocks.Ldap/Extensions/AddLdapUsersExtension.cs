using BuildingBlocks.Ldap.UserStore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Ldap.Extensions;

public static class AddLdapUsersExtension
{
    /// <summary>
    /// Adds the LDAP users mechanism to IdentityServer.
    /// </summary>
    /// <typeparam name="TUserDetails">The type of the user details.</typeparam>
    /// <param name="builder">The builder.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>
    /// Returns the builder instance.   
    /// </returns>
    public static IIdentityServerBuilder AddLdapUsers<TUserDetails>(
        this IIdentityServerBuilder builder,
        IConfiguration configuration)
        where TUserDetails : IAppUser, new()
    {
        LdapConfig ldapConfig = AddLdapUsersExtension.RegisterLdapConfigurations(builder, configuration);
        builder.Services.AddSingleton<ILdapService<TUserDetails>, LdapService<TUserDetails>>();
        if (ldapConfig.UserStore == UserStore.InMemory)
            builder.Services.AddSingleton<ILdapUserStore<TUserDetails>, InMemoryUserStore<TUserDetails>>();
        else
            builder.Services.AddSingleton<ILdapUserStore<TUserDetails>, RedisUserStore<TUserDetails>>();
        return builder;
    }

    /// <summary>
    /// Adds Ldap Users to identity server.
    /// </summary>
    /// <typeparam name="TUserDetails">The type of the user details.</typeparam>
    /// <typeparam name="TCustomUserStore">The type of the custom user store.</typeparam>
    /// <param name="builder">The builder.</param>
    /// <param name="configuration">The ldap configuration.</param>
    /// <param name="customUserStore">The custom user store (ILdapUserStore).</param>
    /// <returns>
    /// Returns the builder instance
    /// </returns>
    public static IIdentityServerBuilder AddLdapUsers<TUserDetails, TCustomUserStore>(
        this IIdentityServerBuilder builder,
        IConfiguration configuration,
        ILdapUserStore<TUserDetails> customUserStore)
        where TUserDetails : IAppUser, new()
    {
        AddLdapUsersExtension.RegisterLdapConfigurations(builder, configuration);
        builder.Services.AddSingleton<ILdapService<TUserDetails>, LdapService<TUserDetails>>();
        builder.Services.AddSingleton<ILdapUserStore<TUserDetails>>(customUserStore);
        builder.AddProfileService<LdapUserProfileService<TUserDetails>>();
        builder.AddResourceOwnerValidator<LdapUserResourceOwnerPasswordValidator<TUserDetails>>();
        return builder;
    }

    private static LdapConfig RegisterLdapConfigurations(
        IIdentityServerBuilder builder,
        IConfiguration configuration)
    {
        var section = configuration.GetSection("LdapServerConfiguration");
        var extensionConfig = section.Get<ExtensionConfig>() ?? new ExtensionConfig();
        var ldapConfig = extensionConfig.Connections?.FirstOrDefault() ?? section.Get<LdapConfig>() ?? new LdapConfig();

        if (extensionConfig.Connections == null || !extensionConfig.Connections.Any())
        {
            ldapConfig.RefreshClaimsInSeconds ??= 1800;
            extensionConfig.Connections = new List<LdapConfig> { ldapConfig };
        }

        SetFriendlyNames(extensionConfig.Connections);
        builder.Services.AddSingleton(ldapConfig);
        builder.Services.AddSingleton(extensionConfig);
        builder.Services.AddSingleton<LdapConnectionPool>();

        return ldapConfig;
    }

    private static void SetFriendlyNames(ICollection<LdapConfig> connections)
    {
        int index = 1;
        foreach (var config in connections)
        {
            config.FriendlyName ??= $"Config #{index++}";
        }
    }
}