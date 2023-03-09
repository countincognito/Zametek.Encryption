using FluentValidation;
using System.Threading;
using System.Threading.Tasks;

namespace Zametek.Utility.Encryption
{
    public class WrapSymmetricKeyRequestValidator
        : AbstractValidator<WrapSymmetricKeyRequest>
    {
        private static readonly WrapSymmetricKeyRequestValidator s_Instance = new WrapSymmetricKeyRequestValidator();

        protected WrapSymmetricKeyRequestValidator()
        {
            RuleFor(request => request).NotNull();
            RuleFor(request => request.AsymmetricKeyName).NotEmpty();
            RuleFor(request => request.AsymmetricKeyVersion).NotEmpty();
            RuleFor(request => request.SymmetricKey).NotEmpty();
        }

        public static async Task ValidateAndThrowAsync(
            WrapSymmetricKeyRequest request,
            CancellationToken ct)
        {
            await s_Instance
                .ValidateAndThrowAsync(request, cancellationToken: ct)
                .ConfigureAwait(false);
        }
    }
}
