using AutoFixture;
using FluentAssertions;
using FluentValidation;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Zametek.Utility.Encryption.Tests
{
    public class RemoveAsymmetricKeyRequestValidatorTests
    {
        [Fact]
        public async Task RemoveAsymmetricKeyRequestValidator_GivenValidateAndThrowAsync_WhenNullRemoveRequest_ThenArgumentNullExceptionThrown()
        {
            Func<Task> act = async () => await RemoveAsymmetricKeyRequestValidator
                .ValidateAndThrowAsync(null, default)
                .ConfigureAwait(false);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task RemoveAsymmetricKeyRequestValidator_GivenValidateAndThrowAsync_WhenInvalidName_ThenValidationExceptionThrown()
        {
            var fixture = new Fixture();
            RemoveAsymmetricKeyRequest removeRequest = fixture
                .Build<RemoveAsymmetricKeyRequest>()
                .With(x => x.Name, string.Empty)
                .Create();

            Func<Task> act = async () => await RemoveAsymmetricKeyRequestValidator
                .ValidateAndThrowAsync(removeRequest, default)
                .ConfigureAwait(false);

            await act.Should().ThrowAsync<ValidationException>();
        }

        //[Fact]
        //public void RemoveAsymmetricKeyRequestValidator_GivenValidateAndThrowAsync_WhenInvalidVersion_ThenValidationExceptionThrown()
        //{
        //    var fixture = new Fixture();
        //    RemoveAsymmetricKeyRequest removeRequest = fixture
        //        .Build<RemoveAsymmetricKeyRequest>()
        //        .With(x => x.Version, string.Empty)
        //        .Create();

        //    Func<Task> act = async () => await RemoveAsymmetricKeyRequestValidator
        //        .ValidateAndThrowAsync(removeRequest, default)
        //        .ConfigureAwait(false);

        //    act.Should().Throw<ValidationException>();
        //}
    }
}
