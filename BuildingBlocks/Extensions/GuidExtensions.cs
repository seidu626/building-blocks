// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Extensions.GuidExtensions
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
namespace BuildingBlocks.Extensions;

public static class GuidExtensions
{
  public static string AsShortCodeBase64(this Guid guid, bool trimEnd = true)
  {
    string base64String = Convert.ToBase64String(guid.ToByteArray());
    return !trimEnd ? base64String : base64String.Substring(0, base64String.Length - 2);
  }

  public static string AsShortCode(this Guid guid)
  {
    long num1 = 1;
    foreach (byte num2 in guid.ToByteArray())
      num1 *= (long) ((int) num2 + 1);
    return (num1 - DateTime.Now.Ticks).ToString("x");
  }

  public static long AsNumber(this Guid guid) => BitConverter.ToInt64(guid.ToByteArray(), 0);
}