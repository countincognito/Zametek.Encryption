using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Zametek.Utility.Logging;

namespace Zametek.Utility.Encryption
{
    /// <summary>
    /// Based on the work of Stephen Haunts
    /// https://github.com/stephenhaunts/AzureKeyVault
    /// https://github.com/stephenhaunts/SecureCodingWorkshop
    /// https://github.com/stephenhaunts/Building-Secure-Applications-with-Cryptography-in-.NET-Course-Source-Code
    /// and
    /// https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.aesmanaged?view=netstandard-2.0
    /// https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.aescryptoserviceprovider?view=netstandard-2.0
    /// https://stackoverflow.com/questions/45473884/what-is-the-difference-between-aes-and-aesmanaged
    /// https://docs.microsoft.com/en-us/dotnet/standard/security/cryptography-model
    /// </summary>
    [DiagnosticLogging(LogActive.Off)]
    public class AesEncryption
        : ISymmetricKeyEncryption
    {
        #region Private Members

        private static void CheckInputs(
            byte[] symmetricKey,
            byte[] initializationVector)
        {
            if (symmetricKey is null)
            {
                throw new ArgumentNullException(nameof(symmetricKey));
            }
            if (symmetricKey.Length != Constants.c_256_Bit_ByteArrayLength)
            {
                // Reject if key is not 256-bit
                throw new ArgumentException(Properties.Resources.Message_KeyMustBe256Bit, nameof(symmetricKey));
            }
            if (initializationVector is null)
            {
                throw new ArgumentNullException(nameof(initializationVector));
            }
            if (initializationVector.Length != Constants.c_128_Bit_ByteArrayLength)
            {
                // Reject if initialization vector is not 128-bit
                throw new ArgumentException(Properties.Resources.Message_InitializationVectorMustBe128Bit, nameof(initializationVector));
            }
        }

        #endregion

        #region ISymmetricKeyEncryption Members

        public async Task<byte[]> EncryptAsync(
            byte[] dataToEncrypt,
            byte[] symmetricKey,
            byte[] initializationVector,
            CancellationToken ct)
        {
            if (dataToEncrypt is null)
            {
                throw new ArgumentNullException(nameof(dataToEncrypt));
            }
            CheckInputs(symmetricKey, initializationVector);

            using (var aes = Aes.Create())
            {
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                aes.Key = symmetricKey;
                aes.IV = initializationVector;

                using (var memoryStream = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        await cryptoStream.WriteAsync(dataToEncrypt, 0, dataToEncrypt.Length, ct)
                            .ConfigureAwait(false);
                        cryptoStream.FlushFinalBlock();
                        return memoryStream.ToArray();
                    }
                }
            }
        }

        public async Task<byte[]> DecryptAsync(
            byte[] dataToDecrypt,
            byte[] symmetricKey,
            byte[] initializationVector,
            CancellationToken ct)
        {
            if (dataToDecrypt is null)
            {
                throw new ArgumentNullException(nameof(dataToDecrypt));
            }
            CheckInputs(symmetricKey, initializationVector);

            using (var aes = Aes.Create())
            {
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                aes.Key = symmetricKey;
                aes.IV = initializationVector;

                using (var memoryStream = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        await cryptoStream.WriteAsync(dataToDecrypt, 0, dataToDecrypt.Length, ct)
                            .ConfigureAwait(false);
                        cryptoStream.FlushFinalBlock();
                        var decryptBytes = memoryStream.ToArray();
                        return decryptBytes;
                    }
                }
            }
        }

        #endregion
    }
}
