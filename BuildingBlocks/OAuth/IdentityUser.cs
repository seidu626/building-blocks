using System.Text;
using System.Text.Json.Serialization;
using BuildingBlocks.Json;

namespace BuildingBlocks.OAuth;

public record IdentityUser
{
    [JsonPropertyName("sub")] public string? Sub { get; init; }

    [JsonPropertyName("name")] public string Name { get; init; }

    [JsonPropertyName("given_name")] public string GivenName { get; init; }

    [JsonPropertyName("manager")]
    [JsonConverter(typeof(NestedJsonStringConverter<ManagerInfo>))]
    public ManagerInfo Manager { get; set; }

    [JsonPropertyName("accountToken")] public string AccountToken { get; init; }

    [JsonPropertyName("employee_category")]
    public string EmployeeCategory { get; init; }

    [JsonPropertyName("preferred_username")]
    public string PreferredUsername { get; init; }

    [JsonPropertyName("family_name")] public string FamilyName { get; init; }

    [JsonPropertyName("email")] public string Email { get; init; }

    [JsonPropertyName("role")] public List<string> Role { get; init; } = new List<string>();


    [JsonPropertyName("updatedOn")] public List<string> UpdatedOn { get; init; }

    [JsonPropertyName("permissions")] public List<string> Permissions { get; init; } = new List<string>();


    [JsonPropertyName("phone_number")] public List<string> PhoneNumber { get; init; }


    [JsonPropertyName("badPwdCount")] public List<string> BadPasswordCount { get; init; }


    [JsonPropertyName("lockoutTime")] public List<string> LockoutTime { get; init; }


    [JsonPropertyName("pwdLastSet")] List<string> PasswordLastSet { get; init; }

    [JsonPropertyName("title")] public string Title { get; init; }

    [JsonPropertyName("description")] public string Description { get; init; }

    [JsonPropertyName("division")] public string Division { get; init; }

    [JsonPropertyName("department")] public string Department { get; init; }

    [JsonPropertyName("section")] public string Section { get; init; }

    [JsonPropertyName("employeeID")] public string EmployeeNumber { get; init; }

    [JsonPropertyName("createdOn")] public string CreatedOn { get; init; }

    [JsonPropertyName("last_login")] public List<string> LastLogin { get; set; }

    [JsonPropertyName("accountExpires")]
    [JsonConverter(typeof(FlexibleLongConverter))]
    public long AccountExpires { get; set; }

    [JsonPropertyName("lastLogonTimestamp")]
    [JsonConverter(typeof(FlexibleLongConverter))]
    public long LastLogonTimestamp { get; set; }


    [JsonPropertyName("AspNet.Identity.SecurityStamp")]
    public string SecurityStamp { get; init; }

    // Optional: If a more complex print of members is needed
    protected virtual bool PrintMembers(StringBuilder builder)
    {
        builder.Append($"Username = {PreferredUsername}, Sub = {Sub}, Name = {Name}, ");
        builder.Append(
            $"GivenName = {GivenName}, AccountToken = {AccountToken}, EmployeeCategory = {EmployeeCategory}, ");
        builder.Append($"PreferredUsername = {PreferredUsername}, FamilyName = {FamilyName}, Email = {Email}, ");
        builder.Append(
            $"Role = {string.Join(", ", Role)}, UpdatedOn = {UpdatedOn}, Permissions = {string.Join(", ", Permissions)}, ");
        builder.Append(
            $"PhoneNumber = {PhoneNumber}, BadPasswordCount = {BadPasswordCount}, LockoutTime = {LockoutTime}, ");
        builder.Append($"PasswordLastSet = {PasswordLastSet}, Title = {Title}, Description = {Description}, ");
        builder.Append($"Division = {Division}, Department = {Department}, CreatedOn = {CreatedOn}, ");
        builder.Append($"LastLogin = {LastLogin}, SecurityStamp = {SecurityStamp}");
        return true;
    }
}

public class ManagerInfo
{
    [JsonPropertyName("DisplayName")] public string DisplayName { get; set; }

    [JsonPropertyName("Fullname")] public string Fullname { get; set; }

    [JsonPropertyName("Email")] public string Email { get; set; }

    [JsonPropertyName("Title")] public string Title { get; set; }

    [JsonPropertyName("Department")] public string Department { get; set; }

    [JsonPropertyName("Division")] public string Division { get; set; }

    [JsonPropertyName("Mobile")] public string Mobile { get; set; }
}