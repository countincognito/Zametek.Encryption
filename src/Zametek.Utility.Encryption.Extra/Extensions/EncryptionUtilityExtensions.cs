using System;
using System.Threading;
using System.Threading.Tasks;

namespace Zametek.Utility.Encryption
{
    public static class EncryptionUtilityExtensions
    {
        public static async Task<(SymmetricKeyDefinition, AsymmetricKeyDefinition)> CreateSymmetricKeyIdAsync(
            this IEncryptionUtility encryptionUtility,
            string symmetricKeyName,
            CancellationToken ct)
        {
            if (encryptionUtility is null)
            {
                throw new ArgumentNullException(nameof(encryptionUtility));
            }
            if (string.IsNullOrWhiteSpace(symmetricKeyName))
            {
                throw new ArgumentNullException(nameof(symmetricKeyName));
            }

            var createKeysRequest = new CreateKeysRequest
            {
                SymmetricKeyName = symmetricKeyName,
                AsymmetricKeyName = Guid.NewGuid().ToDashedString(),
            };

            CreateKeysResponse createKeyResponse = await encryptionUtility
                .CreateKeysAsync(createKeysRequest, ct)
                .ConfigureAwait(false);

            return (createKeyResponse.SymmetricKeyDefinition, createKeyResponse.AsymmetricKeyDefinition);
        }

        public static async Task<byte[]> EncryptObjectAsync<T>(
            this IEncryptionUtility encryptionUtility,
            T data,
            CancellationToken ct)
            where T : class
        {
            if (encryptionUtility is null)
            {
                throw new ArgumentNullException(nameof(encryptionUtility));
            }
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var encryptionContext = EncryptionContext.Current ?? throw new InvalidOperationException(Properties.Resources.Message_EncryptionContextIsNotSet);
            
            var encryptRequest = new EncryptRequest
            {
                SymmetricKeyId = encryptionContext.SymmetricKeyId,
                Data = data.ObjectToByteArray(),
            };

            EncryptResponse encryptResponse = await encryptionUtility
                .EncryptAsync(encryptRequest, ct)
                .ConfigureAwait(false);

            return encryptResponse.EncryptedData;
        }

        public static async Task<T> DecryptObjectAsync<T>(
            this IEncryptionUtility encryptionUtility,
            byte[] encryptedData,
            CancellationToken ct)
            where T : class
        {
            if (encryptionUtility is null)
            {
                throw new ArgumentNullException(nameof(encryptionUtility));
            }
            if (encryptedData is null)
            {
                throw new ArgumentNullException(nameof(encryptedData));
            }

            var encryptionContext = EncryptionContext.Current ?? throw new InvalidOperationException(Properties.Resources.Message_EncryptionContextIsNotSet);

            var decryptRequest = new DecryptRequest
            {
                SymmetricKeyId = encryptionContext.SymmetricKeyId,
                EncryptedData = encryptedData,
            };

            DecryptResponse decryptResponse = await encryptionUtility
                .DecryptAsync(decryptRequest, ct)
                .ConfigureAwait(false);

            return decryptResponse.Data.ByteArrayToObject<T>();
        }

        public static async Task<(SymmetricKeyDefinition, AsymmetricKeyDefinition)> RotateAsymmetricKeyAsync(
            this IEncryptionUtility encryptionUtility,
            CancellationToken ct)
        {
            if (encryptionUtility is null)
            {
                throw new ArgumentNullException(nameof(encryptionUtility));
            }

            var encryptionContext = EncryptionContext.Current ?? throw new InvalidOperationException(Properties.Resources.Message_EncryptionContextIsNotSet);

            var rotateAsymmetricKeyRequest = new RotateAsymmetricKeyRequest
            {
                SymmetricKeyId = encryptionContext.SymmetricKeyId,
            };

            RotateAsymmetricKeyResponse rotateAsymmetricKeyResponse = await encryptionUtility
                .RotateAsymmetricKeyAsync(rotateAsymmetricKeyRequest, ct)
                .ConfigureAwait(false);

            return (rotateAsymmetricKeyResponse.SymmetricKeyDefinition, rotateAsymmetricKeyResponse.AsymmetricKeyDefinition);
        }
    }
}
