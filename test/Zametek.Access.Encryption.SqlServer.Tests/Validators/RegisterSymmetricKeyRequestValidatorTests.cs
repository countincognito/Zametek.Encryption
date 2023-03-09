using AutoFixture;
using FluentAssertions;
using FluentValidation;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Zametek.Access.Encryption.Tests
{
    public class RegisterSymmetricKeyRequestValidatorTests
    {
        [Fact]
        public async Task RegisterSymmetricKeyRequestValidator_GivenValidateAndThrowAsync_WhenNullRegisterRequest_ThenArgumentNullExceptionThrown()
        {
            Func<Task> act = async () => await RegisterSymmetricKeyRequestValidator
                .ValidateAndThrowAsync(null, default)
                .ConfigureAwait(false);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task RegisterSymmetricKeyRequestValidator_GivenValidateAndThrowAsync_WhenInvalidSymmetricKeyId_ThenValidationExceptionThrown()
        {
            var fixture = new Fixture();
            RegisterSymmetricKeyRequest registerRequest = fixture
                .Build<RegisterSymmetricKeyRequest>()
                .With(x => x.SymmetricKeyId, Guid.Empty)
                .Create();

            Func<Task> act = async () => await RegisterSymmetricKeyRequestValidator
                .ValidateAndThrowAsync(registerRequest, default)
                .ConfigureAwait(false);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task RegisterSymmetricKeyRequestValidator_GivenValidateAndThrowAsync_WhenInvalidName_ThenValidationExceptionThrown()
        {
            var fixture = new Fixture();
            RegisterSymmetricKeyRequest registerRequest = fixture
                .Build<RegisterSymmetricKeyRequest>()
                .With(x => x.SymmetricKeyName, string.Empty)
                .Create();

            Func<Task> act = async () => await RegisterSymmetricKeyRequestValidator
                .ValidateAndThrowAsync(registerRequest, default)
                .ConfigureAwait(false);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task RegisterSymmetricKeyRequestValidator_GivenValidateAndThrowAsync_WhenInvalidAsymmetricKeyId_ThenValidationExceptionThrown()
        {
            var fixture = new Fixture();
            RegisterSymmetricKeyRequest registerRequest = fixture
                .Build<RegisterSymmetricKeyRequest>()
                .With(x => x.AsymmetricKeyId, string.Empty)
                .Create();

            Func<Task> act = async () => await RegisterSymmetricKeyRequestValidator
                .ValidateAndThrowAsync(registerRequest, default)
                .ConfigureAwait(false);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task RegisterSymmetricKeyRequestValidator_GivenValidateAndThrowAsync_WhenInvalidAsymmetricKeyName_ThenValidationExceptionThrown()
        {
            var fixture = new Fixture();
            RegisterSymmetricKeyRequest registerRequest = fixture
                .Build<RegisterSymmetricKeyRequest>()
                .With(x => x.AsymmetricKeyName, string.Empty)
                .Create();

            Func<Task> act = async () => await RegisterSymmetricKeyRequestValidator
                .ValidateAndThrowAsync(registerRequest, default)
                .ConfigureAwait(false);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task RegisterSymmetricKeyRequestValidator_GivenValidateAndThrowAsync_WhenInvalidAsymmetricKeyVersion_ThenValidationExceptionThrown()
        {
            var fixture = new Fixture();
            RegisterSymmetricKeyRequest registerRequest = fixture
                .Build<RegisterSymmetricKeyRequest>()
                .With(x => x.AsymmetricKeyVersion, string.Empty)
                .Create();

            Func<Task> act = async () => await RegisterSymmetricKeyRequestValidator
                .ValidateAndThrowAsync(registerRequest, default)
                .ConfigureAwait(false);

            await act.Should().ThrowAsync<ValidationException>();
        }
    }
}
