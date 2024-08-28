#nullable enable
namespace BuildingBlocks.Ldap.UserModel;

public static class ActiveDirectoryLdapAttributesExtensions
{
    public const int Length = 20;

    public static string ToStringFast(this ActiveDirectoryLdapAttributes value)
    {
        return value switch
        {
            ActiveDirectoryLdapAttributes.DisplayName => "displayName",
            ActiveDirectoryLdapAttributes.FirstName => "givenName",
            ActiveDirectoryLdapAttributes.LastName => "sn",
            ActiveDirectoryLdapAttributes.FullName => "cn",
            ActiveDirectoryLdapAttributes.Description => "description",
            ActiveDirectoryLdapAttributes.TelephoneNumber => "mobile",
            ActiveDirectoryLdapAttributes.Name => "name",
            ActiveDirectoryLdapAttributes.CreatedOn => "whenCreated",
            ActiveDirectoryLdapAttributes.UpdatedOn => "whenChanged",
            ActiveDirectoryLdapAttributes.UserName => "sAMAccountName",
            ActiveDirectoryLdapAttributes.EMail => "mail",
            ActiveDirectoryLdapAttributes.Tile => "title",
            ActiveDirectoryLdapAttributes.EmployeeNumber => "employeeID",
            ActiveDirectoryLdapAttributes.Department => "department",
            ActiveDirectoryLdapAttributes.Division => "division",
            ActiveDirectoryLdapAttributes.LockoutTime => "lockoutTime",
            ActiveDirectoryLdapAttributes.BadPwdCount => "badPwdCount",
            ActiveDirectoryLdapAttributes.PwdLastSet => "pwdLastSet",
            ActiveDirectoryLdapAttributes.AccountExpires => "accountExpires",
            ActiveDirectoryLdapAttributes.MemberOf => "memberOf",
            _ => value.ToString()
        };
    }

    public static bool IsDefined(ActiveDirectoryLdapAttributes value)
    {
        return value switch
        {
            ActiveDirectoryLdapAttributes.DisplayName => true,
            ActiveDirectoryLdapAttributes.FirstName => true,
            ActiveDirectoryLdapAttributes.LastName => true,
            ActiveDirectoryLdapAttributes.FullName => true,
            ActiveDirectoryLdapAttributes.Description => true,
            ActiveDirectoryLdapAttributes.TelephoneNumber => true,
            ActiveDirectoryLdapAttributes.Name => true,
            ActiveDirectoryLdapAttributes.CreatedOn => true,
            ActiveDirectoryLdapAttributes.UpdatedOn => true,
            ActiveDirectoryLdapAttributes.UserName => true,
            ActiveDirectoryLdapAttributes.EMail => true,
            ActiveDirectoryLdapAttributes.Tile => true,
            ActiveDirectoryLdapAttributes.EmployeeNumber => true,
            ActiveDirectoryLdapAttributes.Department => true,
            ActiveDirectoryLdapAttributes.Division => true,
            ActiveDirectoryLdapAttributes.LockoutTime => true,
            ActiveDirectoryLdapAttributes.BadPwdCount => true,
            ActiveDirectoryLdapAttributes.PwdLastSet => true,
            ActiveDirectoryLdapAttributes.AccountExpires => true,
            ActiveDirectoryLdapAttributes.MemberOf => true,
            _ => false
        };
    }

    public static bool IsDefined(string name)
    {
        return IsDefined(name, false);
    }

    public static bool IsDefined(string name, bool allowMatchingMetadataAttribute)
    {
        return allowMatchingMetadataAttribute
            ? name switch
            {
                "displayName" => true,
                "givenName" => true,
                "sn" => true,
                "cn" => true,
                "description" => true,
                "mobile" => true,
                "name" => true,
                "whenCreated" => true,
                "whenChanged" => true,
                "sAMAccountName" => true,
                "mail" => true,
                "title" => true,
                "employeeID" => true,
                "department" => true,
                "division" => true,
                "lockoutTime" => true,
                "badPwdCount" => true,
                "pwdLastSet" => true,
                "accountExpires" => true,
                "memberOf" => true,
                _ => false
            }
            : name switch
            {
                "DisplayName" => true,
                "FirstName" => true,
                "LastName" => true,
                "FullName" => true,
                "Description" => true,
                "TelephoneNumber" => true,
                "Name" => true,
                "CreatedOn" => true,
                "UpdatedOn" => true,
                "UserName" => true,
                "EMail" => true,
                "Tile" => true,
                "EmployeeNumber" => true,
                "Department" => true,
                "Division" => true,
                "LockoutTime" => true,
                "BadPwdCount" => true,
                "PwdLastSet" => true,
                "AccountExpires" => true,
                "MemberOf" => true,
                _ => false
            };
    }

