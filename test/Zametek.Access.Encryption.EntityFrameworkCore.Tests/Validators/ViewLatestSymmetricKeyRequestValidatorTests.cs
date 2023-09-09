using AutoFixture;
using FluentAssertions;
using FluentValidation;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Zametek.Access.Encryption.Tests
{
    public class ViewLatestSymmetricKeyRequestValidatorTests
    {
        [Fact]
        public async Task ViewLatestSymmetricKeyRequestValidator_GivenValidateAndThrowAsync_WhenNullViewLatestRequest_ThenArgumentNullExceptionThrown()
        {
            Func<Task> act = async () => await ViewLatestSymmetricKeyRequestValidator
                .ValidateAndThrowAsync(null, default)
                .ConfigureAwait(false);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task ViewLatestSymmetricKeyRequestValidator_GivenValidateAndThrowAsync_WhenInvalidSymmetricKeyId_ThenValidationExceptionThrown()
        {
            var fixture = new Fixture();
            ViewLatestSymmetricKeyRequest viewLatestRequest = fixture
                .Build<ViewLatestSymmetricKeyRequest>()
                .With(x => x.SymmetricKeyId, Guid.Empty)
                .Create();

            Func<Task> act = async () => await ViewLatestSymmetricKeyRequestValidator
                .ValidateAndThrowAsync(viewLatestRequest, default)
                .ConfigureAwait(false);

            await act.Should().ThrowAsync<ValidationException>();
        }
    }
}
