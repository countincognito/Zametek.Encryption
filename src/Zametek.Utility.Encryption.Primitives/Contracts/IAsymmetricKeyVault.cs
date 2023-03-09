using System.Threading;
using System.Threading.Tasks;

namespace Zametek.Utility.Encryption
{
    public interface IAsymmetricKeyVault
    {
        Task<CreateAsymmetricKeyResponse> CreateAsymmetricKeyAsync(CreateAsymmetricKeyRequest request, CancellationToken ct);

        Task<DisableAsymmetricKeyResponse> DisableAsymmetricKeyAsync(DisableAsymmetricKeyRequest request, CancellationToken ct);

        Task<EnableAsymmetricKeyResponse> EnableAsymmetricKeyAsync(EnableAsymmetricKeyRequest request, CancellationToken ct);

        Task<bool> RemoveAsymmetricKeyAsync(RemoveAsymmetricKeyRequest request, CancellationToken ct);

        Task<WrapSymmetricKeyResponse> WrapSymmetricKeyAsync(WrapSymmetricKeyRequest request, CancellationToken ct);

        Task<UnwrapSymmetricKeyResponse> UnwrapSymmetricKeyAsync(UnwrapSymmetricKeyRequest request, CancellationToken ct);

        Task<ViewAsymmetricKeyDefinitionResponse> ViewAsymmetricKeyDefinitionAsync(ViewAsymmetricKeyDefinitionRequest request, CancellationToken ct);
    }
}
