// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Common.Base64
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
namespace BuildingBlocks.Common;

public static class Base64
{
  public static string Encode(byte[] arg)
  {
    return Convert.ToBase64String(arg).Split('=', StringSplitOptions.None)[0].Replace('+', '-').Replace('/', '_');
  }

  public static byte[] Decode(string arg)
  {
    Ensure.NotNullOrEmptyOrWhiteSpace(arg);
    string str = arg.Replace('-', '+').Replace('_', '/');
    switch (str.Length % 4)
    {
      case 0:
        return Convert.FromBase64String(str);
      case 2:
        str += "==";
        goto case 0;
      case 3:
        str += "=";
        goto case 0;
      default:
        throw new InvalidDataException("Invalid Base64UrlSafe encoded string.");
    }
  }
}