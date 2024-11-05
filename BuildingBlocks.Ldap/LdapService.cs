using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Security;
using BuildingBlocks.Common.AOP;
using BuildingBlocks.Exceptions;
using CSharpFunctionalExtensions;
using MethodTimer;
using Microsoft.Extensions.Logging;
using Novell.Directory.Ldap;
using Novell.Directory.Ldap.Utilclass;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace BuildingBlocks.Ldap
{
    public class LdapService<TUser> : ILdapService<TUser> where TUser : IAppUser, new()
    {
        private readonly ILogger<LdapService<TUser>> _logger;
        private readonly ICollection<LdapConfig> _config;
        private readonly ConcurrentDictionary<string, ConcurrentBag<LdapConnection>> _connectionPools = new();
        private readonly int _maxPoolSize; // Maximum connections in the pool
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;

        public LdapService(ExtensionConfig config, ILogger<LdapService<TUser>> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config.Connections ?? throw new ArgumentNullException(nameof(config.Connections));
            _maxPoolSize = _config.First().MaxPoolSize;
            InitializeConnectionPools();

            // Define the retry policy
            _retryPolicy = Policy
                .Handle<LdapException>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromMilliseconds(_config.First().Timeout * retryAttempt),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning($"Retry {retryCount} for LDAP operation due to: {exception.Message}");
                    });

            // Define the circuit breaker policy
            _circuitBreakerPolicy = Policy
                .Handle<LdapException>()
                .CircuitBreakerAsync(2, TimeSpan.FromMinutes(2),
                    (exception, timespan) => { _logger.LogWarning("Circuit breaker triggered for LDAP operation."); },
                    () => { _logger.LogInformation("Circuit breaker reset for LDAP."); });
        }

        #region Helpers

        private void InitializeConnectionPools()
        {
            foreach (var ldapConfig in _config)
            {
                _connectionPools[ldapConfig.FriendlyName] = new ConcurrentBag<LdapConnection>();
            }
        }

        private LdapConnection GetConnectionFromPool(LdapConfig ldapConfig)
        {
            if (_connectionPools.TryGetValue(ldapConfig.FriendlyName, out var pool))
            {
                if (pool.TryTake(out var connection) && IsConnectionValid(connection))
                {
                    _logger.LogInformation("Reusing existing LDAP connection.");
                    return connection;
                }
            }

            _logger.LogInformation("Creating new LDAP connection.");
            return CreateLdapConnection(ldapConfig);
        }

        private void ReturnConnectionToPool(LdapConfig ldapConfig, LdapConnection connection)
        {
            if (_connectionPools.TryGetValue(ldapConfig.FriendlyName, out var pool) &&
                pool.Count < _maxPoolSize)
            {
                _logger.LogInformation("Returning LDAP connection to the pool.");
                pool.Add(connection);
            }
            else
            {
                _logger.LogInformation("Disposing unused LDAP connection.");
                connection.Disconnect();
            }
        }

        private bool IsConnectionValid(LdapConnection connection)
        {
            try
            {
                return connection.Connected;
            }
            catch
            {
                return false;
            }
        }

        private LdapConnection CreateLdapConnection(LdapConfig ldapConfig)
        {
            var connection = new LdapConnection { SecureSocketLayer = ldapConfig.Ssl };
            connection.UserDefinedServerCertValidationDelegate += CertificateValidationCallBack;
            connection.Connect(ldapConfig.Url, ldapConfig.FinalLdapConnectionPort);
            return connection;
        }


        /// <summary>
        /// https://learn.microsoft.com/en-us/previous-versions/office/developer/exchange-server-2010/dd633677(v=exchg.80)?redirectedfrom=MSDN
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns></returns>
        private bool CertificateValidationCallBack(object sender,
            System.Security.Cryptography.X509Certificates.X509Certificate certificate,
            System.Security.Cryptography.X509Certificates.X509Chain chain,
            System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true;
            }

            _logger.LogWarning("Certificate validation issue for {0}: {1}", certificate.Subject, sslPolicyErrors);

            if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateNameMismatch)
            {
                _logger.LogWarning("Certificate name mismatch: {0}", certificate.Subject);
                return true;
            }

            return sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors && IsChainValid(chain);
        }


        private bool IsChainValid(System.Security.Cryptography.X509Certificates.X509Chain chain)
        {
            if (chain == null || chain.ChainStatus == null)
                return false;

            foreach (var status in chain.ChainStatus)
            {
                if (status.Status == System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.UntrustedRoot)
                {
                    _logger.LogWarning("Untrusted root detected");
                    continue;
                }

                if (status.Status != System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
                {
                    _logger.LogError("Chain status error: {0}", status.StatusInformation);
                    return false;
                }
            }

            return true;
        }

        #endregion


        public async Task<Result<TUser, Error>> Login(string username, string password)
        {
            return await AuthenticateUser(username, password, null);
        }

        public async Task<Result<TUser, Error>> Login(string username, string password, string? domain)
        {
            return await AuthenticateUser(username, password, domain);
        }

        public Task<Result<TUser, Error>> FindUser(string username)
        {
            return Task.FromResult(FindUserByUsername(username, null));
        }

        public Task<Result<TUser, Error>> FindUser(string username, string domain)
        {
            return Task.FromResult(FindUserByUsername(username, domain));
        }

        public Task<Result<TUser, Error>> FindUserByEmail(string email)
        {
            return FindUserByEmail(email, null);
        }

        public Task<Result<TUser, Error>> FindUserByPhone(string phone)
        {
            return FindUserByPhone(phone, null);
        }

        public Task<Result<TUser, Error>> FindUserByEmail(string email, string? domain)
        {
            return Task.FromResult(GetUserByAttribute("mail", email, domain));
        }

        public Task<Result<TUser, Error>> FindUserByPhone(string phone, string? domain)
        {
            var formattedPhones = GetFormattedPhoneNumbers(phone);
            foreach (var formattedPhone in formattedPhones)
            {
                var result = GetUserByAttribute("mobile", formattedPhone, domain);
                if (result.IsSuccess)
                {
                    return Task.FromResult(result); // Return the user if found
                }
            }

            return Task.FromResult<Result<TUser, Error>>(
                new Error("404", "User not found with the given phone number."));
        }

        private Result<TUser, Error> FindUserByUsername(string username, string? domain)
        {
            return GetUserByAttribute("sAMAccountName", username, domain);
        }

        private async Task<Result<TUser, Error>> AuthenticateUser(string username, string password, string? domain)
        {
            return await _circuitBreakerPolicy.ExecuteAsync(async () =>
            {
                return await _retryPolicy.ExecuteAsync(() => PerformLdapAuthentication(username, password, domain));
            });
        }

        [Time]
        [Timeout(1200)]
        private LdapEntry? GetManagerEntry(LdapEntry userEntry, LdapConnection ldapConnection)
        {
            try
            {
                var managerDn = userEntry.GetAttribute("manager")?.StringValue;
                if (managerDn == null)
                {
                    return null;
                }

                var ldapConfig = _config.First();
                var managerSearchFilter = string.Format(ldapConfig.ManagerSearchFilter, managerDn);
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

        #region Utils

        private IEnumerable<string> GetFormattedPhoneNumbers(string phone)
        {
            var formattedPhones = new List<string>();

            // Add the original phone number
            formattedPhones.Add(phone);

            // Add the phone number with international code (e.g., +233)
            if (phone.StartsWith("0"))
            {
                formattedPhones.Add("233" + phone.Substring(1)); // Example: 0540618592 -> 233540618592
            }

            // Add the local format if the phone number starts with the country code
            if (phone.StartsWith("233"))
            {
                formattedPhones.Add("0" + phone.Substring(3)); // Example: 233540618592 -> 0540618592
            }

            return formattedPhones;
        }

        [Time]
        [Timeout(1200)]
        private Result<TUser, Error> GetUserByAttribute(string attributeName, string attributeValue, string? domain)
        {
            var ldapConfig = _config.First();
            var connection = GetConnectionFromPool(ldapConfig);
            try
            {
                connection.Bind(ldapConfig.BindDn, ldapConfig.BindCredentials);
                var searchResult = PerformLdapSearch(attributeName, attributeValue, ldapConfig, connection);
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
            catch (LdapException e)
            {
                var ex = e.Demystify();
                _logger.LogError(ex, ex.LdapErrorMessage);
                return new Error("500", ex.LdapErrorMessage, ex);
            }
            catch (Exception e)
            {
                var ex = e.Demystify();
                _logger.LogError(ex, ex.Message);
                return new Error("500", ex.Message, ex);
            }
            finally
            {
                ReturnConnectionToPool(ldapConfig, connection);
            }
        }

        /// <summary>
        /// https://github.com/dsbenghe/Novell.Directory.Ldap.NETStandard/blob/master/original_samples/Samples/Search.cs
        /// </summary>
        /// <param name="attributeName"></param>
        /// <param name="attributeValue"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        [Time]
        [Timeout(1200)]
        public Task<Result<List<string>, Error>> GetUserAttributes(string attributeName, string attributeValue,
            string domain)
        {
            var ldapConfig = _config.First();
            var connection = GetConnectionFromPool(ldapConfig);

            try
            {
                connection.Bind(ldapConfig.BindDn, ldapConfig.BindCredentials);
                var searchResult = PerformLdapSearch(attributeName, attributeValue, ldapConfig, connection);
                if (searchResult.IsFailure) return Task.FromResult<Result<List<string>, Error>>(searchResult.Error);
                var userAttributes = new List<string>();

                while (searchResult.Value.Results.HasMore())
                {
                    LdapEntry? ldapEntry = null;
                    try
                    {
                        ldapEntry = searchResult.Value.Results.Next();
                        _logger.LogInformation("DN: {Dn}", ldapEntry.Dn);
                    }
                    catch (LdapException e)
                    {
                        var ex = e.Demystify();
                        _logger.LogError(ex, ex.LdapErrorMessage);
                        // Exception is thrown, go for next entry
                        continue;
                    }

                    foreach (LdapAttribute attribute in ldapEntry.GetAttributeSet())
                    {
                        userAttributes.Add($"{attribute.Name} = {attribute.StringValue}");
                    }

                    return Task.FromResult<Result<List<string>, Error>>(userAttributes);
                }

                return Task.FromResult<Result<List<string>, Error>>(userAttributes);
            }
            catch (LdapException e)
            {
                var ex = e.Demystify();
                _logger.LogError(ex, ex.LdapErrorMessage);
                return Task.FromResult<Result<List<string>, Error>>(new Error("500", ex.LdapErrorMessage, ex));
            }
            catch (Exception e)
            {
                var ex = e.Demystify();
                _logger.LogError(ex, ex.Message);
                return Task.FromResult<Result<List<string>, Error>>(new Error("500", ex.Message, ex));
            }
            finally
            {
                ReturnConnectionToPool(ldapConfig, connection);
            }
        }

        private TUser CreateUser(LdapEntry user, LdapEntry? manager = null, string? domain = "")
        {
            var newUser = new TUser();
            newUser.SetBaseDetails(user, manager, domain);
            return newUser;
        }

        private Task<Result<TUser, Error>> PerformLdapAuthentication(string username, string password,
            string? domain)
        {
            var ldapConfig = _config.First(); // Simplified: Select the first LDAP config
            var connection = GetConnectionFromPool(ldapConfig);

            try
            {
                connection.Bind(ldapConfig.BindDn, ldapConfig.BindCredentials);
                // Perform authentication logic
                var searchResult = PerformLdapSearch("sAMAccountName", username, ldapConfig, connection);
                if (searchResult.IsFailure) return Task.FromResult<Result<TUser, Error>>(searchResult.Error);

                var userEntry = GetUserFromSearchResults(searchResult.Value);
                if (userEntry.IsFailure) return Task.FromResult<Result<TUser, Error>>(userEntry.Error);
                if (userEntry.Value == default)
                {
                    return Task.FromResult<Result<TUser, Error>>(new Error("500", "UserEntry is null"));
                }

                searchResult.Value.LdapConnection.Bind(userEntry.Value.Dn, password);
                if (!searchResult.Value.LdapConnection.Bound)
                {
                    return Task.FromResult<Result<TUser, Error>>(new Error("404", "User not found in any LDAP."));
                }

                var managerEntry = GetManagerEntry(userEntry.Value, searchResult.Value.LdapConnection);
                var user = CreateUser(userEntry.Value, managerEntry, domain);
                return Task.FromResult<Result<TUser, Error>>(user);
            }
            catch (LdapException e)
            {
                var ex = e.Demystify();
                _logger.LogError(ex, "LDAP connection or authentication error: {Error}", ex.LdapErrorMessage);
                return Task.FromResult<Result<TUser, Error>>(new Error("500", ex.LdapErrorMessage, ex));
            }
            finally
            {
                ReturnConnectionToPool(ldapConfig, connection);
            }
        }

        [Time]
        [Timeout(1200)]
        private Result<(ILdapSearchResults Results, LdapConnection LdapConnection), Error> PerformLdapSearch(
            string attributeName, string attributeValue, LdapConfig ldapConfig,
            LdapConnection connection, bool allAttributes = false)
        {
            try
            {
                var ldapAttributes = new TUser().LdapAttributes;
                var searchFilter = ldapConfig.UserSearchFilter;
                var filter = string.Format(searchFilter, attributeName, attributeValue);
                _logger.LogInformation("LDAP Search Filter: {Filter}", filter);

                var ldapSearchResults = connection.Search(ldapConfig.SearchBase, LdapConnection.ScopeSub,
                    filter,
                    allAttributes ? null : ldapAttributes, false);

                return ldapSearchResults.HasMore()
                    ? (ldapSearchResults, connection)
                    : new Error("404", "User not found in any LDAP.");
            }
            catch (LdapException e)
            {
                var ex = e.Demystify();
                _logger.LogError(ex, "Error while performing LDAP search.");
                return new Error("500", ex.LdapErrorMessage);
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

        #endregion

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