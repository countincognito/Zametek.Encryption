using AutoFixture;
using FluentAssertions;
using FluentValidation;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Zametek.Utility.Encryption.Tests
{
    public class UnwrapSymmetricKeyRequestValidatorTests
    {
        [Fact]
        public async Task UnwrapSymmetricKeyRequestValidator_GivenValidateAndThrowAsync_WhenNullRemoveRequest_ThenArgumentNullExceptionThrown()
        {
            Func<Task> act = async () => await UnwrapSymmetricKeyRequestValidator
                .ValidateAndThrowAsync(null, default)
                .ConfigureAwait(false);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task UnwrapSymmetricKeyRequestValidator_GivenValidateAndThrowAsync_WhenInvalidName_ThenValidationExceptionThrown()
        {
            var fixture = new Fixture();
            UnwrapSymmetricKeyRequest unwrapRequest = fixture
                .Build<UnwrapSymmetricKeyRequest>()
                .With(x => x.AsymmetricKeyName, string.Empty)
                .Create();

            Func<Task> act = async () => await UnwrapSymmetricKeyRequestValidator
                .ValidateAndThrowAsync(unwrapRequest, default)
                .ConfigureAwait(false);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task UnwrapSymmetricKeyRequestValidator_GivenValidateAndThrowAsync_WhenInvalidVersion_ThenValidationExceptionThrown()
        {
            var fixture = new Fixture();
            UnwrapSymmetricKeyRequest unwrapRequest = fixture
                .Build<UnwrapSymmetricKeyRequest>()
                .With(x => x.AsymmetricKeyVersion, string.Empty)
                .Create();

            Func<Task> act = async () => await UnwrapSymmetricKeyRequestValidator
                .ValidateAndThrowAsync(unwrapRequest, default)
                .ConfigureAwait(false);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task UnwrapSymmetricKeyRequestValidator_GivenValidateAndThrowAsync_WhenEmptyWrappedSymmetricKey_ThenValidationExceptionThrown()
        {
            var fixture = new Fixture();
            UnwrapSymmetricKeyRequest unwrapRequest = fixture
                .Build<UnwrapSymmetricKeyRequest>()
                .With(x => x.WrappedSymmetricKey, new byte[] { })
                .Create();

            Func<Task> act = async () => await UnwrapSymmetricKeyRequestValidator
                .ValidateAndThrowAsync(unwrapRequest, default)
                .ConfigureAwait(false);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task UnwrapSymmetricKeyRequestValidator_GivenValidateAndThrowAsync_WhenNullWrappedSymmetricKey_ThenValidationExceptionThrown()
        {
            var fixture = new Fixture();
            UnwrapSymmetricKeyRequest unwrapRequest = fixture
                .Build<UnwrapSymmetricKeyRequest>()
                .With(x => x.WrappedSymmetricKey, (byte[])null)
                .Create();

            Func<Task> act = async () => await UnwrapSymmetricKeyRequestValidator
                .ValidateAndThrowAsync(unwrapRequest, default)
                .ConfigureAwait(false);

            await act.Should().ThrowAsync<ValidationException>();
        }
    }
}
