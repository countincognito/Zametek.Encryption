using AutoFixture;
using FluentAssertions;
using FluentValidation;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Zametek.Utility.Encryption.Tests
{
    public class EnableAsymmetricKeyRequestValidatorTests
    {
        [Fact]
        public async Task EnableAsymmetricKeyRequestValidator_GivenValidateAndThrowAsync_WhenNullRemoveRequest_ThenArgumentNullExceptionThrown()
        {
            Func<Task> act = async () => await EnableAsymmetricKeyRequestValidator
                .ValidateAndThrowAsync(null, default)
                .ConfigureAwait(false);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task EnableAsymmetricKeyRequestValidator_GivenValidateAndThrowAsync_WhenInvalidName_ThenValidationExceptionThrown()
        {
            var fixture = new Fixture();
            EnableAsymmetricKeyRequest enableRequest = fixture
                .Build<EnableAsymmetricKeyRequest>()
                .With(x => x.Name, string.Empty)
                .Create();

            Func<Task> act = async () => await EnableAsymmetricKeyRequestValidator
                .ValidateAndThrowAsync(enableRequest, default)
                .ConfigureAwait(false);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task EnableAsymmetricKeyRequestValidator_GivenValidateAndThrowAsync_WhenInvalidVersion_ThenValidationExceptionThrown()
        {
            var fixture = new Fixture();
            EnableAsymmetricKeyRequest enableRequest = fixture
                .Build<EnableAsymmetricKeyRequest>()
                .With(x => x.Version, string.Empty)
                .Create();

            Func<Task> act = async () => await EnableAsymmetricKeyRequestValidator
                .ValidateAndThrowAsync(enableRequest, default)
                .ConfigureAwait(false);

            await act.Should().ThrowAsync<ValidationException>();
        }
    }
}
