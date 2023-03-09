using AutoFixture;
using FluentAssertions;
using FluentValidation;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Zametek.Utility.Encryption.Tests
{
    public class CreateAsymmetricKeyRequestValidatorTests
    {
        [Fact]
        public async Task CreateAsymmetricKeyRequestValidator_GivenValidateAndThrowAsync_WhenNullRemoveRequest_ThenArgumentNullExceptionThrown()
        {
            Func<Task> act = async () => await CreateAsymmetricKeyRequestValidator
                .ValidateAndThrowAsync(null, default)
                .ConfigureAwait(false);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task CreateAsymmetricKeyRequestValidator_GivenValidateAndThrowAsync_WhenInvalidName_ThenValidationExceptionThrown()
        {
            var fixture = new Fixture();
            CreateAsymmetricKeyRequest createRequest = fixture
                .Build<CreateAsymmetricKeyRequest>()
                .With(x => x.Name, string.Empty)
                .Create();

            Func<Task> act = async () => await CreateAsymmetricKeyRequestValidator
                .ValidateAndThrowAsync(createRequest, default)
                .ConfigureAwait(false);

            await act.Should().ThrowAsync<ValidationException>();
        }
    }
}
