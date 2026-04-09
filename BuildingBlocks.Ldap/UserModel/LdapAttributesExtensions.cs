using System.ComponentModel;
using System.Reflection;

namespace BuildingBlocks.Ldap.UserModel;

public static class LdapAttributesExtensions
{
    public static Array ToDescriptionArray<T>() where T : IConvertible
    {
        if (!typeof(T).IsEnum)
            throw new ArgumentException("T must be an enumerated type");
        
        var stringList = new List<string>();
        foreach (var value in Enum.GetValues(typeof(T)))
        {
            var field = value.GetType().GetField(value.ToString());
            if (field != null)
            {
                var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attributes.Length > 0 && attributes[0] is DescriptionAttribute descriptionAttr)
                {
                    if (!stringList.Contains(descriptionAttr.Description))
                        stringList.Add(descriptionAttr.Description);
                }
            }
        }
        return stringList.ToArray();
    }

    public static string ToDescriptionString(this ActiveDirectoryLdapAttributes val)
    {
        var field = val.GetType().GetField(val.ToString());
        if (field != null)
        {
            var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length > 0 && attributes[0] is DescriptionAttribute descriptionAttr)
            {
                return descriptionAttr.Description;
            }
        }
        return string.Empty;
    }
}