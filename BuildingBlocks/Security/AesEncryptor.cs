// Decompiled with JetBrains decompiler
// Type: f14.Security.AesEncryptor
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Security.Cryptography;

namespace BuildingBlocks.Security;

public sealed class AesEncryptor : ISymmetricEncryptor
{
    public byte[] Encrypt(string data, byte[] key)
    {
        using (Aes aes = Aes.Create())
        {
            ((SymmetricAlgorithm)aes).Key = key;
            ((SymmetricAlgorithm)aes).GenerateIV();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                ((Stream)memoryStream).Write(((SymmetricAlgorithm)aes).IV, 0, 16);
                ICryptoTransform encryptor =
                    ((SymmetricAlgorithm)aes).CreateEncryptor(((SymmetricAlgorithm)aes).Key,
                        ((SymmetricAlgorithm)aes).IV);
                using (CryptoStream cryptoStream =
                       new CryptoStream((Stream)memoryStream, encryptor, (CryptoStreamMode)1))
                {
                    using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        ((TextWriter)streamWriter).Write(data);
                    return memoryStream.ToArray();
                }
            }
        }
    }

    public string Decrypt(byte[] data, byte[] key)
    {
        using (Aes aes = Aes.Create())
        {
            ((SymmetricAlgorithm)aes).Key = key;
            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                byte[] numArray = new byte[16];
                ((Stream)memoryStream).Read(numArray, 0, 16);
                ((SymmetricAlgorithm)aes).IV = numArray;
                ICryptoTransform decryptor =
                    ((SymmetricAlgorithm)aes).CreateDecryptor(((SymmetricAlgorithm)aes).Key,
                        ((SymmetricAlgorithm)aes).IV);
                using (CryptoStream cryptoStream =
                       new CryptoStream((Stream)memoryStream, decryptor, (CryptoStreamMode)0))
                {
                    using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        return ((TextReader)streamReader).ReadToEnd();
                }
            }
        }
    }
}