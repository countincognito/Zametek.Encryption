using FluentValidation;
using System.Threading;
using System.Threading.Tasks;

namespace Zametek.Utility.Encryption
{
    public class ViewSymmetricKeyDefinitionRequestValidator
        : AbstractValidator<ViewSymmetricKeyDefinitionRequest>
    {
        private static readonly ViewSymmetricKeyDefinitionRequestValidator s_Instance = new ViewSymmetricKeyDefinitionRequestValidator();

        protected ViewSymmetricKeyDefinitionRequestValidator()
        {
            RuleFor(request => request).NotNull();
            RuleFor(request => request.SymmetricKeyId).NotEmpty();
        }

        public static async Task ValidateAndThrowAsync(
            ViewSymmetricKeyDefinitionRequest request,
            CancellationToken ct)
        {
            await s_Instance
                .ValidateAndThrowAsync(request, cancellationToken: ct)
                .ConfigureAwait(false);
        }
    }
}
