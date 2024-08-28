// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Ldap.LdapConnectionPool
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Collections.Concurrent;
using Novell.Directory.Ldap;
using Serilog;
namespace BuildingBlocks.Ldap;

public class LdapConnectionPool
{
    private readonly ConcurrentBag<LdapConnection> _connections;
    private readonly string _ldapHost;
    private readonly int _ldapPort;
    private readonly string _loginDn;
    private readonly string _password;
    private readonly ILogger _logger;

    public LdapConnectionPool(LdapConfig config, ILogger logger)
    {
        this._ldapHost = config.Url;
        this._ldapPort = config.FinalLdapConnectionPort;
        this._loginDn = config.BindDn;
        this._password = config.BindCredentials;
        this._logger = logger;
        this._connections = new ConcurrentBag<LdapConnection>();
    }

    private LdapConnection CreateNewConnection()
    {
        LdapConnection newConnection = new LdapConnection();
        newConnection.Connect(this._ldapHost, this._ldapPort);
        newConnection.Bind(this._loginDn, this._password);
        return newConnection;
    }

    public LdapConnection AcquireConnection()
    {
        LdapConnection result;
        return this._connections.TryTake(out result) ? result : this.CreateNewConnection();
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
                this._logger.Error(ex, ex.Message);
            }
        }
    }
}