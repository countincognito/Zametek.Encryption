using FluentValidation;
using System.Threading;
using System.Threading.Tasks;

namespace Zametek.Utility.Encryption
{
    public class CreateKeysRequestValidator
        : AbstractValidator<CreateKeysRequest>
    {
        private static readonly CreateKeysRequestValidator s_Instance = new CreateKeysRequestValidator();

        protected CreateKeysRequestValidator()
        {
            RuleFor(request => request).NotNull();
            RuleFor(request => request.SymmetricKeyName).NotEmpty();
        }

        public static async Task ValidateAndThrowAsync(
            CreateKeysRequest request,
            CancellationToken ct)
        {
            await s_Instance
                .ValidateAndThrowAsync(request, cancellationToken: ct)
                .ConfigureAwait(false);
        }
    }
}
