using AutoFixture;
using FluentAssertions;
using FluentValidation;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Zametek.Utility.Encryption.Tests
{
    public class WrapSymmetricKeyRequestValidatorTests
    {
        [Fact]
        public async Task WrapSymmetricKeyRequestValidator_GivenValidateAndThrowAsync_WhenNullRemoveRequest_ThenArgumentNullExceptionThrown()
        {
            Func<Task> act = async () => await WrapSymmetricKeyRequestValidator
                .ValidateAndThrowAsync(null, default)
                .ConfigureAwait(false);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task WrapSymmetricKeyRequestValidator_GivenValidateAndThrowAsync_WhenInvalidName_ThenValidationExceptionThrown()
        {
            var fixture = new Fixture();
            WrapSymmetricKeyRequest wrapRequest = fixture
                .Build<WrapSymmetricKeyRequest>()
                .With(x => x.AsymmetricKeyName, string.Empty)
                .Create();

            Func<Task> act = async () => await WrapSymmetricKeyRequestValidator
                .ValidateAndThrowAsync(wrapRequest, default)
                .ConfigureAwait(false);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task WrapSymmetricKeyRequestValidator_GivenValidateAndThrowAsync_WhenInvalidVersion_ThenValidationExceptionThrown()
        {
            var fixture = new Fixture();
            WrapSymmetricKeyRequest wrapRequest = fixture
                .Build<WrapSymmetricKeyRequest>()
                .With(x => x.AsymmetricKeyVersion, string.Empty)
                .Create();

            Func<Task> act = async () => await WrapSymmetricKeyRequestValidator
                .ValidateAndThrowAsync(wrapRequest, default)
                .ConfigureAwait(false);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task WrapSymmetricKeyRequestValidator_GivenValidateAndThrowAsync_WhenEmptySymmetricKey_ThenValidationExceptionThrown()
        {
            var fixture = new Fixture();
            WrapSymmetricKeyRequest wrapRequest = fixture
                .Build<WrapSymmetricKeyRequest>()
                .With(x => x.SymmetricKey, new byte[] { })
                .Create();

            Func<Task> act = async () => await WrapSymmetricKeyRequestValidator
                .ValidateAndThrowAsync(wrapRequest, default)
                .ConfigureAwait(false);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task WrapSymmetricKeyRequestValidator_GivenValidateAndThrowAsync_WhenNullSymmetricKey_ThenValidationExceptionThrown()
        {
            var fixture = new Fixture();
            WrapSymmetricKeyRequest wrapRequest = fixture
                .Build<WrapSymmetricKeyRequest>()
                .With(x => x.SymmetricKey, (byte[])null)
                .Create();

            Func<Task> act = async () => await WrapSymmetricKeyRequestValidator
                .ValidateAndThrowAsync(wrapRequest, default)
                .ConfigureAwait(false);

            await act.Should().ThrowAsync<ValidationException>();
        }
    }
}
