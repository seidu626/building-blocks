using System.Security.Claims;
using System.Text.Json;
using BuildingBlocks.Ldap.Extensions;
using Novell.Directory.Ldap;

namespace BuildingBlocks.Ldap.UserModel;

public class ActiveDirectoryAppUser : IAppUser
{
    private string _subjectId;

    public string SubjectId
    {
        get => _subjectId ?? Username;
        set => _subjectId = value;
    }
    public string ProviderSubjectId { get; set; }
    public string? ProviderName { get; set; }
    public string DisplayName { get; set; }
    public string EmployeeNumber { get; set; }
    public string Username { get; set; }
    public string Fullname { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Email { get; set; }
    public string Department { get; set; }
    public string Division { get; set; }
    public string Mobile { get; set; }
    public long? LockoutTime { get; set; }
    public long? PwdLastSet { get; set; }
    public long? BadPwdCount { get; set; }
    public long? AccountExpires { get; set; }
    public ICollection<Claim> Claims { get; set; } = new List<Claim>();

    public string[] LdapAttributes => Enum<ActiveDirectoryLdapAttributes>.Descriptions;

    public bool IsActive => !IsAccountExpired() && !IsPasswordExpired() && !IsAccountLocked();

    public bool IsAccountLocked()
    {
        return LockoutTime.HasValue && LockoutTime.GetValueOrDefault() != 0L &&
               DateTime.FromFileTime(LockoutTime.Value).AddMinutes(30.0) >= DateTime.UtcNow;
    }

    public bool IsPasswordExpired(int maxPasswordAgeDays = 30)
    {
        return !PwdLastSet.HasValue || PwdLastSet.Value == 0L ||
               DateTime.FromFileTime(PwdLastSet.Value).AddDays(maxPasswordAgeDays) <= DateTime.UtcNow;
    }

    public bool IsAccountExpired()
    {
        if (!AccountExpires.HasValue || AccountExpires.Value == 0L)
            return false;
        if (AccountExpires.Value == long.MaxValue)
            return false;
        try
        {
            return DateTime.FromFileTimeUtc(AccountExpires.Value) <= DateTime.UtcNow;
        }
        catch (ArgumentOutOfRangeException)
        {
            return true;
        }
    }

    public void FillClaims(LdapEntry user, LdapEntry? manager = null)
    {
        Claims = new List<Claim>
        {
            GetClaimFromLdapAttributes(user, "name", ActiveDirectoryLdapAttributes.DisplayName),
            GetClaimFromLdapAttributes(user, "family_name", ActiveDirectoryLdapAttributes.LastName),
            GetClaimFromLdapAttributes(user, "given_name", ActiveDirectoryLdapAttributes.FirstName),
            GetClaimFromLdapAttributes(user, "email", ActiveDirectoryLdapAttributes.EMail),
            GetClaimFromLdapAttributes(user, "phone_number", ActiveDirectoryLdapAttributes.TelephoneNumber),
            GetClaimFromLdapAttributes(user, "fullName", ActiveDirectoryLdapAttributes.FullName),
            GetClaimFromLdapAttributes(user, "badPwdCount", ActiveDirectoryLdapAttributes.BadPwdCount),
            GetClaimFromLdapAttributes(user, "lockoutTime", ActiveDirectoryLdapAttributes.LockoutTime),
            GetClaimFromLdapAttributes(user, "pwdLastSet", ActiveDirectoryLdapAttributes.PwdLastSet),
            GetClaimFromLdapAttributes(user, "title", ActiveDirectoryLdapAttributes.Tile),
            GetClaimFromLdapAttributes(user, "description", ActiveDirectoryLdapAttributes.Description),
            GetClaimFromLdapAttributes(user, "division", ActiveDirectoryLdapAttributes.Division),
            GetClaimFromLdapAttributes(user, "department", ActiveDirectoryLdapAttributes.Department),
            GetClaimFromLdapAttributes(user, "createdOn", ActiveDirectoryLdapAttributes.CreatedOn),
            GetClaimFromLdapAttributes(user, "updatedOn", ActiveDirectoryLdapAttributes.UpdatedOn),
            GetClaimFromLdapAttributes(user, "employeeType", ActiveDirectoryLdapAttributes.EmployeeType)
        };
        
        if (manager != null)
        {
            var managerDetails = new Dictionary<string, string>
            {
                { "DisplayName", SafeGetAttributeStringValue(manager, ActiveDirectoryLdapAttributes.DisplayName.ToDescriptionString()) },
                { "Fullname", SafeGetAttributeStringValue(manager, ActiveDirectoryLdapAttributes.FullName.ToDescriptionString()) },
                { "Email", SafeGetAttributeStringValue(manager, ActiveDirectoryLdapAttributes.EMail.ToDescriptionString()) },
                { "Title", SafeGetAttributeStringValue(manager, ActiveDirectoryLdapAttributes.Tile.ToDescriptionString()) },
                { "Department", SafeGetAttributeStringValue(manager, ActiveDirectoryLdapAttributes.Department.ToDescriptionString()) },
                { "Division", SafeGetAttributeStringValue(manager, ActiveDirectoryLdapAttributes.Division.ToDescriptionString()) },
                { "Mobile", SafeGetAttributeStringValue(manager, ActiveDirectoryLdapAttributes.TelephoneNumber.ToDescriptionString()) }
            };
            var managerDetailsJson = JsonSerializer.Serialize(managerDetails);
            Claims.Add(new Claim("manager", managerDetailsJson));
        }

        try
        {
            var stringValues = user.GetAttribute(ActiveDirectoryLdapAttributes.MemberOf.ToDescriptionString())
                ?.StringValues;
            if (stringValues == null) return;
            while (stringValues.MoveNext())
            {
                var curr = stringValues.Current;
                if (curr != null)
                {
                    Claims.Add(new Claim("role", curr));
                }
            }
        }
        catch (Exception ex)
        {
            // Log exception if necessary
        }
    }
    

    private Claim GetClaimFromLdapAttributes(LdapEntry user, string claimType,
        ActiveDirectoryLdapAttributes ldapAttribute)
    {
        var value = SafeGetAttributeStringValue(user, ldapAttribute.ToDescriptionString()) ?? string.Empty;
        return new Claim(claimType, value);
    }


    public void SetBaseDetails(LdapEntry ldapEntry, LdapEntry? manager = null, string? providerName = "")
        {
            DisplayName = SafeGetAttributeStringValue(ldapEntry, ActiveDirectoryLdapAttributes.DisplayName.ToDescriptionString());
            Fullname = SafeGetAttributeStringValue(ldapEntry, ActiveDirectoryLdapAttributes.FullName.ToDescriptionString());
            EmployeeNumber = SafeGetAttributeStringValue(ldapEntry, ActiveDirectoryLdapAttributes.EmployeeNumber.ToDescriptionString());
            Username = SafeGetAttributeStringValue(ldapEntry, ActiveDirectoryLdapAttributes.UserName.ToDescriptionString());
            Email = SafeGetAttributeStringValue(ldapEntry, ActiveDirectoryLdapAttributes.EMail.ToDescriptionString());
            Title = SafeGetAttributeStringValue(ldapEntry, ActiveDirectoryLdapAttributes.Tile.ToDescriptionString());
            Description = SafeGetAttributeStringValue(ldapEntry, ActiveDirectoryLdapAttributes.Description.ToDescriptionString());
            Division = SafeGetAttributeStringValue(ldapEntry, ActiveDirectoryLdapAttributes.Division.ToDescriptionString());
            Department = SafeGetAttributeStringValue(ldapEntry, ActiveDirectoryLdapAttributes.Department.ToDescriptionString());
            Mobile = SafeGetAttributeStringValue(ldapEntry, ActiveDirectoryLdapAttributes.TelephoneNumber.ToDescriptionString());
            ProviderName = providerName;
            SubjectId = Username;
            ProviderSubjectId = Username;
            LockoutTime = ParseLong(SafeGetAttributeStringValue(ldapEntry, ActiveDirectoryLdapAttributes.LockoutTime.ToDescriptionString()));
            PwdLastSet = ParseLong(SafeGetAttributeStringValue(ldapEntry, ActiveDirectoryLdapAttributes.PwdLastSet.ToDescriptionString()));
            BadPwdCount = ParseLong(SafeGetAttributeStringValue(ldapEntry, ActiveDirectoryLdapAttributes.BadPwdCount.ToDescriptionString()));
            AccountExpires = ParseLong(SafeGetAttributeStringValue(ldapEntry, ActiveDirectoryLdapAttributes.AccountExpires.ToDescriptionString()));
            FillClaims(ldapEntry, manager);
        }
    
    private string? SafeGetAttributeStringValue(LdapEntry ldapEntry, string attributeName)
    {
        try
        {
            return ldapEntry.GetAttribute(attributeName)?.StringValue;
        }
        catch (KeyNotFoundException)
        {
            return null;
        }
    }

    private static long? ParseLong(string? value)
    {
        return long.TryParse(value, out var result) ? result : (long?)null;
    }
}