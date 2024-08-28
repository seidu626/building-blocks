using System.Diagnostics;
using System.Net.Security;
using BuildingBlocks.Exceptions;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Novell.Directory.Ldap;
using Novell.Directory.Ldap.Utilclass;

namespace BuildingBlocks.Ldap
{
    public class LdapService<TUser> : ILdapService<TUser> where TUser : IAppUser, new()
    {
        private readonly ILogger<LdapService<TUser>> _logger;
        private readonly ICollection<LdapConfig> _config;
        private readonly Dictionary<string, LdapConnection> _ldapConnections = new();

        public LdapService(ExtensionConfig config, ILogger<LdapService<TUser>> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config.Connections ?? throw new ArgumentNullException(nameof(config.Connections));
            InitializeLdapConnections();
        }

        private void InitializeLdapConnections()
        {
            foreach (var ldapConfig in _config)
            {
                var connection = new LdapConnection { SecureSocketLayer = ldapConfig.Ssl };
                connection.UserDefinedServerCertValidationDelegate += CertificateValidationCallBack;
                _ldapConnections.Add(ldapConfig.FriendlyName, connection);
            }
        }

        /// <summary>
        /// https://learn.microsoft.com/en-us/previous-versions/office/developer/exchange-server-2010/dd633677(v=exchg.80)?redirectedfrom=MSDN
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns></returns>
        private bool CertificateValidationCallBack(
            object sender,
            System.Security.Cryptography.X509Certificates.X509Certificate certificate,
            System.Security.Cryptography.X509Certificates.X509Chain chain,
            System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            // If the certificate is a valid, signed certificate, return true.
            if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
            {
                return true;
            }

            if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateNameMismatch)
            {
                _logger.LogWarning("Certificate name mismatch for certificate: {0}", certificate.Subject);
                // Optionally, implement further validation or accept the mismatch in specific cases
                return true; // Accept the mismatch for this example
            }

            // If there are errors in the certificate chain, look at each error to determine the cause.
            if ((sslPolicyErrors & System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors) != 0)
            {
                if (chain != null && chain.ChainStatus != null)
                {
                    foreach (System.Security.Cryptography.X509Certificates.X509ChainStatus status in chain.ChainStatus)
                    {
                        if ((certificate.Subject == certificate.Issuer) &&
                            (status.Status == System.Security.Cryptography.X509Certificates.X509ChainStatusFlags
                                .UntrustedRoot))
                        {
                            // Self-signed certificates with an untrusted root are valid. 
                            continue;
                        }
                        else
                        {
                            if (status.Status != System.Security.Cryptography.X509Certificates.X509ChainStatusFlags
                                    .NoError)
                            {
                                _logger.LogWarning("Certificate error: Subject:{0} | Code:{1} | Details:{2}",
                                    certificate.Subject, status.Status, status.StatusInformation);
                                // If there are any other errors in the certificate chain, the certificate is invalid,
                                // so the method returns false.
                                return false;
                            }
                        }
                    }
                }

                // When processing reaches this line, the only errors in the certificate chain are 
                // untrusted root errors for self-signed certificates. These certificates are valid
                // for default Exchange server installations, so return true.
                return true;
            }
            else
            {
                // In all other cases, return false.
                return false;
            }
        }


        public Result<TUser, Error> Login(string username, string password)
        {
            return AuthenticateUser(username, password, null);
        }

        public Result<TUser, Error> Login(string username, string password, string domain)
        {
            return AuthenticateUser(username, password, domain);
        }

        public Result<TUser, Error> FindUser(string username)
        {
            return FindUserByUsername(username, null);
        }

        public Result<TUser, Error> FindUser(string username, string domain)
        {
            return FindUserByUsername(username, domain);
        }

        public Result<TUser, Error> FindUserByEmail(string email)
        {
            return FindUserByEmail(email, null);
        }

        public Result<TUser, Error> FindUserByEmail(string email, string domain)
        {
            return GetUserByAttribute("mail", email, domain);
        }

        private Result<TUser, Error> FindUserByUsername(string username, string? domain)
        {
            return GetUserByAttribute("sAMAccountName", username, domain);
        }

        private Result<TUser, Error> AuthenticateUser(string username, string password, string domain)
        {
            var searchResult = PerformLdapSearch("sAMAccountName", username, domain);
            if (searchResult.IsFailure) return searchResult.Error;
            var userEntry = GetUserFromSearchResults(searchResult.Value);
            if (userEntry.IsFailure) return userEntry.Error;

            if (userEntry.Value == null)
            {
                return new Error("404", "User not found in any LDAP.");
            }

            searchResult.Value.LdapConnection.Bind(userEntry.Value.Dn, password);
            if (!searchResult.Value.LdapConnection.Bound)
            {
                return new Error("404", "User not found in any LDAP.");
            }

            var managerEntry = GetManagerEntry(userEntry.Value, searchResult.Value.LdapConnection);
            var user = CreateUser(userEntry.Value, managerEntry, domain);
            searchResult.Value.LdapConnection.Disconnect();
            return user;
        }

