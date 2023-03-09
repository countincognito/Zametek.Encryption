using AutoFixture;
using FluentAssertions;
using FluentValidation;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Zametek.Utility.Encryption.Tests
{
    public class ViewAsymmetricKeyDefinitionRequestValidatorTests
    {
        [Fact]
        public async Task ViewAsymmetricKeyDefinitionRequestValidator_GivenValidateAndThrowAsync_WhenNullRemoveRequest_ThenArgumentNullExceptionThrown()
        {
            Func<Task> act = async () => await ViewAsymmetricKeyDefinitionRequestValidator
                .ValidateAndThrowAsync(null, default)
                .ConfigureAwait(false);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task ViewAsymmetricKeyDefinitionRequestValidator_GivenValidateAndThrowAsync_WhenInvalidName_ThenValidationExceptionThrown()
        {
            var fixture = new Fixture();
            ViewAsymmetricKeyDefinitionRequest viewRequest = fixture
                .Build<ViewAsymmetricKeyDefinitionRequest>()
                .With(x => x.Name, string.Empty)
                .Create();

            Func<Task> act = async () => await ViewAsymmetricKeyDefinitionRequestValidator
                .ValidateAndThrowAsync(viewRequest, default)
                .ConfigureAwait(false);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task ViewAsymmetricKeyDefinitionRequestValidator_GivenValidateAndThrowAsync_WhenInvalidVersion_ThenValidationExceptionThrown()
        {
            var fixture = new Fixture();
            ViewAsymmetricKeyDefinitionRequest viewRequest = fixture
                .Build<ViewAsymmetricKeyDefinitionRequest>()
                .With(x => x.Version, string.Empty)
                .Create();

            Func<Task> act = async () => await ViewAsymmetricKeyDefinitionRequestValidator
                .ValidateAndThrowAsync(viewRequest, default)
                .ConfigureAwait(false);

            await act.Should().ThrowAsync<ValidationException>();
        }
    }
}
