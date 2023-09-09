using AutoFixture;
using FluentAssertions;
using FluentValidation;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Zametek.Access.Encryption.Tests
{
    public class ViewSymmetricKeyRequestValidatorTests
    {
        [Fact]
        public async Task ViewSymmetricKeyRequestValidator_GivenValidateAndThrowAsync_WhenNullViewRequest_ThenArgumentNullExceptionThrown()
        {
            Func<Task> act = async () => await ViewSymmetricKeyRequestValidator
                .ValidateAndThrowAsync(null, default)
                .ConfigureAwait(false);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task ViewSymmetricKeyRequestValidator_GivenValidateAndThrowAsync_WhenInvalidSymmetricKeyId_ThenValidationExceptionThrown()
        {
            var fixture = new Fixture();
            ViewSymmetricKeyRequest viewRequest = fixture
                .Build<ViewSymmetricKeyRequest>()
                .With(x => x.SymmetricKeyId, Guid.Empty)
                .Create();

            Func<Task> act = async () => await ViewSymmetricKeyRequestValidator
                .ValidateAndThrowAsync(viewRequest, default)
                .ConfigureAwait(false);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task ViewSymmetricKeyRequestValidator_GivenValidateAndThrowAsync_WhenInvalidAsymmetricKeyId_ThenValidationExceptionThrown()
        {
            var fixture = new Fixture();
            ViewSymmetricKeyRequest viewRequest = fixture
                .Build<ViewSymmetricKeyRequest>()
                .With(x => x.AsymmetricKeyId, string.Empty)
                .Create();

            Func<Task> act = async () => await ViewSymmetricKeyRequestValidator
                .ValidateAndThrowAsync(viewRequest, default)
                .ConfigureAwait(false);

            await act.Should().ThrowAsync<ValidationException>();
        }
    }
}
