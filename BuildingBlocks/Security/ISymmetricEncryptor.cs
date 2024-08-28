// Decompiled with JetBrains decompiler
// Type: f14.Security.ISymmetricEncryptor
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
namespace BuildingBlocks.Security;

public interface ISymmetricEncryptor
{
    byte[] Encrypt(string data, byte[] key);

    string Decrypt(byte[] data, byte[] key);
}