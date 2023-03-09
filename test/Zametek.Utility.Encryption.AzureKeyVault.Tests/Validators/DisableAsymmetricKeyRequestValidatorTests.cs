using AutoFixture;
using FluentAssertions;
using FluentValidation;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Zametek.Utility.Encryption.Tests
{
    public class DisableAsymmetricKeyRequestValidatorTests
    {
        [Fact]
        public async Task DisableAsymmetricKeyRequestValidator_GivenValidateAndThrowAsync_WhenNullRemoveRequest_ThenArgumentNullExceptionThrown()
        {
            Func<Task> act = async () => await DisableAsymmetricKeyRequestValidator
                .ValidateAndThrowAsync(null, default)
                .ConfigureAwait(false);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task DisableAsymmetricKeyRequestValidator_GivenValidateAndThrowAsync_WhenInvalidName_ThenValidationExceptionThrown()
        {
            var fixture = new Fixture();
            DisableAsymmetricKeyRequest disableRequest = fixture
                .Build<DisableAsymmetricKeyRequest>()
                .With(x => x.Name, string.Empty)
                .Create();

            Func<Task> act = async () => await DisableAsymmetricKeyRequestValidator
                .ValidateAndThrowAsync(disableRequest, default)
                .ConfigureAwait(false);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task DisableAsymmetricKeyRequestValidator_GivenValidateAndThrowAsync_WhenInvalidVersion_ThenValidationExceptionThrown()
        {
            var fixture = new Fixture();
            DisableAsymmetricKeyRequest disableRequest = fixture
                .Build<DisableAsymmetricKeyRequest>()
                .With(x => x.Version, string.Empty)
                .Create();

            Func<Task> act = async () => await DisableAsymmetricKeyRequestValidator
                .ValidateAndThrowAsync(disableRequest, default)
                .ConfigureAwait(false);

            await act.Should().ThrowAsync<ValidationException>();
        }
    }
}
