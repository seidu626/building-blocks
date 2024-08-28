// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Helpers.SecurityHelper
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace BuildingBlocks.Helpers;

public static class SecurityHelper
{
  public static string GenerateSalt(int nSalt)
  {
    byte[] numArray = new byte[nSalt];
    using (RNGCryptoServiceProvider cryptoServiceProvider = new RNGCryptoServiceProvider())
      ((RandomNumberGenerator) cryptoServiceProvider).GetNonZeroBytes(numArray);
    return Convert.ToBase64String(numArray);
  }

  public static string HashPassword(string password, string salt, int nIterations, int nHash)
  {
    byte[] numArray = Convert.FromBase64String(salt);
    using (Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, numArray, nIterations))
      return Convert.ToBase64String(((DeriveBytes) rfc2898DeriveBytes).GetBytes(nHash));
  }

  public static string GetPin(int length)
  {
    byte[] numArray = new byte[4];
    RandomNumberGenerator.Create().GetBytes(numArray);
    uint num = BitConverter.ToUInt32(numArray, 0) % 100000000U;
    return string.Format("{0:D" + length.ToString() + "}", (object) num);
  }

  public static string createPinFromString(this string seed, int length, string uniqueHash = "9t45uufg92dit093ik,96igm0v9m6i09im09i309disl54923")
  {
    string str = "";
    do
    {
      string md5 = (seed + uniqueHash).CreateMD5();
      str += Regex.Split(md5, "\\D+")?.ToString();
      seed = str + md5;
    }
    while (str.Length < length);
    return str.Substring(0, length);
  }

  public static string CreateMD5(this string input)
  {
    using (MD5 md5 = MD5.Create())
    {
      byte[] bytes = Encoding.ASCII.GetBytes(input);
      byte[] hash = ((HashAlgorithm) md5).ComputeHash(bytes);
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < hash.Length; ++index)
        stringBuilder.Append(hash[index].ToString("X2"));
      return stringBuilder.ToString();
    }
  }

  public static string CreatePassword(int length, int numberOfNonAlphanumericCharacters)
  {
    StringBuilder stringBuilder = new StringBuilder();
    Random random = new Random();
    while (0 < length--)
      stringBuilder.Append("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890"[random.Next("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".Length)]);
    return stringBuilder.ToString();
  }

  public static string RandomString(int length, string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789")
  {
    if (length < 0)
      throw new ArgumentOutOfRangeException(nameof (length), "length cannot be less than zero.");
    char[] chArray = !string.IsNullOrEmpty(allowedChars) ? new HashSet<char>((IEnumerable<char>) allowedChars).ToArray<char>() : throw new ArgumentException("allowedChars may not be empty.");
    if (256 < chArray.Length)
    {
      DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(50, 1);
      interpolatedStringHandler.AppendLiteral("allowedChars may contain no more than ");
      interpolatedStringHandler.AppendFormatted<int>(256);
      interpolatedStringHandler.AppendLiteral(" characters.");
      throw new ArgumentException(interpolatedStringHandler.ToStringAndClear());
    }
    using (RNGCryptoServiceProvider cryptoServiceProvider = new RNGCryptoServiceProvider())
    {
      StringBuilder stringBuilder = new StringBuilder();
      byte[] numArray = new byte[128];
      label_13:
      while (stringBuilder.Length < length)
      {
        ((RandomNumberGenerator) cryptoServiceProvider).GetBytes(numArray);
        int index = 0;
        while (true)
        {
          if (index < numArray.Length && stringBuilder.Length < length)
          {
            if (256 - 256 % chArray.Length > (int) numArray[index])
              stringBuilder.Append(chArray[(int) numArray[index] % chArray.Length]);
            ++index;
          }
          else
            goto label_13;
        }
      }
      return stringBuilder.ToString();
    }
  }
}