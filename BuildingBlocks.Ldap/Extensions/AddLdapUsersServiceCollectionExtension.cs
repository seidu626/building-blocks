using BuildingBlocks.Ldap.UserStore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Ldap.Extensions;

public static class AddLdapUsersServiceCollectionExtension
{
    public static IServiceCollection AddLdapUsers<TUserDetails>(
        this IServiceCollection serviceCollection,
        IConfiguration configuration)
        where TUserDetails : IAppUser, new()
    {
        var ldapConfig = RegisterLdapConfigurations(serviceCollection, configuration);
        serviceCollection.AddSingleton<ILdapService<TUserDetails>, LdapService<TUserDetails>>();

        if (ldapConfig.UserStore == UserStore.InMemory)
        {
            serviceCollection.AddSingleton<ILdapUserStore<TUserDetails>, InMemoryUserStore<TUserDetails>>();
        }
        else
        {
            serviceCollection.AddSingleton<ILdapUserStore<TUserDetails>, RedisUserStore<TUserDetails>>();
        }

        return serviceCollection;
    }

    public static IServiceCollection AddLdapUsers<TUserDetails, TCustomUserStore>(
        this IServiceCollection serviceCollection,
        IConfiguration configuration,
        ILdapUserStore<TUserDetails> customUserStore)
        where TUserDetails : IAppUser, new()
    {
        RegisterLdapConfigurations(serviceCollection, configuration);
        serviceCollection.AddSingleton<ILdapService<TUserDetails>, LdapService<TUserDetails>>();
        serviceCollection.AddSingleton(customUserStore);
        return serviceCollection;
    }

    private static LdapConfig RegisterLdapConfigurations(
        IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        var section = configuration.GetSection("LdapServerConfiguration");
        var extensionConfig = section.Get<ExtensionConfig>() ?? new ExtensionConfig();
        var ldapConfig = section.Get<LdapConfig>() ?? new LdapConfig();

        if (extensionConfig.Connections == null || extensionConfig.Connections.Count == 0)
        {
            extensionConfig.Redis = ldapConfig.Redis;
            extensionConfig.RefreshClaimsInSeconds = ldapConfig.RefreshClaimsInSeconds ?? 1800;
            extensionConfig.Connections = new List<LdapConfig> { ldapConfig };
        }

        int configIndex = 0;
        extensionConfig.Connections.ToList().ForEach(f =>
        {
            configIndex++;
            f.FriendlyName ??= $"Config #{configIndex}";
        });

        serviceCollection.AddSingleton(ldapConfig);
        serviceCollection.AddSingleton(extensionConfig);
        serviceCollection.AddSingleton<LdapConnectionPool>();
        return ldapConfig;
    }
}