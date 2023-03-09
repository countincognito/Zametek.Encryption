using FluentValidation;
using System.Threading;
using System.Threading.Tasks;

namespace Zametek.Utility.Encryption
{
    public class UnwrapSymmetricKeyRequestValidator
        : AbstractValidator<UnwrapSymmetricKeyRequest>
    {
        private static readonly UnwrapSymmetricKeyRequestValidator s_Instance = new UnwrapSymmetricKeyRequestValidator();

        protected UnwrapSymmetricKeyRequestValidator()
        {
            RuleFor(request => request).NotNull();
            RuleFor(request => request.AsymmetricKeyName).NotEmpty();
            RuleFor(request => request.AsymmetricKeyVersion).NotEmpty();
            RuleFor(request => request.WrappedSymmetricKey).NotEmpty();
        }

        public static async Task ValidateAndThrowAsync(
            UnwrapSymmetricKeyRequest request,
            CancellationToken ct)
        {
            await s_Instance
                .ValidateAndThrowAsync(request, cancellationToken: ct)
                .ConfigureAwait(false);
        }
    }
}
