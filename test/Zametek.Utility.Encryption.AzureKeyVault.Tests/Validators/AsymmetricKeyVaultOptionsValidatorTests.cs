using AutoFixture;
using FluentAssertions;
using FluentValidation;
using System;
using Xunit;

namespace Zametek.Utility.Encryption.Tests
{
    public class AsymmetricKeyVaultOptionsValidatorTests
    {
        [Fact]
        public void AsymmetricKeyVaultOptionsValidator_GivenValidateAndThrow_WhenNullAzureKeyVaultOptions_ThenArgumentNullExceptionThrown()
        {
            Action act = () => AsymmetricKeyVaultOptionsValidator.ValidateAndThrow(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AsymmetricKeyVaultOptionsValidator_GivenValidateAndThrow_WhenInvalidAppName_ThenValidationExceptionThrown()
        {
            var fixture = new Fixture();
            AzureKeyVaultOptions azureKeyVaultOptions = fixture
                .Build<AzureKeyVaultOptions>()
                .With(x => x.AppName, string.Empty)
                .Create();

            Action act = () => AsymmetricKeyVaultOptionsValidator.ValidateAndThrow(azureKeyVaultOptions);

            act.Should().Throw<ValidationException>();
        }

        [Fact]
        public void AsymmetricKeyVaultOptionsValidator_GivenValidateAndThrow_WhenInvalidVaultUrl_ThenValidationExceptionThrown()
        {
            var fixture = new Fixture();
            AzureKeyVaultOptions azureKeyVaultOptions = fixture
                .Build<AzureKeyVaultOptions>()
                .With(x => x.VaultUrl, (Uri)null)
                .Create();

            Action act = () => AsymmetricKeyVaultOptionsValidator.ValidateAndThrow(azureKeyVaultOptions);

            act.Should().Throw<ValidationException>();
        }

        [Fact]
        public void AsymmetricKeyVaultOptionsValidator_GivenValidateAndThrow_WhenInvalidClientId_ThenValidationExceptionThrown()
        {
            var fixture = new Fixture();
            AzureKeyVaultOptions azureKeyVaultOptions = fixture
                .Build<AzureKeyVaultOptions>()
                .With(x => x.ClientId, string.Empty)
                .Create();

            Action act = () => AsymmetricKeyVaultOptionsValidator.ValidateAndThrow(azureKeyVaultOptions);

            act.Should().Throw<ValidationException>();
        }

        [Fact]
        public void AsymmetricKeyVaultOptionsValidator_GivenValidateAndThrow_WhenInvalidClientSecret_ThenValidationExceptionThrown()
        {
            var fixture = new Fixture();
            AzureKeyVaultOptions azureKeyVaultOptions = fixture
                .Build<AzureKeyVaultOptions>()
                .With(x => x.ClientSecret, string.Empty)
                .Create();

            Action act = () => AsymmetricKeyVaultOptionsValidator.ValidateAndThrow(azureKeyVaultOptions);

            act.Should().Throw<ValidationException>();
        }

        [Fact]
        public void AsymmetricKeyVaultOptionsValidator_GivenValidateAndThrow_WhenInvalidTenantId_ThenValidationExceptionThrown()
        {
            var fixture = new Fixture();
            AzureKeyVaultOptions azureKeyVaultOptions = fixture
                .Build<AzureKeyVaultOptions>()
                .With(x => x.TenantId, string.Empty)
                .Create();

            Action act = () => AsymmetricKeyVaultOptionsValidator.ValidateAndThrow(azureKeyVaultOptions);

            act.Should().Throw<ValidationException>();
        }
    }
}
