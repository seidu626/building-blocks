using System.Security.Claims;
using System.Text.Json;
using BuildingBlocks.Common;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Ldap.Models;
using BuildingBlocks.Ldap.UserModel;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Ldap.Services;

public class MockLdapService<TUser> : ILdapService<TUser> where TUser : IAppUser, new()
{
    private readonly ILogger<MockLdapService<TUser>> _logger;
    private readonly IHostEnvironment _environment;
    private readonly LdapConfig _config;
    private readonly List<MockLdapUser> _users = new();
    private readonly Dictionary<string, MockLdapUser> _usersByUsername = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, MockLdapUser> _usersByEmail = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, MockLdapUser> _usersByPhone = new(StringComparer.OrdinalIgnoreCase);

    public MockLdapService(LdapConfig config, ILogger<MockLdapService<TUser>> logger, IHostEnvironment environment)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));

        LoadMockUsers();
        
        if (!_environment.IsDevelopment())
        {
            _logger.LogWarning("Mock LDAP service is enabled in a non-development environment. This should only be used for testing.");
        }
        else
        {
            _logger.LogInformation("Mock LDAP service initialized with {UserCount} users", _users.Count);
        }
    }

    private void LoadMockUsers()
    {
        try
        {
            var filePath = _config.MockUsersFilePath ?? "mock-ldap-users.json";
            
            // Try to find the file in various locations
            var possiblePaths = new[]
            {
                filePath,
                Path.Combine(AppContext.BaseDirectory, filePath),
                Path.Combine(Directory.GetCurrentDirectory(), filePath),
                Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "src", "Skoruba.Duende.IdentityServer.STS.Identity", filePath)
            };

            string? actualPath = null;
            foreach (var path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    actualPath = path;
                    break;
                }
            }

            if (actualPath == null)
            {
                _logger.LogWarning("Mock LDAP users file not found. Searched paths: {Paths}. Using empty user list.", string.Join(", ", possiblePaths));
                return;
            }

            _logger.LogInformation("Loading mock LDAP users from: {FilePath}", actualPath);
            
            var jsonContent = File.ReadAllText(actualPath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };
            
            var data = JsonSerializer.Deserialize<MockLdapUsersData>(jsonContent, options);
            
            if (data?.Users == null || data.Users.Count == 0)
            {
                _logger.LogWarning("No users found in mock LDAP users file: {FilePath}", actualPath);
                return;
            }

            _users.AddRange(data.Users);
            
            // Build lookup dictionaries
            foreach (var user in _users)
            {
                if (!string.IsNullOrEmpty(user.Username))
                {
                    _usersByUsername[user.Username] = user;
                }
                
                if (!string.IsNullOrEmpty(user.Email))
                {
                    _usersByEmail[user.Email] = user;
                }
                
                if (!string.IsNullOrEmpty(user.Phone))
                {
                    _usersByPhone[user.Phone] = user;
                    // Also add formatted phone numbers
                    var formattedPhones = GetFormattedPhoneNumbers(user.Phone);
                    foreach (var formattedPhone in formattedPhones)
                    {
                        if (!string.IsNullOrEmpty(formattedPhone))
                        {
                            _usersByPhone[formattedPhone] = user;
                        }
                    }
                }
            }
            
            _logger.LogInformation("Successfully loaded {UserCount} mock LDAP users", _users.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading mock LDAP users file: {Error}", ex.Message);
        }
    }

    public async Task<Result<TUser, Error>> Login(string? username, string password, IdentifierType identifierType)
    {
        return await Login(username, password, identifierType, null);
    }

    public async Task<Result<TUser, Error>> Login(string? username, string password, IdentifierType identifierType, string? domain)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            return new Error("400", "Username and password are required.");
        }

        _logger.LogInformation("Mock LDAP login attempt for user: {Username}, identifier type: {IdentifierType}", username, identifierType);

        var user = FindUserByIdentifier(username, identifierType);
        
        if (user == null)
        {
            _logger.LogWarning("Mock LDAP user not found: {Username}", username);
            return new Error("404", "User not found in mock LDAP.");
        }

        // Validate password
        var passwordValid = ValidatePassword(password, user.Password);
        if (!passwordValid)
        {
            _logger.LogWarning("Mock LDAP authentication failed for user: {Username} - invalid password", username);
            return new Error("401", "Invalid credentials.");
        }

        var appUser = ConvertToAppUser(user, domain);
        return Result.Success<TUser, Error>(appUser);
    }

    public async Task<Result<TUser, Error>> FindUser(string? username)
    {
        return await FindUser(username, null);
    }

    public async Task<Result<TUser, Error>> FindUser(string? username, string domain)
    {
        if (string.IsNullOrEmpty(username))
        {
            return new Error("400", "Username is required.");
        }

        var user = _usersByUsername.TryGetValue(username, out var foundUser) ? foundUser : null;
        
        if (user == null)
        {
            return new Error("404", "User not found in mock LDAP.");
        }

        var appUser = ConvertToAppUser(user, domain);
        return Result.Success<TUser, Error>(appUser);
    }

    public async Task<Result<TUser, Error>> FindUserByEmail(string? email)
    {
        return await FindUserByEmail(email, null);
    }

    public async Task<Result<TUser, Error>> FindUserByEmail(string? email, string? domain)
    {
        if (string.IsNullOrEmpty(email))
        {
            return new Error("400", "Email is required.");
        }

        var user = _usersByEmail.TryGetValue(email, out var foundUser) ? foundUser : null;
        
        if (user == null)
        {
            return new Error("404", "User not found in mock LDAP.");
        }

        var appUser = ConvertToAppUser(user, domain);
        return Result.Success<TUser, Error>(appUser);
    }

    public async Task<Result<TUser, Error>> FindUserByPhone(string? phone)
    {
        return await FindUserByPhone(phone, null);
    }

    public async Task<Result<TUser, Error>> FindUserByPhone(string? phone, string? domain)
    {
        if (string.IsNullOrEmpty(phone))
        {
            return new Error("400", "Phone number is required.");
        }

        var formattedPhones = GetFormattedPhoneNumbers(phone);
        MockLdapUser? user = null;
        
        foreach (var formattedPhone in formattedPhones)
        {
            if (!string.IsNullOrEmpty(formattedPhone) && _usersByPhone.TryGetValue(formattedPhone, out var foundUser))
            {
                user = foundUser;
                break;
            }
        }

        if (user == null)
        {
            return new Error("404", "User not found with the given phone number.");
        }

        var appUser = ConvertToAppUser(user, domain);
        return Result.Success<TUser, Error>(appUser);
    }

    public async Task<Result<List<string>, Error>> GetUserAttributes(string attributeName, string? attributeValue, string domain)
    {
        if (string.IsNullOrEmpty(attributeValue))
        {
            return new Error("400", "Attribute value is required.");
        }

        var user = FindUserByAttribute(attributeName, attributeValue);
        
        if (user == null)
        {
            return new Error("404", "User not found in mock LDAP.");
        }

        var attributes = new List<string>();
        
        // Add all user attributes
        if (!string.IsNullOrEmpty(user.Username))
            attributes.Add($"sAMAccountName = {user.Username}");
        if (!string.IsNullOrEmpty(user.Email))
            attributes.Add($"mail = {user.Email}");
        if (!string.IsNullOrEmpty(user.Phone))
            attributes.Add($"mobile = {user.Phone}");
        if (!string.IsNullOrEmpty(user.DisplayName))
            attributes.Add($"displayName = {user.DisplayName}");
        if (!string.IsNullOrEmpty(user.FullName))
            attributes.Add($"cn = {user.FullName}");
        if (!string.IsNullOrEmpty(user.Department))
            attributes.Add($"department = {user.Department}");
        if (!string.IsNullOrEmpty(user.Division))
            attributes.Add($"division = {user.Division}");
        if (!string.IsNullOrEmpty(user.Title))
            attributes.Add($"title = {user.Title}");
        if (!string.IsNullOrEmpty(user.EmployeeNumber))
            attributes.Add($"employeeID = {user.EmployeeNumber}");
        if (user.MemberOf != null && user.MemberOf.Count > 0)
        {
            foreach (var group in user.MemberOf)
            {
                attributes.Add($"memberOf = {group}");
            }
        }

        return Result.Success<List<string>, Error>(attributes);
    }

    private MockLdapUser? FindUserByIdentifier(string identifier, IdentifierType identifierType)
    {
        return identifierType switch
        {
            IdentifierType.Username => _usersByUsername.TryGetValue(identifier, out var user) ? user : null,
            IdentifierType.Email => _usersByEmail.TryGetValue(identifier, out var user) ? user : null,
            IdentifierType.Phone => FindUserByPhoneNumber(identifier),
            _ => null
        };
    }

    private MockLdapUser? FindUserByPhoneNumber(string phone)
    {
        var formattedPhones = GetFormattedPhoneNumbers(phone);
        foreach (var formattedPhone in formattedPhones)
        {
            if (!string.IsNullOrEmpty(formattedPhone) && _usersByPhone.TryGetValue(formattedPhone, out var user))
            {
                return user;
            }
        }
        return null;
    }

    private MockLdapUser? FindUserByAttribute(string attributeName, string attributeValue)
    {
        return attributeName.ToLowerInvariant() switch
        {
            "samaccountname" or "username" => _usersByUsername.TryGetValue(attributeValue, out var user) ? user : null,
            "mail" or "email" => _usersByEmail.TryGetValue(attributeValue, out var user) ? user : null,
            "mobile" or "phone" => FindUserByPhoneNumber(attributeValue),
            _ => null
        };
    }

    private bool ValidatePassword(string providedPassword, string? storedPassword)
    {
        if (string.IsNullOrEmpty(storedPassword))
        {
            return false;
        }

        // For mock LDAP, we support plain text password comparison
        // In a real scenario, you might want to support hashed passwords
        return _config.MockPasswordValidation == MockPasswordValidation.PlainText
            ? providedPassword == storedPassword
            : providedPassword == storedPassword; // Default to plain text for simplicity
    }

    private TUser ConvertToAppUser(MockLdapUser mockUser, string? domain)
    {
        var appUser = new TUser();
        
        // Set basic properties
        if (appUser is ActiveDirectoryAppUser adUser)
        {
            adUser.Username = mockUser.Username;
            adUser.Email = mockUser.Email;
            adUser.Mobile = mockUser.Phone ?? string.Empty;
            adUser.DisplayName = mockUser.DisplayName ?? string.Empty;
            adUser.Fullname = mockUser.FullName ?? string.Empty;
            adUser.EmployeeNumber = mockUser.EmployeeNumber ?? string.Empty;
            adUser.EmployeeType = mockUser.EmployeeType ?? string.Empty;
            adUser.Department = mockUser.Department ?? string.Empty;
            adUser.Division = mockUser.Division ?? string.Empty;
            adUser.Title = mockUser.Title ?? string.Empty;
            adUser.Description = mockUser.Description ?? string.Empty;
            adUser.LockoutTime = mockUser.LockoutTime;
            adUser.PwdLastSet = mockUser.PwdLastSet;
            adUser.BadPwdCount = mockUser.BadPwdCount;
            adUser.AccountExpires = mockUser.AccountExpires;
            adUser.ProviderName = domain ?? "mock-ldap";
            adUser.ProviderSubjectId = mockUser.Username;
            adUser.SubjectId = mockUser.Username;

            // Build claims
            var claims = new List<Claim>();
            
            if (!string.IsNullOrEmpty(adUser.DisplayName))
                claims.Add(new Claim("name", adUser.DisplayName));
            if (!string.IsNullOrEmpty(mockUser.LastName))
                claims.Add(new Claim("family_name", mockUser.LastName));
            if (!string.IsNullOrEmpty(mockUser.FirstName))
                claims.Add(new Claim("given_name", mockUser.FirstName));
            if (!string.IsNullOrEmpty(adUser.Email))
                claims.Add(new Claim("email", adUser.Email));
            if (!string.IsNullOrEmpty(adUser.Mobile))
                claims.Add(new Claim("phone_number", adUser.Mobile));
            if (!string.IsNullOrEmpty(adUser.Fullname))
                claims.Add(new Claim("fullName", adUser.Fullname));
            if (adUser.BadPwdCount.HasValue)
                claims.Add(new Claim("badPwdCount", adUser.BadPwdCount.Value.ToString()));
            if (adUser.LockoutTime.HasValue)
                claims.Add(new Claim("lockoutTime", adUser.LockoutTime.Value.ToString()));
            if (adUser.PwdLastSet.HasValue)
                claims.Add(new Claim("pwdLastSet", adUser.PwdLastSet.Value.ToString()));
            if (!string.IsNullOrEmpty(adUser.Title))
                claims.Add(new Claim("title", adUser.Title));
            if (!string.IsNullOrEmpty(adUser.Description))
                claims.Add(new Claim("description", adUser.Description));
            if (!string.IsNullOrEmpty(adUser.Division))
                claims.Add(new Claim("division", adUser.Division));
            if (!string.IsNullOrEmpty(adUser.Department))
                claims.Add(new Claim("department", adUser.Department));
            if (!string.IsNullOrEmpty(adUser.EmployeeNumber))
                claims.Add(new Claim("employeeNumber", adUser.EmployeeNumber));
            if (!string.IsNullOrEmpty(adUser.EmployeeType))
                claims.Add(new Claim("employeeType", adUser.EmployeeType));

            // Add manager claim if available
            if (mockUser.Manager != null)
            {
                var managerDetails = new Dictionary<string, string>
                {
                    { "DisplayName", mockUser.Manager.DisplayName ?? string.Empty },
                    { "Fullname", mockUser.Manager.FullName ?? string.Empty },
                    { "Email", mockUser.Manager.Email ?? string.Empty },
                    { "Title", mockUser.Manager.Title ?? string.Empty },
                    { "Department", mockUser.Manager.Department ?? string.Empty },
                    { "Division", mockUser.Manager.Division ?? string.Empty },
                    { "Mobile", mockUser.Manager.Mobile ?? string.Empty }
                };
                var managerDetailsJson = JsonSerializer.Serialize(managerDetails);
                claims.Add(new Claim("manager", managerDetailsJson));
            }

            // Add role claims from memberOf
            if (mockUser.MemberOf != null && mockUser.MemberOf.Count > 0)
            {
                foreach (var group in mockUser.MemberOf)
                {
                    claims.Add(new Claim("role", group));
                }
            }

            adUser.Claims = claims;
        }
        else
        {
            // Fallback for other user types
            appUser.Username = mockUser.Username;
            appUser.Email = mockUser.Email;
            appUser.ProviderName = domain ?? "mock-ldap";
            appUser.ProviderSubjectId = mockUser.Username;
            appUser.SubjectId = mockUser.Username;
        }

        return appUser;
    }

    private IEnumerable<string?> GetFormattedPhoneNumbers(string? phone)
    {
        if (string.IsNullOrEmpty(phone))
            yield break;

        yield return phone;

        // Add the phone number with international code (e.g., +233)
        if (phone.StartsWith("0", StringComparison.Ordinal) && phone.Length > 1)
        {
            yield return "233" + phone.Substring(1);
        }

        // Add the local format if the phone number starts with the country code
        if (phone.StartsWith("233", StringComparison.Ordinal) && phone.Length > 3)
        {
            yield return "0" + phone.Substring(3);
        }

        // Add with + prefix
        if (!phone.StartsWith("+", StringComparison.Ordinal))
        {
            yield return "+" + phone;
        }
        else
        {
            yield return phone.Substring(1);
        }
    }
}

