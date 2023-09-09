using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;

namespace Zametek.Utility.Encryption
{
    /// <summary>
    /// Based on the work of Stephen Haunts
    /// https://github.com/stephenhaunts/AzureKeyVault
    /// https://github.com/stephenhaunts/SecureCodingWorkshop
    /// https://github.com/stephenhaunts/Building-Secure-Applications-with-Cryptography-in-.NET-Course-Source-Code
    /// and
    /// https://github.com/dotnet/aspnetcore/blob/master/src/Identity/Extensions.Core/src/PasswordHasher.cs
    /// https://docs.microsoft.com/en-us/dotnet/standard/security/cryptography-model
    /// </summary>
    public static class EncryptionHelper
    {
        #region Private Members

        private static byte[] GenerateRandomNumber(int byteArrayLength)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var byteArray = new byte[byteArrayLength];
                rng.GetBytes(byteArray);
                return byteArray;
            }
        }

        #endregion

        #region Public Members

        public static byte[] Generate128BitSalt() => GenerateRandomNumber(Constants.c_128_Bit_ByteArrayLength);

        public static byte[] Generate256BitSalt() => GenerateRandomNumber(Constants.c_256_Bit_ByteArrayLength);

        public static byte[] Generate256BitKey() => GenerateRandomNumber(Constants.c_256_Bit_ByteArrayLength);

        public static byte[] Generate512BitKey() => GenerateRandomNumber(Constants.c_512_Bit_ByteArrayLength);

        public static byte[] Generate1024BitKey() => GenerateRandomNumber(Constants.c_1024_Bit_ByteArrayLength);

        public static byte[] Generate2048BitKey() => GenerateRandomNumber(Constants.c_2048_Bit_ByteArrayLength);

        public static byte[] Generate4096BitKey() => GenerateRandomNumber(Constants.c_4096_Bit_ByteArrayLength);

        public static byte[] Generate128BitInitializationVector() => GenerateRandomNumber(Constants.c_128_Bit_ByteArrayLength);

        public static byte[] HashPassword(
            string password,
            byte[] salt,
            int iterations,
            int bytesToGenerate,
            byte[] key)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException(nameof(password));
            }
            if (salt is null)
            {
                throw new ArgumentNullException(nameof(salt));
            }
            if (salt.Length < Constants.c_256_Bit_ByteArrayLength)
            {
                // Reject if salt is less than 256-bit
                throw new ArgumentException(Properties.Resources.Message_SaltCannotBeLessThan256Bit, nameof(salt));
            }
            if (iterations < Constants.c_MinIterations)
            {
                // Reject if iterations is less than 500_000
                // https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html#pbkdf2
                throw new ArgumentException(Properties.Resources.Message_IterationsCannotBeLessThan500000, nameof(iterations));
            }
            if (bytesToGenerate < Constants.c_32ByteLength)
            {
                // Reject if bytesToGenerate is less than 32 bytes
                throw new ArgumentException(Properties.Resources.Message_BytesToGenerateCannotBeLessThan32Bytes, nameof(bytesToGenerate));
            }
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (key.Length < Constants.c_256_Bit_ByteArrayLength)
            {
                // Reject if key is less than 256-bit
                throw new ArgumentException(Properties.Resources.Message_KeyCannotBeLessThan256Bit, nameof(key));
            }

            byte[] Pbkdf2 = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA512, iterations, bytesToGenerate);
            using (var hmac = new HMACSHA512(key))
            {
                return hmac.ComputeHash(Pbkdf2);
            }
        }

        #endregion
    }
}
