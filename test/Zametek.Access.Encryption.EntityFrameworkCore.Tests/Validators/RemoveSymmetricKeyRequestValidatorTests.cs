using AutoFixture;
using FluentAssertions;
using FluentValidation;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Zametek.Access.Encryption.Tests
{
    public class RemoveSymmetricKeyRequestValidatorTests
    {
        [Fact]
        public async Task RemoveSymmetricKeyRequestValidator_GivenValidateAndThrowAsync_WhenNullRemoveRequest_ThenArgumentNullExceptionThrown()
        {
            Func<Task> act = async () => await RemoveSymmetricKeyRequestValidator
                .ValidateAndThrowAsync(null, default)
                .ConfigureAwait(false);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task RemoveSymmetricKeyRequestValidator_GivenValidateAndThrowAsync_WhenInvalidSymmetricKeyId_ThenValidationExceptionThrown()
        {
            var fixture = new Fixture();
            RemoveSymmetricKeyRequest removeRequest = fixture
                .Build<RemoveSymmetricKeyRequest>()
                .With(x => x.SymmetricKeyId, Guid.Empty)
                .Create();

            Func<Task> act = async () => await RemoveSymmetricKeyRequestValidator
                .ValidateAndThrowAsync(removeRequest, default)
                .ConfigureAwait(false);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task RemoveSymmetricKeyRequestValidator_GivenValidateAndThrowAsync_WhenInvalidAsymmetricKeyId_ThenValidationExceptionThrown()
        {
            var fixture = new Fixture();
            RemoveSymmetricKeyRequest removeRequest = fixture
                .Build<RemoveSymmetricKeyRequest>()
                .With(x => x.AsymmetricKeyId, string.Empty)
                .Create();

            Func<Task> act = async () => await RemoveSymmetricKeyRequestValidator
                .ValidateAndThrowAsync(removeRequest, default)
                .ConfigureAwait(false);

            await act.Should().ThrowAsync<ValidationException>();
        }
    }
}
