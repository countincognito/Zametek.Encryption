using FluentValidation;
using System.Threading;
using System.Threading.Tasks;

namespace Zametek.Access.Encryption
{
    public class ViewSymmetricKeyRequestValidator
        : AbstractValidator<ViewSymmetricKeyRequest>
    {
        private static readonly ViewSymmetricKeyRequestValidator s_Instance = new ViewSymmetricKeyRequestValidator();

        protected ViewSymmetricKeyRequestValidator()
        {
            RuleFor(request => request).NotNull();
            RuleFor(request => request.SymmetricKeyId).NotEmpty();
            RuleFor(request => request.AsymmetricKeyId).NotEmpty();
        }

        public static async Task ValidateAndThrowAsync(
            ViewSymmetricKeyRequest request,
            CancellationToken ct)
        {
            await s_Instance
                .ValidateAndThrowAsync(request, cancellationToken: ct)
                .ConfigureAwait(false);
        }
    }
}
