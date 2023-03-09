using AutoFixture;
using FluentAssertions;
using FluentValidation;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Zametek.Access.Encryption.Tests
{
    public class UpdateSymmetricKeyRequestValidatorTests
    {
        [Fact]
        public async Task UpdateSymmetricKeyRequestValidator_GivenValidateAndThrowAsync_WhenNullUpdateRequest_ThenArgumentNullExceptionThrown()
        {
            Func<Task> act = async () => await UpdateSymmetricKeyRequestValidator
                .ValidateAndThrowAsync(null, default)
                .ConfigureAwait(false);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task UpdateSymmetricKeyRequestValidator_GivenValidateAndThrowAsync_WhenInvalidSymmetricKeyId_ThenValidationExceptionThrown()
        {
            var fixture = new Fixture();
            UpdateSymmetricKeyRequest updateRequest = fixture
                .Build<UpdateSymmetricKeyRequest>()
                .With(x => x.SymmetricKeyId, Guid.Empty)
                .Create();

            Func<Task> act = async () => await UpdateSymmetricKeyRequestValidator
                .ValidateAndThrowAsync(updateRequest, default)
                .ConfigureAwait(false);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task UpdateSymmetricKeyRequestValidator_GivenValidateAndThrowAsync_WhenInvalidName_ThenValidationExceptionThrown()
        {
            var fixture = new Fixture();
            UpdateSymmetricKeyRequest updateRequest = fixture
                .Build<UpdateSymmetricKeyRequest>()
                .With(x => x.AsymmetricKeyName, string.Empty)
                .Create();

            Func<Task> act = async () => await UpdateSymmetricKeyRequestValidator
                .ValidateAndThrowAsync(updateRequest, default)
                .ConfigureAwait(false);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task UpdateSymmetricKeyRequestValidator_GivenValidateAndThrowAsync_WhenInvalidAsymmetricKeyId_ThenValidationExceptionThrown()
        {
            var fixture = new Fixture();
            UpdateSymmetricKeyRequest updateRequest = fixture
                .Build<UpdateSymmetricKeyRequest>()
                .With(x => x.AsymmetricKeyId, string.Empty)
                .Create();

            Func<Task> act = async () => await UpdateSymmetricKeyRequestValidator
                .ValidateAndThrowAsync(updateRequest, default)
                .ConfigureAwait(false);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task UpdateSymmetricKeyRequestValidator_GivenValidateAndThrowAsync_WhenInvalidAsymmetricKeyName_ThenValidationExceptionThrown()
        {
            var fixture = new Fixture();
            UpdateSymmetricKeyRequest updateRequest = fixture
                .Build<UpdateSymmetricKeyRequest>()
                .With(x => x.AsymmetricKeyName, string.Empty)
                .Create();

            Func<Task> act = async () => await UpdateSymmetricKeyRequestValidator
                .ValidateAndThrowAsync(updateRequest, default)
                .ConfigureAwait(false);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task UpdateSymmetricKeyRequestValidator_GivenValidateAndThrowAsync_WhenInvalidAsymmetricKeyVersion_ThenValidationExceptionThrown()
        {
            var fixture = new Fixture();
            UpdateSymmetricKeyRequest updateRequest = fixture
                .Build<UpdateSymmetricKeyRequest>()
                .With(x => x.AsymmetricKeyVersion, string.Empty)
                .Create();

            Func<Task> act = async () => await UpdateSymmetricKeyRequestValidator
                .ValidateAndThrowAsync(updateRequest, default)
                .ConfigureAwait(false);

            await act.Should().ThrowAsync<ValidationException>();
        }
    }
}
