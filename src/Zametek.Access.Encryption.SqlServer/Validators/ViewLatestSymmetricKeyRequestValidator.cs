using FluentValidation;
using System.Threading;
using System.Threading.Tasks;

namespace Zametek.Access.Encryption
{
    public class ViewLatestSymmetricKeyRequestValidator
        : AbstractValidator<ViewLatestSymmetricKeyRequest>
    {
        private static readonly ViewLatestSymmetricKeyRequestValidator s_Instance = new ViewLatestSymmetricKeyRequestValidator();

        protected ViewLatestSymmetricKeyRequestValidator()
        {
            RuleFor(request => request).NotNull();
            RuleFor(request => request.SymmetricKeyId).NotEmpty();
        }

        public static async Task ValidateAndThrowAsync(
            ViewLatestSymmetricKeyRequest request,
            CancellationToken ct)
        {
            await s_Instance
                .ValidateAndThrowAsync(request, cancellationToken: ct)
                .ConfigureAwait(false);
        }
    }
}
