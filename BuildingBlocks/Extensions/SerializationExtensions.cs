// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Extensions.SerializationExtensions
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Text;
using Newtonsoft.Json;

namespace BuildingBlocks.Extensions;

public static class SerializationExtensions
{
  public static byte[] ToByteArray(this object obj)
  {
    return obj == null ? (byte[]) null : Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(obj));
  }

  public static T FromByteArray<T>(this byte[] byteArray) where T : class
  {
    return byteArray == null ? default (T) : JsonConvert.DeserializeObject<T>(Encoding.ASCII.GetString(byteArray));
  }
}