    public static bool IsDefined(ReadOnlySpan<char> name)
    {
        return IsDefined(name, false);
    }

    public static bool IsDefined(ReadOnlySpan<char> name, bool allowMatchingMetadataAttribute)
    {
        return allowMatchingMetadataAttribute
            ? name.Equals("displayName".AsSpan(), StringComparison.Ordinal) ||
              name.Equals("givenName".AsSpan(), StringComparison.Ordinal) ||
              name.Equals("sn".AsSpan(), StringComparison.Ordinal) ||
              name.Equals("cn".AsSpan(), StringComparison.Ordinal) ||
              name.Equals("description".AsSpan(), StringComparison.Ordinal) ||
              name.Equals("mobile".AsSpan(), StringComparison.Ordinal) ||
              name.Equals("name".AsSpan(), StringComparison.Ordinal) ||
              name.Equals("whenCreated".AsSpan(), StringComparison.Ordinal) ||
              name.Equals("whenChanged".AsSpan(), StringComparison.Ordinal) ||
              name.Equals("sAMAccountName".AsSpan(), StringComparison.Ordinal) ||
              name.Equals("mail".AsSpan(), StringComparison.Ordinal) ||
              name.Equals("title".AsSpan(), StringComparison.Ordinal) ||
              name.Equals("employeeID".AsSpan(), StringComparison.Ordinal) ||
              name.Equals("department".AsSpan(), StringComparison.Ordinal) ||
              name.Equals("division".AsSpan(), StringComparison.Ordinal) ||
              name.Equals("lockoutTime".AsSpan(), StringComparison.Ordinal) ||
              name.Equals("badPwdCount".AsSpan(), StringComparison.Ordinal) ||
              name.Equals("pwdLastSet".AsSpan(), StringComparison.Ordinal) ||
              name.Equals("accountExpires".AsSpan(), StringComparison.Ordinal) ||
              name.Equals("memberOf".AsSpan(), StringComparison.Ordinal)
            : name.Equals("DisplayName".AsSpan(), StringComparison.Ordinal) ||
              name.Equals("FirstName".AsSpan(), StringComparison.Ordinal) ||
              name.Equals("LastName".AsSpan(), StringComparison.Ordinal) ||
              name.Equals("FullName".AsSpan(), StringComparison.Ordinal) ||
              name.Equals("Description".AsSpan(), StringComparison.Ordinal) ||
              name.Equals("TelephoneNumber".AsSpan(), StringComparison.Ordinal) ||
              name.Equals("Name".AsSpan(), StringComparison.Ordinal) ||
              name.Equals("CreatedOn".AsSpan(), StringComparison.Ordinal) ||
              name.Equals("UpdatedOn".AsSpan(), StringComparison.Ordinal) ||
              name.Equals("UserName".AsSpan(), StringComparison.Ordinal) ||
              name.Equals("EMail".AsSpan(), StringComparison.Ordinal) ||
              name.Equals("Tile".AsSpan(), StringComparison.Ordinal) ||
              name.Equals("EmployeeNumber".AsSpan(), StringComparison.Ordinal) ||
              name.Equals("Department".AsSpan(), StringComparison.Ordinal) ||
              name.Equals("Division".AsSpan(), StringComparison.Ordinal) ||
              name.Equals("LockoutTime".AsSpan(), StringComparison.Ordinal) ||
              name.Equals("BadPwdCount".AsSpan(), StringComparison.Ordinal) ||
              name.Equals("PwdLastSet".AsSpan(), StringComparison.Ordinal) ||
              name.Equals("AccountExpires".AsSpan(), StringComparison.Ordinal) ||
              name.Equals("MemberOf".AsSpan(), StringComparison.Ordinal);
    }

    public static bool TryParse(string? name, out ActiveDirectoryLdapAttributes value)
    {
        return TryParse(name, out value, false, false);
    }

