using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Security;
using System.Text;
using BuildingBlocks.Common;
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
        private readonly string[] _phoneAttributesToCheck = new[]
        {
            "mobile",
            "telephoneNumber",
            "phone",
            "homePhone",
            "otherMobile",
            "pager"
        };
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
            var retryDelay = TimeSpan.FromMilliseconds(Math.Clamp(_config.First().Timeout, 250, 1000));
            _retryPolicy = Policy
                .Handle<LdapException>()
                .WaitAndRetryAsync(1, _ => retryDelay,
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

        private async Task<LdapConnection> GetConnectionFromPoolAsync(LdapConfig ldapConfig)
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
            return await CreateLdapConnectionAsync(ldapConfig);
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

        private async Task<LdapConnection> CreateLdapConnectionAsync(LdapConfig ldapConfig)
        {
            var connection = new LdapConnection { SecureSocketLayer = ldapConfig.Ssl };
            connection.UserDefinedServerCertValidationDelegate += CertificateValidationCallBack;
            await connection.ConnectAsync(ldapConfig.Url, ldapConfig.FinalLdapConnectionPort);
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


        public async Task<Result<TUser, Error>> Login(string? username, string password, IdentifierType identifierType)
        {
            return await AuthenticateUser(username, password, identifierType, null);
        }

        public async Task<Result<TUser, Error>> Login(string? username, string password,
            IdentifierType identifierType, string? domain)
        {
            return await AuthenticateUser(username, password, identifierType, domain);
        }

        public async Task<Result<TUser, Error>> FindUser(string? username)
        {
            return await FindUserByUsernameAsync(username, null);
        }

        public async Task<Result<TUser, Error>> FindUser(string? username, string domain)
        {
            return await FindUserByUsernameAsync(username, domain);
        }

        public async Task<Result<TUser, Error>> FindUserByEmail(string? email)
        {
            return await FindUserByEmail(email, null);
        }

        public async Task<Result<TUser, Error>> FindUserByPhone(string? phone)
        {
            return await FindUserByPhone(phone, null);
        }

        public async Task<Result<TUser, Error>> FindUserByEmail(string? email, string? domain)
        {
            return await GetUserByAttributeAsync("mail", email, domain, true);
        }

        public async Task<Result<TUser, Error>> FindUserByPhone(string? phone, string? domain)
        {
            return await FindUserByPhoneAsync(phone, domain);
        }

        private async Task<Result<TUser, Error>> FindUserByUsernameAsync(string? username, string? domain)
        {
            return await GetUserByAttributeAsync("sAMAccountName", username, domain, true);
        }

        private async Task<Result<TUser, Error>> AuthenticateUser(string? username, string password, IdentifierType identifierType, string? domain)
        {
            return await _circuitBreakerPolicy.ExecuteAsync(async () =>
            {
                return await _retryPolicy.ExecuteAsync(() => PerformLdapAuthenticationAsync(username, password, identifierType, domain));
            });
        }

        [Time]
        [Timeout(1200)]
        private async Task<LdapEntry?> GetManagerEntry(LdapEntry userEntry, LdapConnection ldapConnection)
        {
            try
            {
                var managerAttribute = userEntry.Get("manager");
                if (managerAttribute == null)
                {
                    return null;
                }
                
                var managerDn = managerAttribute.StringValue;
                if (managerDn == null)
                {
                    return null;
                }

                var ldapConfig = _config.First();
                var managerSearchFilter = string.Format(ldapConfig.ManagerSearchFilter, (object) managerDn);
                //$"(&(objectClass=user)(distinguishedName={managerDn}))";
                var managerSearchResults = await ldapConnection.SearchAsync(managerDn, LdapConnection.ScopeBase,
                    managerSearchFilter, null, false);

                return await managerSearchResults.HasMoreAsync() ? await managerSearchResults.NextAsync() : null;
            }
            catch (Exception e)
            {
                var ex = e.Demystify();
                _logger.LogError(ex, "Error retrieving manager details.");
                return null;
            }
        }

        #region Utils

        private static string? GetPhoneAttributeValue(LdapEntry ldapEntry, string attributeName)
        {
            try
            {
                return ldapEntry.Get(attributeName)?.StringValue;
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }

        private async Task<Result<TUser, Error>> FindUserByNormalizedPhoneAsync(string normalizedPhone, string? domain)
        {
            if (string.IsNullOrWhiteSpace(normalizedPhone))
            {
                return new Error("404", "User not found with the given phone number.");
            }

            return await FindUserByPhoneAsync(normalizedPhone, domain);
        }

        private async Task<Result<TUser, Error>> FindUserByPhoneAsync(string? phone, string? domain)
        {
            var ldapConfig = _config.First();
            var connection = await GetConnectionFromPoolAsync(ldapConfig);

            try
            {
                await connection.BindAsync(ldapConfig.BindDn, ldapConfig.BindCredentials);
                var userEntryResult = await FindUserEntryByPhoneAsync(phone, ldapConfig, connection);
                if (userEntryResult.IsFailure)
                {
                    return userEntryResult.Error;
                }

                if (userEntryResult.Value == null)
                {
                    return new Error("404", "User not found with the given phone number.");
                }

                var managerEntry = await GetManagerEntry(userEntryResult.Value, connection);
                return CreateUser(userEntryResult.Value, managerEntry, domain ?? "local");
            }
            catch (LdapException e)
            {
                var ex = e.Demystify();
                _logger.LogError(ex, ex.LdapErrorMessage);
                return CreateDirectoryUnavailableError(ex);
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

        private async Task<Result<LdapEntry, Error>> FindUserEntryByPhoneAsync(string? phone, LdapConfig ldapConfig, LdapConnection connection)
        {
            var normalizedPhone = PhoneLookupHelper.NormalizePhoneToDigits(phone);
            if (string.IsNullOrWhiteSpace(normalizedPhone))
            {
                return new Error("400", "Phone number is required.");
            }

            var checkedDns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var formattedPhone in PhoneLookupHelper.GetPhoneLookupCandidates(phone))
            {
                if (string.IsNullOrWhiteSpace(formattedPhone))
                {
                    continue;
                }

                foreach (var phoneAttribute in _phoneAttributesToCheck)
                {
                    var searchResult = await PerformLdapSearch(phoneAttribute, formattedPhone, ldapConfig, connection, true);
                    if (searchResult.IsFailure)
                    {
                        if (searchResult.Error.Code != "404")
                        {
                            return searchResult.Error;
                        }

                        continue;
                    }

                    var userEntry = await GetUserFromSearchResults(searchResult.Value);
                    if (userEntry.IsFailure)
                    {
                        return userEntry.Error;
                    }

                    if (userEntry.Value != null)
                    {
                        return userEntry.Value;
                    }
                }
            }

            foreach (var phoneAttribute in _phoneAttributesToCheck)
            {
                var searchResult = await PerformLdapSearch(phoneAttribute, "*", ldapConfig, connection, true);
                if (searchResult.IsFailure)
                {
                    if (searchResult.Error.Code != "404")
                    {
                        return searchResult.Error;
                    }

                    continue;
                }

                while (await searchResult.Value.Results.HasMoreAsync())
                {
                    var ldapEntry = await searchResult.Value.Results.NextAsync();
                    if (!checkedDns.Add(ldapEntry.Dn))
                    {
                        continue;
                    }

                    if (DoesEntryMatchNormalizedPhone(ldapEntry, normalizedPhone))
                    {
                        return ldapEntry;
                    }
                }
            }

            return new Error("404", "User not found with the given phone number.");
        }

        private bool DoesEntryMatchNormalizedPhone(LdapEntry ldapEntry, string normalizedPhone)
        {
            foreach (var phoneAttribute in _phoneAttributesToCheck)
            {
                if (PhoneLookupHelper.IsNormalizedPhoneMatch(GetPhoneAttributeValue(ldapEntry, phoneAttribute), normalizedPhone))
                {
                    return true;
                }
            }

            return false;
        }

        [Time]
        [Timeout(1200)]
        private async Task<Result<TUser, Error>> GetUserByAttributeAsync(string attributeName, string? attributeValue, string? domain,
            bool includeAllAttributes = false)
        {
            var ldapConfig = _config.First();
            var connection = await GetConnectionFromPoolAsync(ldapConfig);
            try
            {
                await connection.BindAsync(ldapConfig.BindDn, ldapConfig.BindCredentials);
                var searchResult = await PerformLdapSearch(attributeName, attributeValue, ldapConfig, connection, includeAllAttributes);
                if (searchResult.IsFailure) return searchResult.Error;

                var userEntry = await GetUserFromSearchResults(searchResult.Value);
                if (userEntry.IsFailure) return userEntry.Error;

                if (userEntry.Value == null)
                {
                    return new Error("404", "User not found in any LDAP.");
                }

                var managerEntry = await GetManagerEntry(userEntry.Value, searchResult.Value.LdapConnection);
                return CreateUser(userEntry.Value, managerEntry, domain ?? "local");
            }
            catch (LdapException e)
            {
                var ex = e.Demystify();
                _logger.LogError(ex, ex.LdapErrorMessage);
                return CreateDirectoryUnavailableError(ex);
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
        public async Task<Result<List<string>, Error>> GetUserAttributes(string attributeName, string? attributeValue,
            string domain)
        {
            var ldapConfig = _config.First();
            var connection = await GetConnectionFromPoolAsync(ldapConfig);

            try
            {
                await connection.BindAsync(ldapConfig.BindDn, ldapConfig.BindCredentials);
                var searchResult = await PerformLdapSearch(attributeName, attributeValue, ldapConfig, connection);
                if (searchResult.IsFailure) return searchResult.Error;
                var userAttributes = new List<string>();

                while (await searchResult.Value.Results.HasMoreAsync())
                {
                    LdapEntry? ldapEntry = null;
                    try
                    {
                        ldapEntry = await searchResult.Value.Results.NextAsync();
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

                    return userAttributes;
                }

                return userAttributes;
            }
            catch (LdapException e)
            {
                var ex = e.Demystify();
                _logger.LogError(ex, ex.LdapErrorMessage);
                return CreateDirectoryUnavailableError(ex);
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

        private TUser CreateUser(LdapEntry user, LdapEntry? manager = null, string? domain = "")
        {
            var newUser = new TUser();
            newUser.SetBaseDetails(user, manager, domain);
            return newUser;
        }

        private async Task<Result<TUser, Error>> PerformLdapAuthenticationAsync(string? username, string password, IdentifierType identifierType,
            string? domain)
        {
            var ldapConfig = _config.First(); // Simplified: Select the first LDAP config
            var connection = await GetConnectionFromPoolAsync(ldapConfig);
            Result<LdapEntry, Error> userEntryResult;
            var attemptedUserCredentialBind = false;

            try
            {
                await connection.BindAsync(ldapConfig.BindDn, ldapConfig.BindCredentials);

                if (identifierType == IdentifierType.Phone)
                {
                    userEntryResult = await FindUserEntryByPhoneAsync(username, ldapConfig, connection);
                }
                else
                {
                    var attribute = identifierType.GetLdapAttribute();
                    var searchResult = await PerformLdapSearch(attribute, username, ldapConfig, connection);
                    if (searchResult.IsFailure) return searchResult.Error;

                    userEntryResult = await GetUserFromSearchResults(searchResult.Value);
                    if (userEntryResult.IsFailure) return userEntryResult.Error;
                    if (userEntryResult.Value == default)
                    {
                        return new Error("500", "UserEntry is null");
                    }
                }

                if (userEntryResult.IsFailure) return userEntryResult.Error;
                if (userEntryResult.Value == default)
                {
                    return new Error("404", "User not found in any LDAP.");
                }

                attemptedUserCredentialBind = true;
                await connection.BindAsync(userEntryResult.Value.Dn, password);
                if (!connection.Bound)
                {
                    return new Error("404", "User not found in any LDAP.");
                }

                var managerEntry = await GetManagerEntry(userEntryResult.Value, connection);
                var user = CreateUser(userEntryResult.Value, managerEntry, domain);
                return user;
            }
            catch (LdapException e)
            {
                var ex = e.Demystify();
                _logger.LogError(ex, "LDAP connection or authentication error: {Error}", ex.LdapErrorMessage);
                if (!attemptedUserCredentialBind)
                {
                    return CreateDirectoryUnavailableError(ex);
                }

                return new Error("500", ex.LdapErrorMessage, ex);
            }
            finally
            {
                ReturnConnectionToPool(ldapConfig, connection);
            }
        }

        [Time]
        [Timeout(1200)]
        private async Task<Result<(ILdapSearchResults Results, LdapConnection LdapConnection), Error>> PerformLdapSearch(
            string attributeName, string? attributeValue, LdapConfig ldapConfig,
            LdapConnection connection, bool allAttributes = false)
        {
            try
            {
                var ldapAttributes = new TUser().LdapAttributes;
                var searchFilter = ldapConfig.UserSearchFilter;
                var filter = string.Format(searchFilter, attributeName, attributeValue);
                _logger.LogInformation("LDAP Search Filter: {Filter}", filter);

                var ldapSearchResults = await connection.SearchAsync(ldapConfig.SearchBase, LdapConnection.ScopeSub,
                    filter,
                    allAttributes ? null : ldapAttributes, false);

                return await ldapSearchResults.HasMoreAsync()
                    ? (ldapSearchResults, connection)
                    : new Error("404", "User not found in any LDAP.");
            }
            catch (LdapException e)
            {
                var ex = e.Demystify();
                _logger.LogError(ex, "Error while performing LDAP search.");
                return CreateDirectoryUnavailableError(ex);
            }
            catch (Exception e)
            {
                var ex = e.Demystify();
                return new Error("500", ex.Message, ex);
            }
        }

        private async Task<Result<LdapEntry?, Error>> GetUserFromSearchResults(
            (ILdapSearchResults Results, LdapConnection LdapConnection) searchResult)
        {
            try
            {
                return await searchResult.Results.HasMoreAsync() ? await searchResult.Results.NextAsync() : null;
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

        private static Error CreateDirectoryUnavailableError(Exception exception)
        {
            return new Error("DIRECTORY_UNAVAILABLE", "Directory lookup failed.", exception);
        }
    }
}