        private Result<TUser, Error> GetUserByAttribute(string attributeName, string attributeValue, string? domain)
        {
            var searchResult = PerformLdapSearch(attributeName, attributeValue, domain);
            if (searchResult.IsFailure) return searchResult.Error;

            var userEntry = GetUserFromSearchResults(searchResult.Value);
            if (userEntry.IsFailure) return userEntry.Error;

            if (userEntry.Value == null)
            {
                return new Error("404", "User not found in any LDAP.");
            }

            var managerEntry = GetManagerEntry(userEntry.Value, searchResult.Value.LdapConnection);
            return CreateUser(userEntry.Value, managerEntry, domain ?? "local");
        }

        public Result<List<string>, Error> GetUserAttributes(string attributeName, string attributeValue,
            string? domain)
        {
            var searchResult = PerformLdapSearch(attributeName, attributeValue, domain, true);
            if (searchResult.IsFailure) return searchResult.Error;
            var userAttributes = new List<string>();

            while (searchResult.Value.Results.HasMore())
            {
                try
                {
                    var ldapEntry = searchResult.Value.Results.Next();
                    _logger.LogInformation("DN: {Dn}", ldapEntry.Dn);
                    foreach (LdapAttribute attribute in ldapEntry.GetAttributeSet())
                    {
                        userAttributes.Add($"{attribute.Name} = {attribute.StringValue}");
                    }

                    return userAttributes;
                }
                catch (Exception e)
                {
                    var ex = e.Demystify();
                    _logger.LogError(ex, ex.Message);
                }
            }

            return userAttributes;
        }


        private TUser CreateUser(LdapEntry user, LdapEntry? manager = null, string domain = "")
        {
            var newUser = new TUser();
            newUser.SetBaseDetails(user, manager, domain);
            return newUser;
        }

        private Result<(ILdapSearchResults Results, LdapConnection LdapConnection), Error> PerformLdapSearch(
            string attributeName, string attributeValue, string? domain, bool allAttributes = false)
        {
            try
            {
                var concernedConfigs = _config.Where(f => f.IsConcerned(attributeValue)).ToList();

                if (!string.IsNullOrEmpty(domain))
                {
                    concernedConfigs = concernedConfigs.Where(e => e.FriendlyName.Equals(domain)).ToList();
                }

                if (!concernedConfigs.Any())
                {
                    return new Error("404", "No searchable LDAP");
                }

                foreach (var ldapConfig in concernedConfigs)
                {
                    var ldapConnection = _ldapConnections[ldapConfig.FriendlyName];
                    ldapConnection.Connect(ldapConfig.Url, ldapConfig.FinalLdapConnectionPort);
                    ldapConnection.Bind(ldapConfig.BindDn, ldapConfig.BindCredentials);

                    var ldapAttributes = new TUser().LdapAttributes;
                    var searchFilter = ldapConfig.UserSearchFilter;
                    var filter = string.Format(searchFilter, attributeName, attributeValue);
                    var ldapSearchResults = ldapConnection.Search(ldapConfig.SearchBase, LdapConnection.ScopeSub,
                        filter,
                        allAttributes ? null : ldapAttributes, false);

                    if (ldapSearchResults.HasMore())
                    {
                        return (ldapSearchResults, ldapConnection);
                    }
                }

                return new Error("404", "User not found in any LDAP.");
            }
            catch (Exception e)
            {
                var ex = e.Demystify();
                return new Error("500", ex.Message, ex);
            }
        }

        private Result<LdapEntry?, Error> GetUserFromSearchResults(
            (ILdapSearchResults Results, LdapConnection LdapConnection) searchResult)
        {
            try
            {
                return searchResult.Results.HasMore() ? searchResult.Results.Next() : null;
            }
            catch (Exception ex)
            {
                return LogAndThrowException(ex);
            }
        }

        private LdapEntry? GetManagerEntry(LdapEntry userEntry, LdapConnection ldapConnection)
        {
            try
            {
                var managerDn = userEntry.GetAttribute("manager")?.StringValue;
                if (managerDn == null)
                {
                    return null;
                }

                var _ldapConfig = _config.First();


                var managerSearchFilter = string.Format(_ldapConfig.ManagerSearchFilter, managerDn);
                //$"(&(objectClass=user)(distinguishedName={managerDn}))";
                var managerSearchResults = ldapConnection.Search(managerDn, LdapConnection.ScopeBase,
                    managerSearchFilter, null, false);

                return managerSearchResults.HasMore() ? managerSearchResults.Next() : null;
            }
            catch (Exception e)
            {
                var ex = e.Demystify();
                _logger.LogError(ex, "Error retrieving manager details.");
                return null;
            }
        }

        void PrintAttributes(LdapEntry? entry)
        {
            if (entry == null)
            {
                _logger.LogError("Manager entry is null");
                return;
            }

            LdapAttributeSet attributeSet = entry.GetAttributeSet();
            foreach (LdapAttribute attribute in attributeSet)
            {
                string attributeName = attribute.Name;
                string attributeVal = attribute.StringValue;
                if (!Base64.IsLdifSafe(attributeVal))
                {
                    byte[] tbyte = System.Text.Encoding.UTF8.GetBytes(attributeVal);
                    attributeVal = Base64.Encode(tbyte);
                }

                _logger.LogError(attributeName + " value: " + attributeVal);
            }
        }

        private Error LogAndThrowException(Exception e)
        {
            var ex = e.Demystify();
            _logger.LogError(ex, ex.Message);
            return new Error("500", ex.Message, ex);
        }
    }
}