    public static bool TryParse(string? name, out ActiveDirectoryLdapAttributes value, bool ignoreCase)
    {
        return TryParse(name, out value, ignoreCase, false);
    }

    public static bool TryParse(string? name, out ActiveDirectoryLdapAttributes value, bool ignoreCase, bool allowMatchingMetadataAttribute)
    {
        if (allowMatchingMetadataAttribute)
        {
            if (ignoreCase)
            {
                return name?.ToLower() switch
                {
                    "displayname" => ReturnValue(ActiveDirectoryLdapAttributes.DisplayName, out value),
                    "givenname" => ReturnValue(ActiveDirectoryLdapAttributes.FirstName, out value),
                    "sn" => ReturnValue(ActiveDirectoryLdapAttributes.LastName, out value),
                    "cn" => ReturnValue(ActiveDirectoryLdapAttributes.FullName, out value),
                    "description" => ReturnValue(ActiveDirectoryLdapAttributes.Description, out value),
                    "mobile" => ReturnValue(ActiveDirectoryLdapAttributes.TelephoneNumber, out value),
                    "name" => ReturnValue(ActiveDirectoryLdapAttributes.Name, out value),
                    "whencreated" => ReturnValue(ActiveDirectoryLdapAttributes.CreatedOn, out value),
                    "whenchanged" => ReturnValue(ActiveDirectoryLdapAttributes.UpdatedOn, out value),
                    "samaccountname" => ReturnValue(ActiveDirectoryLdapAttributes.UserName, out value),
                    "mail" => ReturnValue(ActiveDirectoryLdapAttributes.EMail, out value),
                    "title" => ReturnValue(ActiveDirectoryLdapAttributes.Tile, out value),
                    "employeeid" => ReturnValue(ActiveDirectoryLdapAttributes.EmployeeNumber, out value),
                    "department" => ReturnValue(ActiveDirectoryLdapAttributes.Department, out value),
                    "division" => ReturnValue(ActiveDirectoryLdapAttributes.Division, out value),
                    "lockouttime" => ReturnValue(ActiveDirectoryLdapAttributes.LockoutTime, out value),
                    "badpwdcount" => ReturnValue(ActiveDirectoryLdapAttributes.BadPwdCount, out value),
                    "pwdlastset" => ReturnValue(ActiveDirectoryLdapAttributes.PwdLastSet, out value),
                    "accountexpires" => ReturnValue(ActiveDirectoryLdapAttributes.AccountExpires, out value),
                    "memberof" => ReturnValue(ActiveDirectoryLdapAttributes.MemberOf, out value),
                    _ => ReturnValue(ActiveDirectoryLdapAttributes.DisplayName, out value, false)
                };
            }
            else
            {
                return name switch
                {
                    "displayName" => ReturnValue(ActiveDirectoryLdapAttributes.DisplayName, out value),
                    "givenName" => ReturnValue(ActiveDirectoryLdapAttributes.FirstName, out value),
                    "sn" => ReturnValue(ActiveDirectoryLdapAttributes.LastName, out value),
                    "cn" => ReturnValue(ActiveDirectoryLdapAttributes.FullName, out value),
                    "description" => ReturnValue(ActiveDirectoryLdapAttributes.Description, out value),
                    "mobile" => ReturnValue(ActiveDirectoryLdapAttributes.TelephoneNumber, out value),
                    "name" => ReturnValue(ActiveDirectoryLdapAttributes.Name, out value),
                    "whenCreated" => ReturnValue(ActiveDirectoryLdapAttributes.CreatedOn, out value),
                    "whenChanged" => ReturnValue(ActiveDirectoryLdapAttributes.UpdatedOn, out value),
                    "sAMAccountName" => ReturnValue(ActiveDirectoryLdapAttributes.UserName, out value),
                    "mail" => ReturnValue(ActiveDirectoryLdapAttributes.EMail, out value),
                    "title" => ReturnValue(ActiveDirectoryLdapAttributes.Tile, out value),
                    "employeeID" => ReturnValue(ActiveDirectoryLdapAttributes.EmployeeNumber, out value),
                    "department" => ReturnValue(ActiveDirectoryLdapAttributes.Department, out value),
                    "division" => ReturnValue(ActiveDirectoryLdapAttributes.Division, out value),
                    "lockoutTime" => ReturnValue(ActiveDirectoryLdapAttributes.LockoutTime, out value),
                    "badPwdCount" => ReturnValue(ActiveDirectoryLdapAttributes.BadPwdCount, out value),
                    "pwdLastSet" => ReturnValue(ActiveDirectoryLdapAttributes.PwdLastSet, out value),
                    "accountExpires" => ReturnValue(ActiveDirectoryLdapAttributes.AccountExpires, out value),
                    "memberOf" => ReturnValue(ActiveDirectoryLdapAttributes.MemberOf, out value),
                    _ => ReturnValue(ActiveDirectoryLdapAttributes.DisplayName, out value, false)
                };
            }
        }

        if (ignoreCase)
        {
            return name?.ToLower() switch
            {
                "displayname" => ReturnValue(ActiveDirectoryLdapAttributes.DisplayName, out value),
                "firstname" => ReturnValue(ActiveDirectoryLdapAttributes.FirstName, out value),
                "lastname" => ReturnValue(ActiveDirectoryLdapAttributes.LastName, out value),
                "fullname" => ReturnValue(ActiveDirectoryLdapAttributes.FullName, out value),
                "description" => ReturnValue(ActiveDirectoryLdapAttributes.Description, out value),
                "telephonenumber" => ReturnValue(ActiveDirectoryLdapAttributes.TelephoneNumber, out value),
                "name" => ReturnValue(ActiveDirectoryLdapAttributes.Name, out value),
                "createdon" => ReturnValue(ActiveDirectoryLdapAttributes.CreatedOn, out value),
                "updatedon" => ReturnValue(ActiveDirectoryLdapAttributes.UpdatedOn, out value),
                "username" => ReturnValue(ActiveDirectoryLdapAttributes.UserName, out value),
                "email" => ReturnValue(ActiveDirectoryLdapAttributes.EMail, out value),
                "tile" => ReturnValue(ActiveDirectoryLdapAttributes.Tile, out value),
                "employeenumber" => ReturnValue(ActiveDirectoryLdapAttributes.EmployeeNumber, out value),
                "department" => ReturnValue(ActiveDirectoryLdapAttributes.Department, out value),
                "division" => ReturnValue(ActiveDirectoryLdapAttributes.Division, out value),
                "lockouttime" => ReturnValue(ActiveDirectoryLdapAttributes.LockoutTime, out value),
                "badpwdcount" => ReturnValue(ActiveDirectoryLdapAttributes.BadPwdCount, out value),
                "pwdlastset" => ReturnValue(ActiveDirectoryLdapAttributes.PwdLastSet, out value),
                "accountexpires" => ReturnValue(ActiveDirectoryLdapAttributes.AccountExpires, out value),
                "memberof" => ReturnValue(ActiveDirectoryLdapAttributes.MemberOf, out value),
                _ => ReturnValue(ActiveDirectoryLdapAttributes.DisplayName, out value, false)
            };
        }

        return name switch
        {
            "DisplayName" => ReturnValue(ActiveDirectoryLdapAttributes.DisplayName, out value),
            "FirstName" => ReturnValue(ActiveDirectoryLdapAttributes.FirstName, out value),
            "LastName" => ReturnValue(ActiveDirectoryLdapAttributes.LastName, out value),
            "FullName" => ReturnValue(ActiveDirectoryLdapAttributes.FullName, out value),
            "Description" => ReturnValue(ActiveDirectoryLdapAttributes.Description, out value),
            "TelephoneNumber" => ReturnValue(ActiveDirectoryLdapAttributes.TelephoneNumber, out value),
            "Name" => ReturnValue(ActiveDirectoryLdapAttributes.Name, out value),
            "CreatedOn" => ReturnValue(ActiveDirectoryLdapAttributes.CreatedOn, out value),
            "UpdatedOn" => ReturnValue(ActiveDirectoryLdapAttributes.UpdatedOn, out value),
            "UserName" => ReturnValue(ActiveDirectoryLdapAttributes.UserName, out value),
            "EMail" => ReturnValue(ActiveDirectoryLdapAttributes.EMail, out value),
            "Tile" => ReturnValue(ActiveDirectoryLdapAttributes.Tile, out value),
            "EmployeeNumber" => ReturnValue(ActiveDirectoryLdapAttributes.EmployeeNumber, out value),
            "Department" => ReturnValue(ActiveDirectoryLdapAttributes.Department, out value),
            "Division" => ReturnValue(ActiveDirectoryLdapAttributes.Division, out value),
            "LockoutTime" => ReturnValue(ActiveDirectoryLdapAttributes.LockoutTime, out value),
            "BadPwdCount" => ReturnValue(ActiveDirectoryLdapAttributes.BadPwdCount, out value),
            "PwdLastSet" => ReturnValue(ActiveDirectoryLdapAttributes.PwdLastSet, out value),
            "AccountExpires" => ReturnValue(ActiveDirectoryLdapAttributes.AccountExpires, out value),
            "MemberOf" => ReturnValue(ActiveDirectoryLdapAttributes.MemberOf, out value),
            _ => ReturnValue(ActiveDirectoryLdapAttributes.DisplayName, out value, false)
        };
    }

