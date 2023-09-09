using FluentValidation;

namespace Zametek.Access.Encryption
{
    public class UpdateSymmetricKeyRequestValidator
        : AbstractValidator<UpdateSymmetricKeyRequest>
    {
        private static readonly UpdateSymmetricKeyRequestValidator s_Instance = new();

        protected UpdateSymmetricKeyRequestValidator()
        {
            RuleFor(request => request).NotNull();
            RuleFor(request => request.SymmetricKeyId).NotEmpty();
            RuleFor(request => request.SymmetricKeyName).NotEmpty();
            RuleFor(request => request.AsymmetricKeyId).NotEmpty();
            RuleFor(request => request.AsymmetricKeyName).NotEmpty();
            RuleFor(request => request.AsymmetricKeyVersion).NotEmpty();
        }

        public static async Task ValidateAndThrowAsync(
            UpdateSymmetricKeyRequest request,
            CancellationToken ct)
        {
            await s_Instance
                .ValidateAndThrowAsync(request, cancellationToken: ct)
                .ConfigureAwait(false);
        }
    }
}
