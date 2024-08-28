// Decompiled with JetBrains decompiler
// Type: Skoruba.Duende.IdentityServer.STS.Identity.Infrastructure.LdapExtension.Extensions.Enum`1
// Assembly: Skoruba.Duende.IdentityServer.STS.Identity, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 425C0317-D64B-453B-BC95-043C64DD9F8A
// Assembly location: C:\Users\420919\Repositories\STS\STS\Skoruba.Duende.IdentityServer.STS.Identity.dll

#nullable disable
using System.ComponentModel;
using System.Reflection;

namespace BuildingBlocks.Ldap.Extensions;

internal class Enum<T> where T : struct, IConvertible
{
  public static string[] Descriptions
  {
    get
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
      return stringList.ToArray();
    }
  }
}