    public static bool TryParse(ReadOnlySpan<char> name, out ActiveDirectoryLdapAttributes value)
    {
        return TryParse(name, out value, false, false);
    }

    public static bool TryParse(ReadOnlySpan<char> name, out ActiveDirectoryLdapAttributes value, bool ignoreCase)
    {
        return TryParse(name, out value, ignoreCase, false);
    }

    public static bool TryParse(ReadOnlySpan<char> name, out ActiveDirectoryLdapAttributes value, bool ignoreCase, bool allowMatchingMetadataAttribute)
    {
        if (allowMatchingMetadataAttribute)
        {
            if (ignoreCase)
            {
                return name.ToString().ToLower() switch
                {
                    "displayname" => ReturnValue(ActiveDirectoryLdapAttributes.DisplayName, out value),
                    "givenname" => ReturnValue(ActiveDirectoryLdapAttributes.FirstName, out value),
                    "sn" => ReturnValue(ActiveDirectoryLdapAttributes.LastName, out value),
                    "cn" => ReturnValue(ActiveDirectoryLdapAttributes.FullName, out value),
                    "description" => ReturnValue(ActiveDirectoryLdapAttributes.Description, out value),
                    "mobile" => ReturnValue(ActiveDirectoryLdapAttributes.TelephoneNumber, out value),
                    "name" => ReturnValue(ActiveDirectoryLdapAttributes.Name, out value),
                    "whencreated" => ReturnValue(ActiveDirectoryLdapAttributes.CreatedOn, out value),
                    "whenchanged" => ReturnValue(ActiveDirectoryLdapAttributes.UpdatedOn, out value),
                    "samaccountname" => ReturnValue(ActiveDirectoryLdapAttributes.UserName, out value),
                    "mail" => ReturnValue(ActiveDirectoryLdapAttributes.EMail, out value),
                    "title" => ReturnValue(ActiveDirectoryLdapAttributes.Tile, out value),
                    "employeeid" => ReturnValue(ActiveDirectoryLdapAttributes.EmployeeNumber, out value),
                    "department" => ReturnValue(ActiveDirectoryLdapAttributes.Department, out value),
                    "division" => ReturnValue(ActiveDirectoryLdapAttributes.Division, out value),
                    "lockouttime" => ReturnValue(ActiveDirectoryLdapAttributes.LockoutTime, out value),
                    "badpwdcount" => ReturnValue(ActiveDirectoryLdapAttributes.BadPwdCount, out value),
                    "pwdlastset" => ReturnValue(ActiveDirectoryLdapAttributes.PwdLastSet, out value),
                    "accountexpires" => ReturnValue(ActiveDirectoryLdapAttributes.AccountExpires, out value),
                    "memberof" => ReturnValue(ActiveDirectoryLdapAttributes.MemberOf, out value),
                    _ => ReturnValue(ActiveDirectoryLdapAttributes.DisplayName, out value, false)
                };
            }
            else
            {
                return name.ToString() switch
                {
                    "displayName" => ReturnValue(ActiveDirectoryLdapAttributes.DisplayName, out value),
                    "givenName" => ReturnValue(ActiveDirectoryLdapAttributes.FirstName, out value),
                    "sn" => ReturnValue(ActiveDirectoryLdapAttributes.LastName, out value),
                    "cn" => ReturnValue(ActiveDirectoryLdapAttributes.FullName, out value),
                    "description" => ReturnValue(ActiveDirectoryLdapAttributes.Description, out value),
                    "mobile" => ReturnValue(ActiveDirectoryLdapAttributes.TelephoneNumber, out value),
                    "name" => ReturnValue(ActiveDirectoryLdapAttributes.Name, out value),
                    "whenCreated" => ReturnValue(ActiveDirectoryLdapAttributes.CreatedOn, out value),
                    "whenChanged" => ReturnValue(ActiveDirectoryLdapAttributes.UpdatedOn, out value),
                    "sAMAccountName" => ReturnValue(ActiveDirectoryLdapAttributes.UserName, out value),
                    "mail" => ReturnValue(ActiveDirectoryLdapAttributes.EMail, out value),
                    "title" => ReturnValue(ActiveDirectoryLdapAttributes.Tile, out value),
                    "employeeID" => ReturnValue(ActiveDirectoryLdapAttributes.EmployeeNumber, out value),
                    "department" => ReturnValue(ActiveDirectoryLdapAttributes.Department, out value),
                    "division" => ReturnValue(ActiveDirectoryLdapAttributes.Division, out value),
                    "lockoutTime" => ReturnValue(ActiveDirectoryLdapAttributes.LockoutTime, out value),
                    "badPwdCount" => ReturnValue(ActiveDirectoryLdapAttributes.BadPwdCount, out value),
                    "pwdLastSet" => ReturnValue(ActiveDirectoryLdapAttributes.PwdLastSet, out value),
                    "accountExpires" => ReturnValue(ActiveDirectoryLdapAttributes.AccountExpires, out value),
                    "memberOf" => ReturnValue(ActiveDirectoryLdapAttributes.MemberOf, out value),
                    _ => ReturnValue(ActiveDirectoryLdapAttributes.DisplayName, out value, false)
                };
            }
        }

        if (ignoreCase)
        {
            return name.ToString().ToLower() switch
            {
                "displayname" => ReturnValue(ActiveDirectoryLdapAttributes.DisplayName, out value),
                "firstname" => ReturnValue(ActiveDirectoryLdapAttributes.FirstName, out value),
                "lastname" => ReturnValue(ActiveDirectoryLdapAttributes.LastName, out value),
                "fullname" => ReturnValue(ActiveDirectoryLdapAttributes.FullName, out value),
                "description" => ReturnValue(ActiveDirectoryLdapAttributes.Description, out value),
                "telephonenumber" => ReturnValue(ActiveDirectoryLdapAttributes.TelephoneNumber, out value),
                "name" => ReturnValue(ActiveDirectoryLdapAttributes.Name, out value),
                "createdon" => ReturnValue(ActiveDirectoryLdapAttributes.CreatedOn, out value),
                "updatedon" => ReturnValue(ActiveDirectoryLdapAttributes.UpdatedOn, out value),
                "username" => ReturnValue(ActiveDirectoryLdapAttributes.UserName, out value),
                "email" => ReturnValue(ActiveDirectoryLdapAttributes.EMail, out value),
                "tile" => ReturnValue(ActiveDirectoryLdapAttributes.Tile, out value),
                "employeenumber" => ReturnValue(ActiveDirectoryLdapAttributes.EmployeeNumber, out value),
                "department" => ReturnValue(ActiveDirectoryLdapAttributes.Department, out value),
                "division" => ReturnValue(ActiveDirectoryLdapAttributes.Division, out value),
                "lockouttime" => ReturnValue(ActiveDirectoryLdapAttributes.LockoutTime, out value),
                "badpwdcount" => ReturnValue(ActiveDirectoryLdapAttributes.BadPwdCount, out value),
                "pwdlastset" => ReturnValue(ActiveDirectoryLdapAttributes.PwdLastSet, out value),
                "accountexpires" => ReturnValue(ActiveDirectoryLdapAttributes.AccountExpires, out value),
                "memberof" => ReturnValue(ActiveDirectoryLdapAttributes.MemberOf, out value),
                _ => ReturnValue(ActiveDirectoryLdapAttributes.DisplayName, out value, false)
            };
        }

        return name.ToString() switch
        {
            "DisplayName" => ReturnValue(ActiveDirectoryLdapAttributes.DisplayName, out value),
            "FirstName" => ReturnValue(ActiveDirectoryLdapAttributes.FirstName, out value),
            "LastName" => ReturnValue(ActiveDirectoryLdapAttributes.LastName, out value),
            "FullName" => ReturnValue(ActiveDirectoryLdapAttributes.FullName, out value),
            "Description" => ReturnValue(ActiveDirectoryLdapAttributes.Description, out value),
            "TelephoneNumber" => ReturnValue(ActiveDirectoryLdapAttributes.TelephoneNumber, out value),
            "Name" => ReturnValue(ActiveDirectoryLdapAttributes.Name, out value),
            "CreatedOn" => ReturnValue(ActiveDirectoryLdapAttributes.CreatedOn, out value),
            "UpdatedOn" => ReturnValue(ActiveDirectoryLdapAttributes.UpdatedOn, out value),
            "UserName" => ReturnValue(ActiveDirectoryLdapAttributes.UserName, out value),
            "EMail" => ReturnValue(ActiveDirectoryLdapAttributes.EMail, out value),
            "Tile" => ReturnValue(ActiveDirectoryLdapAttributes.Tile, out value),
            "EmployeeNumber" => ReturnValue(ActiveDirectoryLdapAttributes.EmployeeNumber, out value),
            "Department" => ReturnValue(ActiveDirectoryLdapAttributes.Department, out value),
            "Division" => ReturnValue(ActiveDirectoryLdapAttributes.Division, out value),
            "LockoutTime" => ReturnValue(ActiveDirectoryLdapAttributes.LockoutTime, out value),
            "BadPwdCount" => ReturnValue(ActiveDirectoryLdapAttributes.BadPwdCount, out value),
            "PwdLastSet" => ReturnValue(ActiveDirectoryLdapAttributes.PwdLastSet, out value),
            "AccountExpires" => ReturnValue(ActiveDirectoryLdapAttributes.AccountExpires, out value),
            "MemberOf" => ReturnValue(ActiveDirectoryLdapAttributes.MemberOf, out value),
            _ => ReturnValue(ActiveDirectoryLdapAttributes.DisplayName, out value, false)
        };
    }

