using FluentValidation;
using System.Threading;
using System.Threading.Tasks;

namespace Zametek.Utility.Encryption
{
    public class RotateAsymmetricKeyRequestValidator
        : AbstractValidator<RotateAsymmetricKeyRequest>
    {
        private static readonly RotateAsymmetricKeyRequestValidator s_Instance = new RotateAsymmetricKeyRequestValidator();

        protected RotateAsymmetricKeyRequestValidator()
        {
            RuleFor(request => request).NotNull();
            RuleFor(request => request.SymmetricKeyId).NotEmpty();
        }

        public static async Task ValidateAndThrowAsync(
            RotateAsymmetricKeyRequest request,
            CancellationToken ct)
        {
            await s_Instance
                .ValidateAndThrowAsync(request, cancellationToken: ct)
                .ConfigureAwait(false);
        }
    }
}
