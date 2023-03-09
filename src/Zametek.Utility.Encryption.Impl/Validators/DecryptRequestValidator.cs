using FluentValidation;
using System.Threading;
using System.Threading.Tasks;

namespace Zametek.Utility.Encryption
{
    public class DecryptRequestValidator
        : AbstractValidator<DecryptRequest>
    {
        private static readonly DecryptRequestValidator s_Instance = new DecryptRequestValidator();

        protected DecryptRequestValidator()
        {
            RuleFor(request => request).NotNull();
            RuleFor(request => request.SymmetricKeyId).NotEmpty();
            RuleFor(request => request.EncryptedData).NotNull();
        }

        public static async Task ValidateAndThrowAsync(
            DecryptRequest request,
            CancellationToken ct)
        {
            await s_Instance
                .ValidateAndThrowAsync(request, cancellationToken: ct)
                .ConfigureAwait(false);
        }
    }
}
