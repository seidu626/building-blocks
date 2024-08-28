// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Crypt.Decode
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Text;
using Newtonsoft.Json;

namespace BuildingBlocks.Crypt;

public static class Decode
{
  private const string OBFUSCATION_KEY_BEGIN = "c3f484ed634815b7845f106fa8d56b8846ca2823f9974fd3ed9d2b1b";
  private const string OBFUSCATION_KEY_END = "c3f484ed634815b7845f106fa8d56b8846ca2823f9974fd3ed9d2b1b";
  private const string SEP = "^$$^";

  public static List<CryptStructure> Execute(string base64)
  {
    return JsonConvert.DeserializeObject<List<CryptStructure>>(Encoding.UTF8.GetString(Convert.FromBase64String(base64.Split("^$$^", StringSplitOptions.None)[1])));
  }
}