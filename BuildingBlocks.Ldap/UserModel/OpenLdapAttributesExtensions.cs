// Decompiled with JetBrains decompiler
// Type: Skoruba.Duende.IdentityServer.STS.Identity.Infrastructure.LdapExtension.UserModel.OpenLdapAttributesExtensions
// Assembly: Skoruba.Duende.IdentityServer.STS.Identity, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 425C0317-D64B-453B-BC95-043C64DD9F8A
// Assembly location: C:\Users\420919\Repositories\STS\STS\Skoruba.Duende.IdentityServer.STS.Identity.dll

#nullable disable
using System.ComponentModel;
using System.Reflection;

namespace BuildingBlocks.Ldap.UserModel;

public static class OpenLdapAttributesExtensions
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

    public static string ToDescriptionString(this OpenLdapAttributes val)
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