    private static bool ReturnValue(ActiveDirectoryLdapAttributes attribute, out ActiveDirectoryLdapAttributes value, bool returnValue = true)
    {
        value = attribute;
        return returnValue;
    }

    public static ActiveDirectoryLdapAttributes[] GetValues()
    {
        return new ActiveDirectoryLdapAttributes[]
        {
            ActiveDirectoryLdapAttributes.DisplayName,
            ActiveDirectoryLdapAttributes.FirstName,
            ActiveDirectoryLdapAttributes.LastName,
            ActiveDirectoryLdapAttributes.FullName,
            ActiveDirectoryLdapAttributes.Description,
            ActiveDirectoryLdapAttributes.TelephoneNumber,
            ActiveDirectoryLdapAttributes.Name,
            ActiveDirectoryLdapAttributes.CreatedOn,
            ActiveDirectoryLdapAttributes.UpdatedOn,
            ActiveDirectoryLdapAttributes.UserName,
            ActiveDirectoryLdapAttributes.EMail,
            ActiveDirectoryLdapAttributes.Tile,
            ActiveDirectoryLdapAttributes.EmployeeNumber,
            ActiveDirectoryLdapAttributes.Department,
            ActiveDirectoryLdapAttributes.Division,
            ActiveDirectoryLdapAttributes.LockoutTime,
            ActiveDirectoryLdapAttributes.BadPwdCount,
            ActiveDirectoryLdapAttributes.PwdLastSet,
            ActiveDirectoryLdapAttributes.AccountExpires,
            ActiveDirectoryLdapAttributes.MemberOf
        };
    }

    public static string[] GetNames()
    {
        return new string[]
        {
            "DisplayName",
            "FirstName",
            "LastName",
            "FullName",
            "Description",
            "TelephoneNumber",
            "Name",
            "CreatedOn",
            "UpdatedOn",
            "UserName",
            "EMail",
            "Tile",
            "EmployeeNumber",
            "Department",
            "Division",
            "LockoutTime",
            "BadPwdCount",
            "PwdLastSet",
            "AccountExpires",
            "MemberOf"
        };
    }
}