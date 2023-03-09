using FluentValidation;
using System.Threading;
using System.Threading.Tasks;

namespace Zametek.Utility.Encryption
{
    public class EnableAsymmetricKeyRequestValidator
        : AbstractValidator<EnableAsymmetricKeyRequest>
    {
        private static readonly EnableAsymmetricKeyRequestValidator s_Instance = new EnableAsymmetricKeyRequestValidator();

        protected EnableAsymmetricKeyRequestValidator()
        {
            RuleFor(request => request).NotNull();
            RuleFor(request => request.Name).NotEmpty();
            RuleFor(request => request.Version).NotEmpty();
        }

        public static async Task ValidateAndThrowAsync(
            EnableAsymmetricKeyRequest request,
            CancellationToken ct)
        {
            await s_Instance
                .ValidateAndThrowAsync(request, cancellationToken: ct)
                .ConfigureAwait(false);
        }
    }
}
