using System.Threading;
using System.Threading.Tasks;

namespace Zametek.Utility.Encryption
{
    public interface ISymmetricKeyEncryption
    {
        Task<byte[]> EncryptAsync(byte[] dataToEncrypt, byte[] symmetricKey, byte[] initializationVector, CancellationToken ct);

        Task<byte[]> DecryptAsync(byte[] dataToDecrypt, byte[] symmetricKey, byte[] initializationVector, CancellationToken ct);
    }
}
