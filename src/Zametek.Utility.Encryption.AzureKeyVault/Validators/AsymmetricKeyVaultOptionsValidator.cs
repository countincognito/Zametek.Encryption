using FluentValidation;

namespace Zametek.Utility.Encryption
{
    public class AsymmetricKeyVaultOptionsValidator
        : AbstractValidator<AzureKeyVaultOptions>
    {
        private static readonly AsymmetricKeyVaultOptionsValidator s_Instance = new AsymmetricKeyVaultOptionsValidator();

        protected AsymmetricKeyVaultOptionsValidator()
        {
            RuleFor(request => request).NotNull();
            RuleFor(request => request.AppName).NotEmpty();
            RuleFor(request => request.VaultUrl).NotEmpty();
            RuleFor(request => request.ClientId).NotEmpty();
            RuleFor(request => request.ClientSecret).NotEmpty();
            RuleFor(request => request.TenantId).NotEmpty();
        }

        public static void ValidateAndThrow(AzureKeyVaultOptions request)
        {
            s_Instance.ValidateAndThrow(request);
        }
    }
}
