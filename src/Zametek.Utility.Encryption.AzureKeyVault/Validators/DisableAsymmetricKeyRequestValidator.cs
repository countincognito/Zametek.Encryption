using FluentValidation;
using System.Threading;
using System.Threading.Tasks;

namespace Zametek.Utility.Encryption
{
    public class DisableAsymmetricKeyRequestValidator
        : AbstractValidator<DisableAsymmetricKeyRequest>
    {
        private static readonly DisableAsymmetricKeyRequestValidator s_Instance = new DisableAsymmetricKeyRequestValidator();

        protected DisableAsymmetricKeyRequestValidator()
        {
            RuleFor(request => request).NotNull();
            RuleFor(request => request.Name).NotEmpty();
            RuleFor(request => request.Version).NotEmpty();
        }

        public static async Task ValidateAndThrowAsync(
            DisableAsymmetricKeyRequest request,
            CancellationToken ct)
        {
            await s_Instance
                .ValidateAndThrowAsync(request, cancellationToken: ct)
                .ConfigureAwait(false);
        }
    }
}
