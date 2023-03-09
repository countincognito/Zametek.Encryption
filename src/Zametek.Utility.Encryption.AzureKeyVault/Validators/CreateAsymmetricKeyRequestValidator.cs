using FluentValidation;
using System.Threading;
using System.Threading.Tasks;

namespace Zametek.Utility.Encryption
{
    public class CreateAsymmetricKeyRequestValidator
        : AbstractValidator<CreateAsymmetricKeyRequest>
    {
        private static readonly CreateAsymmetricKeyRequestValidator s_Instance = new CreateAsymmetricKeyRequestValidator();

        protected CreateAsymmetricKeyRequestValidator()
        {
            RuleFor(request => request).NotNull();
            RuleFor(request => request.Name).NotEmpty();
        }

        public static async Task ValidateAndThrowAsync(
            CreateAsymmetricKeyRequest request,
            CancellationToken ct)
        {
            await s_Instance
                .ValidateAndThrowAsync(request, cancellationToken: ct)
                .ConfigureAwait(false);
        }
    }
}
