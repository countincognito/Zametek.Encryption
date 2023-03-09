using System.Threading;
using System.Threading.Tasks;

namespace Zametek.Access.Encryption
{
    public interface IEncryptionAccess
    {
        Task<RegisterSymmetricKeyResponse> RegisterSymmetricKeyAsync(RegisterSymmetricKeyRequest request, CancellationToken ct);

        Task<UpdateSymmetricKeyResponse> UpdateSymmetricKeyAsync(UpdateSymmetricKeyRequest request, CancellationToken ct);

        Task<ViewSymmetricKeyResponse> ViewSymmetricKeyAsync(ViewSymmetricKeyRequest request, CancellationToken ct);

        Task<ViewSymmetricKeyResponse> ViewLatestSymmetricKeyAsync(ViewLatestSymmetricKeyRequest request, CancellationToken ct);

        Task<bool> RemoveSymmetricKeyAsync(RemoveSymmetricKeyRequest request, CancellationToken ct);
    }
}
