using System.Threading;
using System.Threading.Tasks;

namespace Zametek.Utility.Encryption
{
    public interface IEncryptionUtility
    {
        Task<CreateKeysResponse> CreateKeysAsync(CreateKeysRequest request, CancellationToken ct);

        Task<EncryptResponse> EncryptAsync(EncryptRequest request, CancellationToken ct);

        Task<DecryptResponse> DecryptAsync(DecryptRequest request, CancellationToken ct);

        Task<RotateAsymmetricKeyResponse> RotateAsymmetricKeyAsync(RotateAsymmetricKeyRequest request, CancellationToken ct);

        Task<ViewSymmetricKeyDefinitionResponse> ViewSymmetricKeyDefinitionAsync(ViewSymmetricKeyDefinitionRequest request, CancellationToken ct);

        Task<ViewAsymmetricKeyDefinitionResponse> ViewAsymmetricKeyDefinitionAsync(ViewAsymmetricKeyDefinitionRequest request, CancellationToken ct);
    }
}
