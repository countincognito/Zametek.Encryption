using FluentValidation;
using System.Threading;
using System.Threading.Tasks;

namespace Zametek.Utility.Encryption
{
    public class EncryptRequestValidator
        : AbstractValidator<EncryptRequest>
    {
        private static readonly EncryptRequestValidator s_Instance = new EncryptRequestValidator();

        protected EncryptRequestValidator()
        {
            RuleFor(request => request).NotNull();
            RuleFor(request => request.SymmetricKeyId).NotEmpty();
            RuleFor(request => request.Data).NotNull();
        }

        public static async Task ValidateAndThrowAsync(
            EncryptRequest request,
            CancellationToken ct)
        {
            await s_Instance
                .ValidateAndThrowAsync(request, cancellationToken: ct)
                .ConfigureAwait(false);
        }
    }
}
