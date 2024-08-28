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
    if (!typeof (T).IsEnum)
      throw new ArgumentException("T must be an enumerated type");
    List<string> stringList = new List<string>();
    foreach (object obj in Enum.GetValues(typeof (T)))
    {
      string description = ((DescriptionAttribute[]) ((MemberInfo) obj.GetType().GetField(obj.ToString())).GetCustomAttributes(typeof (DescriptionAttribute), false))[0].Description;
      if (!stringList.Contains(description))
        stringList.Add(description);
    }
    return (Array) stringList.ToArray();
  }

  public static string ToDescriptionString(this OpenLdapAttributes val)
  {
    DescriptionAttribute[] customAttributes = (DescriptionAttribute[]) ((MemberInfo) val.GetType().GetField(val.ToString())).GetCustomAttributes(typeof (DescriptionAttribute), false);
    return customAttributes.Length == 0 ? string.Empty : customAttributes[0].Description;
  }
}