using System.Text.Json.Serialization;

namespace BuildingBlocks.Ldap.Models;

public class MockLdapUser
{
    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    [JsonPropertyName("password")]
    public string? Password { get; set; }

    [JsonPropertyName("displayName")]
    public string? DisplayName { get; set; }

    [JsonPropertyName("fullName")]
    public string? FullName { get; set; }

    [JsonPropertyName("firstName")]
    public string? FirstName { get; set; }

    [JsonPropertyName("lastName")]
    public string? LastName { get; set; }

    [JsonPropertyName("employeeNumber")]
    public string? EmployeeNumber { get; set; }

    [JsonPropertyName("employeeType")]
    public string? EmployeeType { get; set; }

    [JsonPropertyName("department")]
    public string? Department { get; set; }

    [JsonPropertyName("division")]
    public string? Division { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("manager")]
    public MockLdapManager? Manager { get; set; }

    [JsonPropertyName("memberOf")]
    public List<string>? MemberOf { get; set; }

    [JsonPropertyName("lockoutTime")]
    public long? LockoutTime { get; set; }

    [JsonPropertyName("pwdLastSet")]
    public long? PwdLastSet { get; set; }

    [JsonPropertyName("badPwdCount")]
    public long? BadPwdCount { get; set; }

    [JsonPropertyName("accountExpires")]
    public long? AccountExpires { get; set; }

    [JsonPropertyName("createdOn")]
    public string? CreatedOn { get; set; }

    [JsonPropertyName("updatedOn")]
    public string? UpdatedOn { get; set; }
}

public class MockLdapManager
{
    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonPropertyName("displayName")]
    public string? DisplayName { get; set; }

    [JsonPropertyName("fullName")]
    public string? FullName { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("department")]
    public string? Department { get; set; }

    [JsonPropertyName("division")]
    public string? Division { get; set; }

    [JsonPropertyName("mobile")]
    public string? Mobile { get; set; }
}

public class MockLdapUsersData
{
    [JsonPropertyName("users")]
    public List<MockLdapUser> Users { get; set; } = new();
}

