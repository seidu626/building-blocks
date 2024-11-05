using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace BuildingBlocks.Helpers
{
    public static class SecurityHelper
    {
        public static string GenerateSalt(int saltLength)
        {
            byte[] saltBytes = new byte[saltLength];
            using (var cryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                cryptoServiceProvider.GetNonZeroBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        public static string HashPassword(string password, string salt, int iterations, int hashLength)
        {
            byte[] saltBytes = Convert.FromBase64String(salt);
            using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltBytes, iterations))
            {
                return Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(hashLength));
            }
        }

        public static string GetPin(int length)
        {
            byte[] randomBytes = new byte[4];
            RandomNumberGenerator.Create().GetBytes(randomBytes);
            uint randomNumber = BitConverter.ToUInt32(randomBytes, 0) % 100000000U;
            return randomNumber.ToString($"D{length}");
        }

        public static string CreatePinFromString(this string seed, int length, string uniqueHash = "9t45uufg92dit093ik,96igm0v9m6i09im09i309disl54923")
        {
            StringBuilder pinBuilder = new StringBuilder();
            while (pinBuilder.Length < length)
            {
                string md5Hash = (seed + uniqueHash).CreateMD5();
                pinBuilder.Append(Regex.Replace(md5Hash, @"\D+", string.Empty));
                seed = pinBuilder.ToString() + md5Hash;
            }
            return pinBuilder.ToString().Substring(0, length);
        }

        public static string CreateMD5(this string input)
        {
            using (var md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                StringBuilder hashBuilder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    hashBuilder.Append(b.ToString("X2"));
                }
                return hashBuilder.ToString();
            }
        }

        public static string CreatePassword(int length, int numberOfNonAlphanumericCharacters)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder passwordBuilder = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                passwordBuilder.Append(chars[random.Next(chars.Length)]);
            }
            return passwordBuilder.ToString();
        }

        public static string RandomString(int length, string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789")
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), "length cannot be less than zero.");

            if (string.IsNullOrEmpty(allowedChars))
                throw new ArgumentException("allowedChars may not be empty.");

            char[] charArray = new HashSet<char>(allowedChars).ToArray();
            if (charArray.Length > 256)
                throw new ArgumentException("allowedChars may contain no more than 256 characters.");

            StringBuilder randomStringBuilder = new StringBuilder();
            using (var cryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                byte[] randomBytes = new byte[128];
                while (randomStringBuilder.Length < length)
                {
                    cryptoServiceProvider.GetBytes(randomBytes);
                    foreach (byte b in randomBytes)
                    {
                        if (randomStringBuilder.Length >= length) break;

                        int index = b % charArray.Length;
                        randomStringBuilder.Append(charArray[index]);
                    }
                }
            }
            return randomStringBuilder.ToString();
        }
    }
}
