using System.Runtime.CompilerServices;
using BuildingBlocks.Ldap.UserStore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Ldap.Extensions;

public static class AddLdapUsersExtension
{
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
        IConfigurationSection section = configuration.GetSection("LdapServerConfiguration");
        ExtensionConfig extensionConfig = (ExtensionConfig)((IConfiguration)section).Get(typeof(ExtensionConfig));
        LdapConfig ldapConfig1 = new LdapConfig();
        ICollection<LdapConfig> connections = extensionConfig.Connections;
        int num;
        if (connections == null)
        {
            num = 1;
        }
        else
        {
            int count = connections.Count;
            num = 0;
        }

        if (num != 0)
        {
            ldapConfig1 = (LdapConfig)((IConfiguration)section).Get(typeof(LdapConfig)) ?? new LdapConfig();
            extensionConfig.Redis = ldapConfig1.Redis;
            extensionConfig.RefreshClaimsInSeconds = ldapConfig1.RefreshClaimsInSeconds ?? 30;
            extensionConfig.Connections = (ICollection<LdapConfig>)new List<LdapConfig>()
            {
                ldapConfig1
            };
        }

        int configIndex = 0;
        extensionConfig.Connections.ToList<LdapConfig>().ForEach((Action<LdapConfig>)(f =>
        {
            ++configIndex;
            LdapConfig ldapConfig2 = f;
            string str;
            if (string.IsNullOrEmpty(f.FriendlyName))
            {
                DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(8, 1);
                interpolatedStringHandler.AppendLiteral("Config #");
                interpolatedStringHandler.AppendFormatted<int>(configIndex);
                str = interpolatedStringHandler.ToStringAndClear();
            }
            else
                str = f.FriendlyName;

            ldapConfig2.FriendlyName = str;
        }));
        builder.Services.AddSingleton<LdapConfig>(ldapConfig1);
        builder.Services.AddSingleton<ExtensionConfig>(extensionConfig);
        builder.Services.AddSingleton<LdapConnectionPool>();
        return ldapConfig1;
    }
}