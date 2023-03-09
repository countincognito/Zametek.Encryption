using FluentValidation;
using System.Threading;
using System.Threading.Tasks;

namespace Zametek.Utility.Encryption
{
    public class ViewAsymmetricKeyDefinitionRequestValidator
        : AbstractValidator<ViewAsymmetricKeyDefinitionRequest>
    {
        private static readonly ViewAsymmetricKeyDefinitionRequestValidator s_Instance = new ViewAsymmetricKeyDefinitionRequestValidator();

        protected ViewAsymmetricKeyDefinitionRequestValidator()
        {
            RuleFor(request => request).NotNull();
            RuleFor(request => request.Name).NotEmpty();
            RuleFor(request => request.Version).NotEmpty();
        }

        public static async Task ValidateAndThrowAsync(
            ViewAsymmetricKeyDefinitionRequest request,
            CancellationToken ct)
        {
            await s_Instance
                .ValidateAndThrowAsync(request, cancellationToken: ct)
                .ConfigureAwait(false);
        }
    }
}
