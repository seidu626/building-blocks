using System.Text;
using System.Text.Json.Serialization;

namespace BuildingBlocks.OAuth;

public record IdentityUser
{
    [JsonPropertyName("sub")]
    public string? Sub { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("given_name")]
    public string GivenName { get; init; }

    [JsonPropertyName("accountToken")]
    public string AccountToken { get; init; }

    [JsonPropertyName("employee_category")]
    public string EmployeeCategory { get; init; }

    [JsonPropertyName("preferred_username")]
    public string? PreferredUsername { get; init; }

    [JsonPropertyName("family_name")]
    public string FamilyName { get; init; }

    [JsonPropertyName("email")]
    public string Email { get; init; }

    [JsonPropertyName("role")]
    public List<string> Role { get; init; } = new List<string>();

    [JsonPropertyName("updatedOn")]
    public string UpdatedOn { get; init; }

    [JsonPropertyName("permissions")]
    public List<string> Permissions { get; init; } = new List<string>();

    [JsonPropertyName("phone_number")]
    public string PhoneNumber { get; init; }

    [JsonPropertyName("badPwdCount")]
    public string BadPasswordCount { get; init; }

    [JsonPropertyName("lockoutTime")]
    public string LockoutTime { get; init; }

    [JsonPropertyName("pwdLastSet")]
    public string PasswordLastSet { get; init; }

    [JsonPropertyName("title")]
    public string Title { get; init; }

    [JsonPropertyName("description")]
    public string Description { get; init; }

    [JsonPropertyName("division")]
    public string Division { get; init; }

    [JsonPropertyName("department")]
    public string Department { get; init; }

    [JsonPropertyName("createdOn")]
    public string CreatedOn { get; init; }

    [JsonPropertyName("last_login")]
    public string LastLogin { get; init; }

    [JsonPropertyName("AspNet.Identity.SecurityStamp")]
    public string SecurityStamp { get; init; }

    // Optional: If a more complex print of members is needed
    protected virtual bool PrintMembers(StringBuilder builder)
    {
        builder.Append($"Username = {PreferredUsername}, Sub = {Sub}, Name = {Name}, ");
        builder.Append($"GivenName = {GivenName}, AccountToken = {AccountToken}, EmployeeCategory = {EmployeeCategory}, ");
        builder.Append($"PreferredUsername = {PreferredUsername}, FamilyName = {FamilyName}, Email = {Email}, ");
        builder.Append($"Role = {string.Join(", ", Role)}, UpdatedOn = {UpdatedOn}, Permissions = {string.Join(", ", Permissions)}, ");
        builder.Append($"PhoneNumber = {PhoneNumber}, BadPasswordCount = {BadPasswordCount}, LockoutTime = {LockoutTime}, ");
        builder.Append($"PasswordLastSet = {PasswordLastSet}, Title = {Title}, Description = {Description}, ");
        builder.Append($"Division = {Division}, Department = {Department}, CreatedOn = {CreatedOn}, ");
        builder.Append($"LastLogin = {LastLogin}, SecurityStamp = {SecurityStamp}");
        return true;
    }
}