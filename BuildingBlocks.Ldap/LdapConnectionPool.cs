// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Ldap.LdapConnectionPool
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Collections.Concurrent;
using Novell.Directory.Ldap;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Ldap;

public class LdapConnectionPool
{
    private readonly ConcurrentBag<LdapConnection> _connections;
    private readonly string _ldapHost;
    private readonly int _ldapPort;
    private readonly string _loginDn;
    private readonly string _password;
    private readonly ILogger _logger;

    public LdapConnectionPool(LdapConfig config, ILogger<LdapConnectionPool> logger)
    {
        this._ldapHost = config.Url;
        this._ldapPort = config.FinalLdapConnectionPort;
        this._loginDn = config.BindDn;
        this._password = config.BindCredentials;
        this._logger = logger;
        this._connections = new ConcurrentBag<LdapConnection>();
    }

    private async Task<LdapConnection> CreateNewConnection()
    {
        LdapConnection newConnection = new LdapConnection();
        await newConnection.ConnectAsync(this._ldapHost, this._ldapPort);
        await newConnection.BindAsync(this._loginDn, this._password);
        return newConnection;
    }

    public async Task<LdapConnection> AcquireConnectionAsync()
    {
        return this._connections.TryTake(out var result) ? result : await this.CreateNewConnection();
    }

    public void ReleaseConnection(LdapConnection connection) => this._connections.Add(connection);

    public void CloseAllConnections()
    {
        foreach (LdapConnection connection in this._connections)
        {
            try
            {
                connection.Disconnect();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, ex.Message);
            }
        }
    }
}