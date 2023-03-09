using FluentValidation;
using System.Threading;
using System.Threading.Tasks;

namespace Zametek.Utility.Encryption
{
    public class RemoveAsymmetricKeyRequestValidator
        : AbstractValidator<RemoveAsymmetricKeyRequest>
    {
        private static readonly RemoveAsymmetricKeyRequestValidator s_Instance = new RemoveAsymmetricKeyRequestValidator();

        protected RemoveAsymmetricKeyRequestValidator()
        {
            RuleFor(request => request).NotNull();
            RuleFor(request => request.Name).NotEmpty();
            //RuleFor(request => request.Version).NotEmpty();
        }

        public static async Task ValidateAndThrowAsync(
            RemoveAsymmetricKeyRequest request,
            CancellationToken ct)
        {
            await s_Instance
                .ValidateAndThrowAsync(request, cancellationToken: ct)
                .ConfigureAwait(false);
        }
    }
}
