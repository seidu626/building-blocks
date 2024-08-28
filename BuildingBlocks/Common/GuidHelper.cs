// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Common.GuidHelper
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
namespace BuildingBlocks.Common;

public static class GuidHelper
{
  public static Guid CreateSequentialUUID()
  {
    Guid guid;
    return NativeMethods.UuidCreateSequential(out guid) != 0 ? Guid.NewGuid() : guid;
  }

  public static Guid CreateComb()
  {
    byte[] byteArray = GuidHelper.CreateSequentialUUID().ToByteArray();
    DateTime dateTime = new DateTime(1900, 1, 1);
    DateTime utcNow = DateTime.UtcNow;
    TimeSpan timeSpan = new TimeSpan(utcNow.Ticks - dateTime.Ticks);
    TimeSpan timeOfDay = utcNow.TimeOfDay;
    byte[] bytes1 = BitConverter.GetBytes(timeSpan.Days);
    byte[] bytes2 = BitConverter.GetBytes((long) (timeOfDay.TotalMilliseconds / 3.333333));
    Array.Reverse<byte>(bytes1);
    Array.Reverse<byte>(bytes2);
    Array.Copy((Array) bytes1, bytes1.Length - 2, (Array) byteArray, byteArray.Length - 6, 2);
    Array.Copy((Array) bytes2, bytes2.Length - 4, (Array) byteArray, byteArray.Length - 4, 4);
    return new Guid(byteArray);
  }
}