using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Zametek.Utility.Encryption.Tests
{
    public class AzureKeyVaultTests
    {
        private readonly AzureKeyVaultFixture m_AzureKeyVaultFixture;

        public AzureKeyVaultTests()
        {
            m_AzureKeyVaultFixture = new AzureKeyVaultFixture();
        }

        [Fact]
        public async Task AzureKeyVault_GivenCreateAsymmetricKeyAsync_WhenValidInputs_ThenKeyCreated()
        {
            IAsymmetricKeyVault azureKeyVault = m_AzureKeyVaultFixture.ServerServices.GetService<IAsymmetricKeyVault>();

            var fixture = new Fixture();
            CreateAsymmetricKeyRequest createRequest = fixture
                .Build<CreateAsymmetricKeyRequest>()
                .Create();

            CreateAsymmetricKeyResponse createResponse = await azureKeyVault.CreateAsymmetricKeyAsync(createRequest, default);

            createResponse.Should().NotBeNull();
            createResponse.AsymmetricKeyDefinition.Should().NotBeNull();
            createResponse.AsymmetricKeyDefinition.Id.Should().NotBeNullOrEmpty();
            createResponse.AsymmetricKeyDefinition.Name.Should().Be(createRequest.Name);
            createResponse.AsymmetricKeyDefinition.Version.Should().NotBeNullOrEmpty();
            createResponse.AsymmetricKeyDefinition.IsEnabled.Should().BeTrue();
            createResponse.AsymmetricKeyDefinition.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));

            RemoveAsymmetricKeyRequest removeRequest = fixture
                  .Build<RemoveAsymmetricKeyRequest>()
                  .With(x => x.Name, createRequest.Name)
                  .With(x => x.AwaitCompletion, false)
                  .Create();

            await azureKeyVault.RemoveAsymmetricKeyAsync(removeRequest, default);
        }

        [Fact]
        public async Task AzureKeyVault_GivenCreateAsymmetricKeyAsync_WhenSameRequestSentTwice_ThenNewVersionReturned()
        {
            IAsymmetricKeyVault azureKeyVault = m_AzureKeyVaultFixture.ServerServices.GetService<IAsymmetricKeyVault>();

            var fixture = new Fixture();
            CreateAsymmetricKeyRequest createRequest = fixture
                .Build<CreateAsymmetricKeyRequest>()
                .Create();

            CreateAsymmetricKeyResponse createResponse1 = await azureKeyVault.CreateAsymmetricKeyAsync(createRequest, default);
            CreateAsymmetricKeyResponse createResponse2 = await azureKeyVault.CreateAsymmetricKeyAsync(createRequest, default);

            createResponse1.Should().NotBeNull();
            createResponse1.AsymmetricKeyDefinition.Should().NotBeNull();
            createResponse1.AsymmetricKeyDefinition.Id.Should().NotBeNullOrEmpty();
            createResponse1.AsymmetricKeyDefinition.Name.Should().Be(createRequest.Name);
            createResponse1.AsymmetricKeyDefinition.Version.Should().NotBeNullOrEmpty();
            createResponse1.AsymmetricKeyDefinition.IsEnabled.Should().BeTrue();
            createResponse1.AsymmetricKeyDefinition.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));

            createResponse2.Should().NotBeNull();
            createResponse2.AsymmetricKeyDefinition.Should().NotBeNull();
            createResponse2.AsymmetricKeyDefinition.Id.Should().NotBeNullOrEmpty();
            createResponse2.AsymmetricKeyDefinition.Name.Should().Be(createRequest.Name);
            createResponse2.AsymmetricKeyDefinition.Version.Should().NotBeNullOrEmpty();
            createResponse2.AsymmetricKeyDefinition.IsEnabled.Should().BeTrue();
            createResponse2.AsymmetricKeyDefinition.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));

            createResponse1.AsymmetricKeyDefinition.Id.Should().NotBe(createResponse2.AsymmetricKeyDefinition.Id);
            createResponse1.AsymmetricKeyDefinition.Name.Should().Be(createResponse2.AsymmetricKeyDefinition.Name);
            createResponse1.AsymmetricKeyDefinition.Version.Should().NotBe(createResponse2.AsymmetricKeyDefinition.Version);

            RemoveAsymmetricKeyRequest removeRequest = fixture
                  .Build<RemoveAsymmetricKeyRequest>()
                  .With(x => x.Name, createRequest.Name)
                  .With(x => x.AwaitCompletion, false)
                  .Create();

            await azureKeyVault.RemoveAsymmetricKeyAsync(removeRequest, default);
        }

        [Fact]
        public async Task AzureKeyVault_GivenDisableAsymmetricKeyAsync_WhenValidInputs_ThenKeyDisabled()
        {
            IAsymmetricKeyVault azureKeyVault = m_AzureKeyVaultFixture.ServerServices.GetService<IAsymmetricKeyVault>();

            var fixture = new Fixture();
            CreateAsymmetricKeyRequest createRequest = fixture
                .Build<CreateAsymmetricKeyRequest>()
                .Create();

            CreateAsymmetricKeyResponse createResponse = await azureKeyVault.CreateAsymmetricKeyAsync(createRequest, default);

            createResponse.Should().NotBeNull();
            createResponse.AsymmetricKeyDefinition.Should().NotBeNull();
            createResponse.AsymmetricKeyDefinition.Id.Should().NotBeNullOrEmpty();
            createResponse.AsymmetricKeyDefinition.Name.Should().Be(createRequest.Name);
            createResponse.AsymmetricKeyDefinition.Version.Should().NotBeNullOrEmpty();
            createResponse.AsymmetricKeyDefinition.IsEnabled.Should().BeTrue();
            createResponse.AsymmetricKeyDefinition.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));

            DisableAsymmetricKeyRequest disableRequest = fixture
                .Build<DisableAsymmetricKeyRequest>()
                .With(x => x.Name, createResponse.AsymmetricKeyDefinition.Name)
                .With(x => x.Version, createResponse.AsymmetricKeyDefinition.Version)
                .Create();

            DisableAsymmetricKeyResponse disableResponse = await azureKeyVault.DisableAsymmetricKeyAsync(disableRequest, default);

            disableResponse.Should().NotBeNull();
            disableResponse.AsymmetricKeyDefinition.Should().NotBeNull();
            disableResponse.AsymmetricKeyDefinition.Id.Should().Be(createResponse.AsymmetricKeyDefinition.Id);
            disableResponse.AsymmetricKeyDefinition.Name.Should().Be(createResponse.AsymmetricKeyDefinition.Name);
            disableResponse.AsymmetricKeyDefinition.Version.Should().Be(createResponse.AsymmetricKeyDefinition.Version);
            disableResponse.AsymmetricKeyDefinition.IsEnabled.Should().BeFalse();
            disableResponse.AsymmetricKeyDefinition.CreatedAt.Should().Be(createResponse.AsymmetricKeyDefinition.CreatedAt);

            RemoveAsymmetricKeyRequest removeRequest = fixture
                  .Build<RemoveAsymmetricKeyRequest>()
                  .With(x => x.Name, createRequest.Name)
                  .With(x => x.AwaitCompletion, false)
                  .Create();

            await azureKeyVault.RemoveAsymmetricKeyAsync(removeRequest, default);
        }

        [Fact]
        public async Task AzureKeyVault_GivenDisableAsymmetricKeyAsync_WhenValidInputsSentTwice_ThenKeyDisabled()
        {
            IAsymmetricKeyVault azureKeyVault = m_AzureKeyVaultFixture.ServerServices.GetService<IAsymmetricKeyVault>();

            var fixture = new Fixture();
            CreateAsymmetricKeyRequest createRequest = fixture
                .Build<CreateAsymmetricKeyRequest>()
                .Create();

            CreateAsymmetricKeyResponse createResponse = await azureKeyVault.CreateAsymmetricKeyAsync(createRequest, default);

            createResponse.Should().NotBeNull();
            createResponse.AsymmetricKeyDefinition.Should().NotBeNull();
            createResponse.AsymmetricKeyDefinition.Id.Should().NotBeNullOrEmpty();
            createResponse.AsymmetricKeyDefinition.Name.Should().Be(createRequest.Name);
            createResponse.AsymmetricKeyDefinition.Version.Should().NotBeNullOrEmpty();
            createResponse.AsymmetricKeyDefinition.IsEnabled.Should().BeTrue();
            createResponse.AsymmetricKeyDefinition.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));

            DisableAsymmetricKeyRequest disableRequest = fixture
                .Build<DisableAsymmetricKeyRequest>()
                .With(x => x.Name, createResponse.AsymmetricKeyDefinition.Name)
                .With(x => x.Version, createResponse.AsymmetricKeyDefinition.Version)
                .Create();

            DisableAsymmetricKeyResponse disableResponse1 = await azureKeyVault.DisableAsymmetricKeyAsync(disableRequest, default);
            DisableAsymmetricKeyResponse disableResponse2 = await azureKeyVault.DisableAsymmetricKeyAsync(disableRequest, default);

            disableResponse1.Should().NotBeNull();
            disableResponse2.Should().NotBeNull();

            disableResponse1.Should().BeEquivalentTo(disableResponse2);

            RemoveAsymmetricKeyRequest removeRequest = fixture
                  .Build<RemoveAsymmetricKeyRequest>()
                  .With(x => x.Name, createRequest.Name)
                  .With(x => x.AwaitCompletion, false)
                  .Create();

            await azureKeyVault.RemoveAsymmetricKeyAsync(removeRequest, default);
        }

        [Fact]
        public async Task AzureKeyVault_GivenDisableAsymmetricKeyAsync_WhenInvalidName_ThenExceptionThrown()
        {
            IAsymmetricKeyVault azureKeyVault = m_AzureKeyVaultFixture.ServerServices.GetService<IAsymmetricKeyVault>();

            var fixture = new Fixture();
            CreateAsymmetricKeyRequest createRequest = fixture
                .Build<CreateAsymmetricKeyRequest>()
                .Create();

            CreateAsymmetricKeyResponse createResponse = await azureKeyVault.CreateAsymmetricKeyAsync(createRequest, default);

            createResponse.Should().NotBeNull();
            createResponse.AsymmetricKeyDefinition.Should().NotBeNull();
            createResponse.AsymmetricKeyDefinition.Id.Should().NotBeNullOrEmpty();
            createResponse.AsymmetricKeyDefinition.Name.Should().Be(createRequest.Name);
            createResponse.AsymmetricKeyDefinition.Version.Should().NotBeNullOrEmpty();
            createResponse.AsymmetricKeyDefinition.IsEnabled.Should().BeTrue();
            createResponse.AsymmetricKeyDefinition.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));

            // Replace name.

            AsymmetricKeyDefinition key = createResponse.AsymmetricKeyDefinition;

            DisableAsymmetricKeyRequest disableRequest = fixture
                .Build<DisableAsymmetricKeyRequest>()
                .With(x => x.Name, Guid.NewGuid().ToDashedString())
                .With(x => x.Version, key.Version)
                .Create();

            Func<Task> act = async () => await azureKeyVault.DisableAsymmetricKeyAsync(disableRequest, default);

            await act.Should().ThrowAsync<Exception>();

            RemoveAsymmetricKeyRequest removeRequest = fixture
                  .Build<RemoveAsymmetricKeyRequest>()
                  .With(x => x.Name, createRequest.Name)
                  .With(x => x.AwaitCompletion, false)
                  .Create();

            await azureKeyVault.RemoveAsymmetricKeyAsync(removeRequest, default);
        }

        [Fact]
        public async Task AzureKeyVault_GivenDisableAsymmetricKeyAsync_WhenInvalidVersion_ThenExceptionThrown()
        {
            IAsymmetricKeyVault azureKeyVault = m_AzureKeyVaultFixture.ServerServices.GetService<IAsymmetricKeyVault>();

            var fixture = new Fixture();
            CreateAsymmetricKeyRequest createRequest = fixture
                .Build<CreateAsymmetricKeyRequest>()
                .Create();

            CreateAsymmetricKeyResponse createResponse = await azureKeyVault.CreateAsymmetricKeyAsync(createRequest, default);

            createResponse.Should().NotBeNull();
            createResponse.AsymmetricKeyDefinition.Should().NotBeNull();
            createResponse.AsymmetricKeyDefinition.Id.Should().NotBeNullOrEmpty();
            createResponse.AsymmetricKeyDefinition.Name.Should().Be(createRequest.Name);
            createResponse.AsymmetricKeyDefinition.Version.Should().NotBeNullOrEmpty();
            createResponse.AsymmetricKeyDefinition.IsEnabled.Should().BeTrue();
            createResponse.AsymmetricKeyDefinition.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));

            // Replace version.

            AsymmetricKeyDefinition key = createResponse.AsymmetricKeyDefinition;

            DisableAsymmetricKeyRequest disableRequest = fixture
                .Build<DisableAsymmetricKeyRequest>()
                .With(x => x.Name, key.Name)
                .With(x => x.Version, Guid.NewGuid().ToFlatString())
                .Create();

            Func<Task> act = async () => await azureKeyVault.DisableAsymmetricKeyAsync(disableRequest, default);

            await act.Should().ThrowAsync<Exception>();

            RemoveAsymmetricKeyRequest removeRequest = fixture
                  .Build<RemoveAsymmetricKeyRequest>()
                  .With(x => x.Name, createRequest.Name)
                  .With(x => x.AwaitCompletion, false)
                  .Create();

            await azureKeyVault.RemoveAsymmetricKeyAsync(removeRequest, default);
        }

        [Fact]
        public async Task AzureKeyVault_GivenDisableAsymmetricKeyAsync_WhenNoKeyExists_ThenExceptionThrown()
        {
            IAsymmetricKeyVault azureKeyVault = m_AzureKeyVaultFixture.ServerServices.GetService<IAsymmetricKeyVault>();

            var fixture = new Fixture();
            DisableAsymmetricKeyRequest disableRequest = fixture
                .Build<DisableAsymmetricKeyRequest>()
                .With(x => x.Name, Guid.NewGuid().ToDashedString())
                .With(x => x.Version, Guid.NewGuid().ToFlatString())
                .Create();

            Func<Task> act = async () => await azureKeyVault.DisableAsymmetricKeyAsync(disableRequest, default);

            await act.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task AzureKeyVault_GivenEnableAsymmetricKeyAsync_WhenValidInputs_ThenKeyEnabled()
        {
            IAsymmetricKeyVault azureKeyVault = m_AzureKeyVaultFixture.ServerServices.GetService<IAsymmetricKeyVault>();

            var fixture = new Fixture();
            CreateAsymmetricKeyRequest createRequest = fixture
                .Build<CreateAsymmetricKeyRequest>()
                .Create();

            CreateAsymmetricKeyResponse createResponse = await azureKeyVault.CreateAsymmetricKeyAsync(createRequest, default);

            createResponse.Should().NotBeNull();
            createResponse.AsymmetricKeyDefinition.Should().NotBeNull();
            createResponse.AsymmetricKeyDefinition.Id.Should().NotBeNullOrEmpty();
            createResponse.AsymmetricKeyDefinition.Name.Should().Be(createRequest.Name);
            createResponse.AsymmetricKeyDefinition.Version.Should().NotBeNullOrEmpty();
            createResponse.AsymmetricKeyDefinition.IsEnabled.Should().BeTrue();
            createResponse.AsymmetricKeyDefinition.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));

            DisableAsymmetricKeyRequest disableRequest = fixture
                .Build<DisableAsymmetricKeyRequest>()
                .With(x => x.Name, createResponse.AsymmetricKeyDefinition.Name)
                .With(x => x.Version, createResponse.AsymmetricKeyDefinition.Version)
                .Create();

            DisableAsymmetricKeyResponse disableResponse = await azureKeyVault.DisableAsymmetricKeyAsync(disableRequest, default);

            disableResponse.Should().NotBeNull();
            disableResponse.AsymmetricKeyDefinition.Should().NotBeNull();
            disableResponse.AsymmetricKeyDefinition.Id.Should().Be(createResponse.AsymmetricKeyDefinition.Id);
            disableResponse.AsymmetricKeyDefinition.Name.Should().Be(createResponse.AsymmetricKeyDefinition.Name);
            disableResponse.AsymmetricKeyDefinition.Version.Should().Be(createResponse.AsymmetricKeyDefinition.Version);
            disableResponse.AsymmetricKeyDefinition.IsEnabled.Should().BeFalse();
            disableResponse.AsymmetricKeyDefinition.CreatedAt.Should().Be(createResponse.AsymmetricKeyDefinition.CreatedAt);

            EnableAsymmetricKeyRequest enableRequest = fixture
                .Build<EnableAsymmetricKeyRequest>()
                .With(x => x.Name, createResponse.AsymmetricKeyDefinition.Name)
                .With(x => x.Version, createResponse.AsymmetricKeyDefinition.Version)
                .Create();

            EnableAsymmetricKeyResponse enableResponse = await azureKeyVault.EnableAsymmetricKeyAsync(enableRequest, default);

            enableResponse.Should().NotBeNull();
            enableResponse.AsymmetricKeyDefinition.Should().NotBeNull();
            enableResponse.AsymmetricKeyDefinition.Should().BeEquivalentTo(createResponse.AsymmetricKeyDefinition);

            RemoveAsymmetricKeyRequest removeRequest = fixture
                  .Build<RemoveAsymmetricKeyRequest>()
                  .With(x => x.Name, createRequest.Name)
                  .With(x => x.AwaitCompletion, false)
                  .Create();

            await azureKeyVault.RemoveAsymmetricKeyAsync(removeRequest, default);
        }

        [Fact]
        public async Task AzureKeyVault_GivenEnableAsymmetricKeyAsync_WhenValidInputsSentTwice_ThenKeyEnabled()
        {
            IAsymmetricKeyVault azureKeyVault = m_AzureKeyVaultFixture.ServerServices.GetService<IAsymmetricKeyVault>();

            var fixture = new Fixture();
            CreateAsymmetricKeyRequest createRequest = fixture
                .Build<CreateAsymmetricKeyRequest>()
                .Create();

            CreateAsymmetricKeyResponse createResponse = await azureKeyVault.CreateAsymmetricKeyAsync(createRequest, default);

            createResponse.Should().NotBeNull();
            createResponse.AsymmetricKeyDefinition.Should().NotBeNull();
            createResponse.AsymmetricKeyDefinition.Id.Should().NotBeNullOrEmpty();
            createResponse.AsymmetricKeyDefinition.Name.Should().Be(createRequest.Name);
            createResponse.AsymmetricKeyDefinition.Version.Should().NotBeNullOrEmpty();
            createResponse.AsymmetricKeyDefinition.IsEnabled.Should().BeTrue();
            createResponse.AsymmetricKeyDefinition.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));

            DisableAsymmetricKeyRequest disableRequest = fixture
                .Build<DisableAsymmetricKeyRequest>()
                .With(x => x.Name, createResponse.AsymmetricKeyDefinition.Name)
                .With(x => x.Version, createResponse.AsymmetricKeyDefinition.Version)
                .Create();

            DisableAsymmetricKeyResponse disableResponse = await azureKeyVault.DisableAsymmetricKeyAsync(disableRequest, default);

            disableResponse.Should().NotBeNull();
            disableResponse.AsymmetricKeyDefinition.Should().NotBeNull();
            disableResponse.AsymmetricKeyDefinition.Id.Should().Be(createResponse.AsymmetricKeyDefinition.Id);
            disableResponse.AsymmetricKeyDefinition.Name.Should().Be(createResponse.AsymmetricKeyDefinition.Name);
            disableResponse.AsymmetricKeyDefinition.Version.Should().Be(createResponse.AsymmetricKeyDefinition.Version);
            disableResponse.AsymmetricKeyDefinition.IsEnabled.Should().BeFalse();
            disableResponse.AsymmetricKeyDefinition.CreatedAt.Should().Be(createResponse.AsymmetricKeyDefinition.CreatedAt);

            EnableAsymmetricKeyRequest enableRequest = fixture
                .Build<EnableAsymmetricKeyRequest>()
                .With(x => x.Name, createResponse.AsymmetricKeyDefinition.Name)
                .With(x => x.Version, createResponse.AsymmetricKeyDefinition.Version)
                .Create();

            EnableAsymmetricKeyResponse enableResponse1 = await azureKeyVault.EnableAsymmetricKeyAsync(enableRequest, default);
            EnableAsymmetricKeyResponse enableResponse2 = await azureKeyVault.EnableAsymmetricKeyAsync(enableRequest, default);

            enableResponse1.Should().NotBeNull();
            enableResponse2.Should().NotBeNull();

            enableResponse1.Should().BeEquivalentTo(enableResponse2);

            RemoveAsymmetricKeyRequest removeRequest = fixture
                  .Build<RemoveAsymmetricKeyRequest>()
                  .With(x => x.Name, createRequest.Name)
                  .With(x => x.AwaitCompletion, false)
                  .Create();

            await azureKeyVault.RemoveAsymmetricKeyAsync(removeRequest, default);
        }

        [Fact]
        public async Task AzureKeyVault_GivenEnableAsymmetricKeyAsync_WhenInvalidName_ThenExceptionThrown()
        {
            IAsymmetricKeyVault azureKeyVault = m_AzureKeyVaultFixture.ServerServices.GetService<IAsymmetricKeyVault>();

            var fixture = new Fixture();
            CreateAsymmetricKeyRequest createRequest = fixture
                .Build<CreateAsymmetricKeyRequest>()
                .Create();

            CreateAsymmetricKeyResponse createResponse = await azureKeyVault.CreateAsymmetricKeyAsync(createRequest, default);

            createResponse.Should().NotBeNull();
            createResponse.AsymmetricKeyDefinition.Should().NotBeNull();
            createResponse.AsymmetricKeyDefinition.Id.Should().NotBeNullOrEmpty();
            createResponse.AsymmetricKeyDefinition.Name.Should().Be(createRequest.Name);
            createResponse.AsymmetricKeyDefinition.Version.Should().NotBeNullOrEmpty();
            createResponse.AsymmetricKeyDefinition.IsEnabled.Should().BeTrue();
            createResponse.AsymmetricKeyDefinition.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));

            DisableAsymmetricKeyRequest disableRequest = fixture
                .Build<DisableAsymmetricKeyRequest>()
                .With(x => x.Name, createResponse.AsymmetricKeyDefinition.Name)
                .With(x => x.Version, createResponse.AsymmetricKeyDefinition.Version)
                .Create();

            DisableAsymmetricKeyResponse disableResponse = await azureKeyVault.DisableAsymmetricKeyAsync(disableRequest, default);

            // Replace name.

            AsymmetricKeyDefinition key = createResponse.AsymmetricKeyDefinition;

            EnableAsymmetricKeyRequest enableRequest = fixture
                .Build<EnableAsymmetricKeyRequest>()
                .With(x => x.Name, Guid.NewGuid().ToDashedString())
                .With(x => x.Version, key.Version)
                .Create();

            Func<Task> act = async () => await azureKeyVault.EnableAsymmetricKeyAsync(enableRequest, default);

            await act.Should().ThrowAsync<Exception>();

            RemoveAsymmetricKeyRequest removeRequest = fixture
                  .Build<RemoveAsymmetricKeyRequest>()
                  .With(x => x.Name, createRequest.Name)
                  .With(x => x.AwaitCompletion, false)
                  .Create();

            await azureKeyVault.RemoveAsymmetricKeyAsync(removeRequest, default);
        }

        [Fact]
        public async Task AzureKeyVault_GivenEnableAsymmetricKeyAsync_WhenInvalidVersion_ThenExceptionThrown()
        {
            IAsymmetricKeyVault azureKeyVault = m_AzureKeyVaultFixture.ServerServices.GetService<IAsymmetricKeyVault>();

            var fixture = new Fixture();
            CreateAsymmetricKeyRequest createRequest = fixture
                .Build<CreateAsymmetricKeyRequest>()
                .Create();

            CreateAsymmetricKeyResponse createResponse = await azureKeyVault.CreateAsymmetricKeyAsync(createRequest, default);

            createResponse.Should().NotBeNull();
            createResponse.AsymmetricKeyDefinition.Should().NotBeNull();
            createResponse.AsymmetricKeyDefinition.Id.Should().NotBeNullOrEmpty();
            createResponse.AsymmetricKeyDefinition.Name.Should().Be(createRequest.Name);
            createResponse.AsymmetricKeyDefinition.Version.Should().NotBeNullOrEmpty();
            createResponse.AsymmetricKeyDefinition.IsEnabled.Should().BeTrue();
            createResponse.AsymmetricKeyDefinition.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));

            DisableAsymmetricKeyRequest disableRequest = fixture
                .Build<DisableAsymmetricKeyRequest>()
                .With(x => x.Name, createResponse.AsymmetricKeyDefinition.Name)
                .With(x => x.Version, createResponse.AsymmetricKeyDefinition.Version)
                .Create();

            DisableAsymmetricKeyResponse disableResponse = await azureKeyVault.DisableAsymmetricKeyAsync(disableRequest, default);

            // Replace version.

            AsymmetricKeyDefinition key = createResponse.AsymmetricKeyDefinition;

            EnableAsymmetricKeyRequest enableRequest = fixture
                .Build<EnableAsymmetricKeyRequest>()
                .With(x => x.Name, key.Name)
                .With(x => x.Version, Guid.NewGuid().ToFlatString())
                .Create();

            Func<Task> act = async () => await azureKeyVault.EnableAsymmetricKeyAsync(enableRequest, default);

            await act.Should().ThrowAsync<Exception>();

            RemoveAsymmetricKeyRequest removeRequest = fixture
                  .Build<RemoveAsymmetricKeyRequest>()
                  .With(x => x.Name, createRequest.Name)
                  .With(x => x.AwaitCompletion, false)
                  .Create();

            await azureKeyVault.RemoveAsymmetricKeyAsync(removeRequest, default);
        }

        [Fact]
        public async Task AzureKeyVault_GivenEnableAsymmetricKeyAsync_WhenNoKeyExists_ThenExceptionThrown()
        {
            IAsymmetricKeyVault azureKeyVault = m_AzureKeyVaultFixture.ServerServices.GetService<IAsymmetricKeyVault>();

            var fixture = new Fixture();
            EnableAsymmetricKeyRequest enableRequest = fixture
                .Build<EnableAsymmetricKeyRequest>()
                .With(x => x.Name, Guid.NewGuid().ToDashedString())
                .With(x => x.Version, Guid.NewGuid().ToFlatString())
                .Create();

            Func<Task> act = async () => await azureKeyVault.EnableAsymmetricKeyAsync(enableRequest, default);

            await act.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task AzureKeyVault_GivenRemoveAsymmetricKeyAsync_WhenValidInputs_ThenKeyDeleted()
        {
            IAsymmetricKeyVault azureKeyVault = m_AzureKeyVaultFixture.ServerServices.GetService<IAsymmetricKeyVault>();

            var fixture = new Fixture();
            CreateAsymmetricKeyRequest createRequest = fixture
                .Build<CreateAsymmetricKeyRequest>()
                .Create();

            CreateAsymmetricKeyResponse createResponse = await azureKeyVault.CreateAsymmetricKeyAsync(createRequest, default);

            createResponse.Should().NotBeNull();
            createResponse.AsymmetricKeyDefinition.Should().NotBeNull();

            RemoveAsymmetricKeyRequest removeRequest = fixture
                  .Build<RemoveAsymmetricKeyRequest>()
                  .With(x => x.Name, createRequest.Name)
                  .With(x => x.AwaitCompletion, true)
                  .Create();

            bool deleted = await azureKeyVault.RemoveAsymmetricKeyAsync(removeRequest, default);

            deleted.Should().BeTrue();

            ViewAsymmetricKeyDefinitionRequest viewRequest = fixture
                  .Build<ViewAsymmetricKeyDefinitionRequest>()
                  .With(x => x.Name, createResponse.AsymmetricKeyDefinition.Name)
                  .With(x => x.Version, createResponse.AsymmetricKeyDefinition.Version)
                  .Create();

            Func<Task> act = async () => await azureKeyVault.ViewAsymmetricKeyDefinitionAsync(viewRequest, default);

            await act.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task AzureKeyVault_GivenRemoveAsymmetricKeyAsync_WhenInvalidName_ThenExceptionThrown()
        {
            IAsymmetricKeyVault azureKeyVault = m_AzureKeyVaultFixture.ServerServices.GetService<IAsymmetricKeyVault>();

            var fixture = new Fixture();
            CreateAsymmetricKeyRequest createRequest = fixture
                .Build<CreateAsymmetricKeyRequest>()
                .Create();

            CreateAsymmetricKeyResponse createResponse = await azureKeyVault.CreateAsymmetricKeyAsync(createRequest, default);

            createResponse.Should().NotBeNull();
            createResponse.AsymmetricKeyDefinition.Should().NotBeNull();

            RemoveAsymmetricKeyRequest removeRequest = fixture
                  .Build<RemoveAsymmetricKeyRequest>()
                  .With(x => x.AwaitCompletion, false)
                  .Create();

            Func<Task> act = async () => await azureKeyVault.RemoveAsymmetricKeyAsync(removeRequest, default);

            await act.Should().ThrowAsync<Exception>();

            removeRequest = fixture
                  .Build<RemoveAsymmetricKeyRequest>()
                  .With(x => x.Name, createRequest.Name)
                  .With(x => x.AwaitCompletion, false)
                  .Create();

            bool deleted = await azureKeyVault.RemoveAsymmetricKeyAsync(removeRequest, default);

            deleted.Should().BeTrue();
        }

        [Fact]
        public async Task AzureKeyVault_GivenRemoveAsymmetricKeyAsync_WhenSameRequestSentTwice_ThenExceptionThrown()
        {
            IAsymmetricKeyVault azureKeyVault = m_AzureKeyVaultFixture.ServerServices.GetService<IAsymmetricKeyVault>();

            var fixture = new Fixture();
            CreateAsymmetricKeyRequest createRequest = fixture
                .Build<CreateAsymmetricKeyRequest>()
                .Create();

            CreateAsymmetricKeyResponse createResponse = await azureKeyVault.CreateAsymmetricKeyAsync(createRequest, default);

            createResponse.Should().NotBeNull();
            createResponse.AsymmetricKeyDefinition.Should().NotBeNull();

            RemoveAsymmetricKeyRequest removeRequest = fixture
                  .Build<RemoveAsymmetricKeyRequest>()
                  .With(x => x.Name, createRequest.Name)
                  .With(x => x.AwaitCompletion, false)
                  .Create();

            bool deleted = await azureKeyVault.RemoveAsymmetricKeyAsync(removeRequest, default);

            deleted.Should().BeTrue();

            Func<Task> act = async () => await azureKeyVault.RemoveAsymmetricKeyAsync(removeRequest, default);

            await act.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task AzureKeyVault_GivenWrapSymmetricKeyAsync_WhenValidInputs_ThenKeyWrapped()
        {
            IAsymmetricKeyVault azureKeyVault = m_AzureKeyVaultFixture.ServerServices.GetService<IAsymmetricKeyVault>();

            var fixture = new Fixture();
            CreateAsymmetricKeyRequest createRequest = fixture
                .Build<CreateAsymmetricKeyRequest>()
                .Create();

            CreateAsymmetricKeyResponse createResponse = await azureKeyVault.CreateAsymmetricKeyAsync(createRequest, default);

            createResponse.Should().NotBeNull();
            createResponse.AsymmetricKeyDefinition.Should().NotBeNull();

            WrapSymmetricKeyRequest wrapRequest = fixture
                .Build<WrapSymmetricKeyRequest>()
                .With(x => x.AsymmetricKeyName, createResponse.AsymmetricKeyDefinition.Name)
                .With(x => x.AsymmetricKeyVersion, createResponse.AsymmetricKeyDefinition.Version)
                .Create();

            WrapSymmetricKeyResponse wrapResponse = await azureKeyVault.WrapSymmetricKeyAsync(wrapRequest, default);

            wrapResponse.Should().NotBeNull();
            wrapResponse.WrappedSymmetricKey.Should().NotBeNull();
            wrapResponse.WrappedSymmetricKey.Should().NotBeEmpty();
            wrapResponse.WrappedSymmetricKey.Should().NotBeEquivalentTo(wrapRequest.SymmetricKey);

            RemoveAsymmetricKeyRequest removeRequest = fixture
                  .Build<RemoveAsymmetricKeyRequest>()
                  .With(x => x.Name, createRequest.Name)
                  .With(x => x.AwaitCompletion, false)
                  .Create();

            await azureKeyVault.RemoveAsymmetricKeyAsync(removeRequest, default);
        }

        [Fact]
        public async Task AzureKeyVault_GivenWrapSymmetricKeyAsync_WhenKeyDisabled_ThenExceptionThrown()
        {
            IAsymmetricKeyVault azureKeyVault = m_AzureKeyVaultFixture.ServerServices.GetService<IAsymmetricKeyVault>();

            var fixture = new Fixture();
            CreateAsymmetricKeyRequest createRequest = fixture
                .Build<CreateAsymmetricKeyRequest>()
                .Create();

            CreateAsymmetricKeyResponse createResponse = await azureKeyVault.CreateAsymmetricKeyAsync(createRequest, default);

            createResponse.Should().NotBeNull();
            createResponse.AsymmetricKeyDefinition.Should().NotBeNull();

            DisableAsymmetricKeyRequest disableRequest = fixture
                .Build<DisableAsymmetricKeyRequest>()
                .With(x => x.Name, createResponse.AsymmetricKeyDefinition.Name)
                .With(x => x.Version, createResponse.AsymmetricKeyDefinition.Version)
                .Create();

            DisableAsymmetricKeyResponse disableResponse = await azureKeyVault.DisableAsymmetricKeyAsync(disableRequest, default);

            disableResponse.Should().NotBeNull();
            disableResponse.AsymmetricKeyDefinition.Should().NotBeNull();
            disableResponse.AsymmetricKeyDefinition.Id.Should().Be(createResponse.AsymmetricKeyDefinition.Id);
            disableResponse.AsymmetricKeyDefinition.Name.Should().Be(createResponse.AsymmetricKeyDefinition.Name);
            disableResponse.AsymmetricKeyDefinition.Version.Should().Be(createResponse.AsymmetricKeyDefinition.Version);
            disableResponse.AsymmetricKeyDefinition.IsEnabled.Should().BeFalse();
            disableResponse.AsymmetricKeyDefinition.CreatedAt.Should().Be(createResponse.AsymmetricKeyDefinition.CreatedAt);

            WrapSymmetricKeyRequest wrapRequest = fixture
                .Build<WrapSymmetricKeyRequest>()
                .With(x => x.AsymmetricKeyName, createResponse.AsymmetricKeyDefinition.Name)
                .With(x => x.AsymmetricKeyVersion, createResponse.AsymmetricKeyDefinition.Version)
                .Create();

            Func<Task> act = async () => await azureKeyVault.WrapSymmetricKeyAsync(wrapRequest, default);

            await act.Should().ThrowAsync<InvalidOperationException>();

            RemoveAsymmetricKeyRequest removeRequest = fixture
                  .Build<RemoveAsymmetricKeyRequest>()
                  .With(x => x.Name, createRequest.Name)
                  .With(x => x.AwaitCompletion, false)
                  .Create();

            await azureKeyVault.RemoveAsymmetricKeyAsync(removeRequest, default);
        }

        [Fact]
        public async Task AzureKeyVault_GivenWrapSymmetricKeyAsync_WhenInvalidName_ThenExceptionThrown()
        {
            IAsymmetricKeyVault azureKeyVault = m_AzureKeyVaultFixture.ServerServices.GetService<IAsymmetricKeyVault>();

            var fixture = new Fixture();
            CreateAsymmetricKeyRequest createRequest = fixture
                .Build<CreateAsymmetricKeyRequest>()
                .Create();

            CreateAsymmetricKeyResponse createResponse = await azureKeyVault.CreateAsymmetricKeyAsync(createRequest, default);

            createResponse.Should().NotBeNull();
            createResponse.AsymmetricKeyDefinition.Should().NotBeNull();

            // Replace name.

            WrapSymmetricKeyRequest wrapRequest = fixture
                .Build<WrapSymmetricKeyRequest>()
                .With(x => x.AsymmetricKeyVersion, createResponse.AsymmetricKeyDefinition.Version)
                .Create();

            Func<Task> act = async () => await azureKeyVault.WrapSymmetricKeyAsync(wrapRequest, default);

            await act.Should().ThrowAsync<Exception>();

            RemoveAsymmetricKeyRequest removeRequest = fixture
                  .Build<RemoveAsymmetricKeyRequest>()
                  .With(x => x.Name, createRequest.Name)
                  .With(x => x.AwaitCompletion, false)
                  .Create();

            await azureKeyVault.RemoveAsymmetricKeyAsync(removeRequest, default);
        }

        [Fact]
        public async Task AzureKeyVault_GivenWrapSymmetricKeyAsync_WhenInvalidVersion_ThenExceptionThrown()
        {
            IAsymmetricKeyVault azureKeyVault = m_AzureKeyVaultFixture.ServerServices.GetService<IAsymmetricKeyVault>();

            var fixture = new Fixture();
            CreateAsymmetricKeyRequest createRequest = fixture
                .Build<CreateAsymmetricKeyRequest>()
                .Create();

            CreateAsymmetricKeyResponse createResponse = await azureKeyVault.CreateAsymmetricKeyAsync(createRequest, default);

            createResponse.Should().NotBeNull();
            createResponse.AsymmetricKeyDefinition.Should().NotBeNull();

            // Replace version.

            WrapSymmetricKeyRequest wrapRequest = fixture
                .Build<WrapSymmetricKeyRequest>()
                .With(x => x.AsymmetricKeyName, createResponse.AsymmetricKeyDefinition.Name)
                .Create();

            Func<Task> act = async () => await azureKeyVault.WrapSymmetricKeyAsync(wrapRequest, default);

            await act.Should().ThrowAsync<Exception>();

            RemoveAsymmetricKeyRequest removeRequest = fixture
                  .Build<RemoveAsymmetricKeyRequest>()
                  .With(x => x.Name, createRequest.Name)
                  .With(x => x.AwaitCompletion, false)
                  .Create();

            await azureKeyVault.RemoveAsymmetricKeyAsync(removeRequest, default);
        }

        [Fact]
        public async Task AzureKeyVault_GivenWrapSymmetricKeyAsync_WhenKeyCreatedTwice_ThenAbleToSelectVersion()
        {
            IAsymmetricKeyVault azureKeyVault = m_AzureKeyVaultFixture.ServerServices.GetService<IAsymmetricKeyVault>();

            var fixture = new Fixture();
            CreateAsymmetricKeyRequest createRequest = fixture
                .Build<CreateAsymmetricKeyRequest>()
                .Create();

            CreateAsymmetricKeyResponse createResponse1 = await azureKeyVault.CreateAsymmetricKeyAsync(createRequest, default);
            CreateAsymmetricKeyResponse createResponse2 = await azureKeyVault.CreateAsymmetricKeyAsync(createRequest, default);

            createResponse1.Should().NotBeNull();
            createResponse2.Should().NotBeNull();

            createResponse1.Should().NotBeEquivalentTo(createResponse2);

            WrapSymmetricKeyRequest wrapRequest1 = fixture
                .Build<WrapSymmetricKeyRequest>()
                .With(x => x.AsymmetricKeyName, createResponse1.AsymmetricKeyDefinition.Name)
                .With(x => x.AsymmetricKeyVersion, createResponse1.AsymmetricKeyDefinition.Version)
                .Create();

            WrapSymmetricKeyResponse wrapResponse1 = await azureKeyVault.WrapSymmetricKeyAsync(wrapRequest1, default);

            wrapResponse1.Should().NotBeNull();
            wrapResponse1.WrappedSymmetricKey.Should().NotBeNull();
            wrapResponse1.WrappedSymmetricKey.Should().NotBeEmpty();
            wrapResponse1.WrappedSymmetricKey.Should().NotBeEquivalentTo(wrapRequest1.SymmetricKey);

            WrapSymmetricKeyRequest wrapRequest2 = fixture
                .Build<WrapSymmetricKeyRequest>()
                .With(x => x.AsymmetricKeyName, createResponse2.AsymmetricKeyDefinition.Name)
                .With(x => x.AsymmetricKeyVersion, createResponse2.AsymmetricKeyDefinition.Version)
                .Create();

            WrapSymmetricKeyResponse wrapResponse2 = await azureKeyVault.WrapSymmetricKeyAsync(wrapRequest2, default);

            wrapResponse2.Should().NotBeNull();
            wrapResponse2.WrappedSymmetricKey.Should().NotBeNull();
            wrapResponse2.WrappedSymmetricKey.Should().NotBeEmpty();
            wrapResponse2.WrappedSymmetricKey.Should().NotBeEquivalentTo(wrapRequest2.SymmetricKey);

            wrapResponse1.WrappedSymmetricKey.Should().NotBeEquivalentTo(wrapResponse2.WrappedSymmetricKey);

            RemoveAsymmetricKeyRequest removeRequest = fixture
                  .Build<RemoveAsymmetricKeyRequest>()
                  .With(x => x.Name, createRequest.Name)
                  .With(x => x.AwaitCompletion, false)
                  .Create();

            await azureKeyVault.RemoveAsymmetricKeyAsync(removeRequest, default);
        }

        [Fact]
        public async Task AzureKeyVault_GivenUnwrapSymmetricKeyAsync_WhenValidInputs_ThenKeyUnwrapped()
        {
            IAsymmetricKeyVault azureKeyVault = m_AzureKeyVaultFixture.ServerServices.GetService<IAsymmetricKeyVault>();

            var fixture = new Fixture();
            CreateAsymmetricKeyRequest createRequest = fixture
                .Build<CreateAsymmetricKeyRequest>()
                .Create();

            CreateAsymmetricKeyResponse createResponse = await azureKeyVault.CreateAsymmetricKeyAsync(createRequest, default);

            createResponse.Should().NotBeNull();
            createResponse.AsymmetricKeyDefinition.Should().NotBeNull();

            WrapSymmetricKeyRequest wrapRequest = fixture
                .Build<WrapSymmetricKeyRequest>()
                .With(x => x.AsymmetricKeyName, createResponse.AsymmetricKeyDefinition.Name)
                .With(x => x.AsymmetricKeyVersion, createResponse.AsymmetricKeyDefinition.Version)
                .Create();

            WrapSymmetricKeyResponse wrapResponse = await azureKeyVault.WrapSymmetricKeyAsync(wrapRequest, default);

            wrapResponse.Should().NotBeNull();
            wrapResponse.WrappedSymmetricKey.Should().NotBeNull();
            wrapResponse.WrappedSymmetricKey.Should().NotBeEmpty();
            wrapResponse.WrappedSymmetricKey.Should().NotBeEquivalentTo(wrapRequest.SymmetricKey);

            UnwrapSymmetricKeyRequest unwrapRequest = fixture
                .Build<UnwrapSymmetricKeyRequest>()
                .With(x => x.AsymmetricKeyName, createResponse.AsymmetricKeyDefinition.Name)
                .With(x => x.AsymmetricKeyVersion, createResponse.AsymmetricKeyDefinition.Version)
                .With(x => x.WrappedSymmetricKey, wrapResponse.WrappedSymmetricKey)
                .Create();

            UnwrapSymmetricKeyResponse unwrapResponse = await azureKeyVault.UnwrapSymmetricKeyAsync(unwrapRequest, default);

            unwrapResponse.Should().NotBeNull();
            unwrapResponse.SymmetricKey.Should().NotBeNull();
            unwrapResponse.SymmetricKey.Should().NotBeEmpty();
            unwrapResponse.SymmetricKey.Should().BeEquivalentTo(wrapRequest.SymmetricKey);

            RemoveAsymmetricKeyRequest removeRequest = fixture
                  .Build<RemoveAsymmetricKeyRequest>()
                  .With(x => x.Name, createRequest.Name)
                  .With(x => x.AwaitCompletion, false)
                  .Create();

            await azureKeyVault.RemoveAsymmetricKeyAsync(removeRequest, default);
        }

        [Fact]
        public async Task AzureKeyVault_GivenUnwrapSymmetricKeyAsync_WhenKeyDisabled_ThenExceptionThrown()
        {
            IAsymmetricKeyVault azureKeyVault = m_AzureKeyVaultFixture.ServerServices.GetService<IAsymmetricKeyVault>();

            var fixture = new Fixture();
            CreateAsymmetricKeyRequest createRequest = fixture
                .Build<CreateAsymmetricKeyRequest>()
                .Create();

            CreateAsymmetricKeyResponse createResponse = await azureKeyVault.CreateAsymmetricKeyAsync(createRequest, default);

            createResponse.Should().NotBeNull();
            createResponse.AsymmetricKeyDefinition.Should().NotBeNull();

            WrapSymmetricKeyRequest wrapRequest = fixture
                .Build<WrapSymmetricKeyRequest>()
                .With(x => x.AsymmetricKeyName, createResponse.AsymmetricKeyDefinition.Name)
                .With(x => x.AsymmetricKeyVersion, createResponse.AsymmetricKeyDefinition.Version)
                .Create();

            WrapSymmetricKeyResponse wrapResponse = await azureKeyVault.WrapSymmetricKeyAsync(wrapRequest, default);

            wrapResponse.Should().NotBeNull();
            wrapResponse.WrappedSymmetricKey.Should().NotBeNull();
            wrapResponse.WrappedSymmetricKey.Should().NotBeEmpty();
            wrapResponse.WrappedSymmetricKey.Should().NotBeEquivalentTo(wrapRequest.SymmetricKey);

            DisableAsymmetricKeyRequest disableRequest = fixture
                   .Build<DisableAsymmetricKeyRequest>()
                   .With(x => x.Name, createResponse.AsymmetricKeyDefinition.Name)
                   .With(x => x.Version, createResponse.AsymmetricKeyDefinition.Version)
                   .Create();

            DisableAsymmetricKeyResponse disableResponse = await azureKeyVault.DisableAsymmetricKeyAsync(disableRequest, default);

            disableResponse.Should().NotBeNull();
            disableResponse.AsymmetricKeyDefinition.Should().NotBeNull();
            disableResponse.AsymmetricKeyDefinition.Id.Should().Be(createResponse.AsymmetricKeyDefinition.Id);
            disableResponse.AsymmetricKeyDefinition.Name.Should().Be(createResponse.AsymmetricKeyDefinition.Name);
            disableResponse.AsymmetricKeyDefinition.Version.Should().Be(createResponse.AsymmetricKeyDefinition.Version);
            disableResponse.AsymmetricKeyDefinition.IsEnabled.Should().BeFalse();
            disableResponse.AsymmetricKeyDefinition.CreatedAt.Should().Be(createResponse.AsymmetricKeyDefinition.CreatedAt);

            UnwrapSymmetricKeyRequest unwrapRequest = fixture
                .Build<UnwrapSymmetricKeyRequest>()
                .With(x => x.AsymmetricKeyName, createResponse.AsymmetricKeyDefinition.Name)
                .With(x => x.AsymmetricKeyVersion, createResponse.AsymmetricKeyDefinition.Version)
                .With(x => x.WrappedSymmetricKey, wrapResponse.WrappedSymmetricKey)
                .Create();

            Func<Task> act = async () => await azureKeyVault.UnwrapSymmetricKeyAsync(unwrapRequest, default);

            await act.Should().ThrowAsync<InvalidOperationException>();

            RemoveAsymmetricKeyRequest removeRequest = fixture
                  .Build<RemoveAsymmetricKeyRequest>()
                  .With(x => x.Name, createRequest.Name)
                  .With(x => x.AwaitCompletion, false)
                  .Create();

            await azureKeyVault.RemoveAsymmetricKeyAsync(removeRequest, default);
        }

        [Fact]
        public async Task AzureKeyVault_GivenUnwrapSymmetricKeyAsync_WhenInvalidName_ThenExceptionThrown()
        {
            IAsymmetricKeyVault azureKeyVault = m_AzureKeyVaultFixture.ServerServices.GetService<IAsymmetricKeyVault>();

            var fixture = new Fixture();
            CreateAsymmetricKeyRequest createRequest = fixture
                .Build<CreateAsymmetricKeyRequest>()
                .Create();

            CreateAsymmetricKeyResponse createResponse = await azureKeyVault.CreateAsymmetricKeyAsync(createRequest, default);

            createResponse.Should().NotBeNull();
            createResponse.AsymmetricKeyDefinition.Should().NotBeNull();

            WrapSymmetricKeyRequest wrapRequest = fixture
                .Build<WrapSymmetricKeyRequest>()
                .With(x => x.AsymmetricKeyName, createResponse.AsymmetricKeyDefinition.Name)
                .With(x => x.AsymmetricKeyVersion, createResponse.AsymmetricKeyDefinition.Version)
                .Create();

            WrapSymmetricKeyResponse wrapResponse = await azureKeyVault.WrapSymmetricKeyAsync(wrapRequest, default);

            wrapResponse.Should().NotBeNull();
            wrapResponse.WrappedSymmetricKey.Should().NotBeNull();
            wrapResponse.WrappedSymmetricKey.Should().NotBeEmpty();
            wrapResponse.WrappedSymmetricKey.Should().NotBeEquivalentTo(wrapRequest.SymmetricKey);

            // Replace name.

            UnwrapSymmetricKeyRequest unwrapRequest = fixture
                .Build<UnwrapSymmetricKeyRequest>()
                .With(x => x.AsymmetricKeyVersion, createResponse.AsymmetricKeyDefinition.Version)
                .With(x => x.WrappedSymmetricKey, wrapResponse.WrappedSymmetricKey)
                .Create();

            Func<Task> act = async () => await azureKeyVault.UnwrapSymmetricKeyAsync(unwrapRequest, default);

            await act.Should().ThrowAsync<Exception>();

            RemoveAsymmetricKeyRequest removeRequest = fixture
                  .Build<RemoveAsymmetricKeyRequest>()
                  .With(x => x.Name, createRequest.Name)
                  .With(x => x.AwaitCompletion, false)
                  .Create();

            await azureKeyVault.RemoveAsymmetricKeyAsync(removeRequest, default);
        }

        [Fact]
        public async Task AzureKeyVault_GivenUnwrapSymmetricKeyAsync_WhenInvalidVersion_ThenExceptionThrown()
        {
            IAsymmetricKeyVault azureKeyVault = m_AzureKeyVaultFixture.ServerServices.GetService<IAsymmetricKeyVault>();

            var fixture = new Fixture();
            CreateAsymmetricKeyRequest createRequest = fixture
                .Build<CreateAsymmetricKeyRequest>()
                .Create();

            CreateAsymmetricKeyResponse createResponse = await azureKeyVault.CreateAsymmetricKeyAsync(createRequest, default);

            createResponse.Should().NotBeNull();
            createResponse.AsymmetricKeyDefinition.Should().NotBeNull();

            WrapSymmetricKeyRequest wrapRequest = fixture
                .Build<WrapSymmetricKeyRequest>()
                .With(x => x.AsymmetricKeyName, createResponse.AsymmetricKeyDefinition.Name)
                .With(x => x.AsymmetricKeyVersion, createResponse.AsymmetricKeyDefinition.Version)
                .Create();

            WrapSymmetricKeyResponse wrapResponse = await azureKeyVault.WrapSymmetricKeyAsync(wrapRequest, default);

            wrapResponse.Should().NotBeNull();
            wrapResponse.WrappedSymmetricKey.Should().NotBeNull();
            wrapResponse.WrappedSymmetricKey.Should().NotBeEmpty();
            wrapResponse.WrappedSymmetricKey.Should().NotBeEquivalentTo(wrapRequest.SymmetricKey);

            // Replace version.

            UnwrapSymmetricKeyRequest unwrapRequest = fixture
                .Build<UnwrapSymmetricKeyRequest>()
                .With(x => x.AsymmetricKeyName, createResponse.AsymmetricKeyDefinition.Name)
                .With(x => x.WrappedSymmetricKey, wrapResponse.WrappedSymmetricKey)
                .Create();

            Func<Task> act = async () => await azureKeyVault.UnwrapSymmetricKeyAsync(unwrapRequest, default);

            await act.Should().ThrowAsync<Exception>();

            RemoveAsymmetricKeyRequest removeRequest = fixture
                  .Build<RemoveAsymmetricKeyRequest>()
                  .With(x => x.Name, createRequest.Name)
                  .With(x => x.AwaitCompletion, false)
                  .Create();

            await azureKeyVault.RemoveAsymmetricKeyAsync(removeRequest, default);
        }

        [Fact]
        public async Task AzureKeyVault_GivenUnwrapSymmetricKeyAsync_WhenKeyCreatedTwice_ThenAbleToSelectVersion()
        {
            IAsymmetricKeyVault azureKeyVault = m_AzureKeyVaultFixture.ServerServices.GetService<IAsymmetricKeyVault>();

            var fixture = new Fixture();
            CreateAsymmetricKeyRequest createRequest = fixture
                .Build<CreateAsymmetricKeyRequest>()
                .Create();

            CreateAsymmetricKeyResponse createResponse1 = await azureKeyVault.CreateAsymmetricKeyAsync(createRequest, default);
            CreateAsymmetricKeyResponse createResponse2 = await azureKeyVault.CreateAsymmetricKeyAsync(createRequest, default);

            createResponse1.Should().NotBeNull();
            createResponse2.Should().NotBeNull();

            createResponse1.Should().NotBeEquivalentTo(createResponse2);

            WrapSymmetricKeyRequest wrapRequest1 = fixture
                .Build<WrapSymmetricKeyRequest>()
                .With(x => x.AsymmetricKeyName, createResponse1.AsymmetricKeyDefinition.Name)
                .With(x => x.AsymmetricKeyVersion, createResponse1.AsymmetricKeyDefinition.Version)
                .Create();

            WrapSymmetricKeyResponse wrapResponse1 = await azureKeyVault.WrapSymmetricKeyAsync(wrapRequest1, default);

            wrapResponse1.Should().NotBeNull();
            wrapResponse1.WrappedSymmetricKey.Should().NotBeNull();
            wrapResponse1.WrappedSymmetricKey.Should().NotBeEmpty();
            wrapResponse1.WrappedSymmetricKey.Should().NotBeEquivalentTo(wrapRequest1.SymmetricKey);

            WrapSymmetricKeyRequest wrapRequest2 = fixture
                .Build<WrapSymmetricKeyRequest>()
                .With(x => x.AsymmetricKeyName, createResponse2.AsymmetricKeyDefinition.Name)
                .With(x => x.AsymmetricKeyVersion, createResponse2.AsymmetricKeyDefinition.Version)
                .Create();

            WrapSymmetricKeyResponse wrapResponse2 = await azureKeyVault.WrapSymmetricKeyAsync(wrapRequest2, default);

            wrapResponse2.Should().NotBeNull();
            wrapResponse2.WrappedSymmetricKey.Should().NotBeNull();
            wrapResponse2.WrappedSymmetricKey.Should().NotBeEmpty();
            wrapResponse2.WrappedSymmetricKey.Should().NotBeEquivalentTo(wrapRequest2.SymmetricKey);

            wrapResponse1.WrappedSymmetricKey.Should().NotBeEquivalentTo(wrapResponse2.WrappedSymmetricKey);

            RemoveAsymmetricKeyRequest removeRequest = fixture
                  .Build<RemoveAsymmetricKeyRequest>()
                  .With(x => x.Name, createRequest.Name)
                  .With(x => x.AwaitCompletion, false)
                  .Create();

            await azureKeyVault.RemoveAsymmetricKeyAsync(removeRequest, default);
        }

        [Fact]
        public async Task AzureKeyVault_GivenViewAsymmetricKeyDefinitionAsync_WhenValidInputs_ThenKeyDefinitionReturned()
        {
            IAsymmetricKeyVault azureKeyVault = m_AzureKeyVaultFixture.ServerServices.GetService<IAsymmetricKeyVault>();

            var fixture = new Fixture();
            CreateAsymmetricKeyRequest createRequest = fixture
                .Build<CreateAsymmetricKeyRequest>()
                .Create();

            CreateAsymmetricKeyResponse createResponse = await azureKeyVault.CreateAsymmetricKeyAsync(createRequest, default);

            createResponse.Should().NotBeNull();
            createResponse.AsymmetricKeyDefinition.Should().NotBeNull();
            createResponse.AsymmetricKeyDefinition.Id.Should().NotBeNullOrEmpty();
            createResponse.AsymmetricKeyDefinition.Name.Should().Be(createRequest.Name);
            createResponse.AsymmetricKeyDefinition.Version.Should().NotBeNullOrEmpty();
            createResponse.AsymmetricKeyDefinition.IsEnabled.Should().BeTrue();
            createResponse.AsymmetricKeyDefinition.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));

            ViewAsymmetricKeyDefinitionRequest viewRequest = fixture
                .Build<ViewAsymmetricKeyDefinitionRequest>()
                .With(x => x.Name, createResponse.AsymmetricKeyDefinition.Name)
                .With(x => x.Version, createResponse.AsymmetricKeyDefinition.Version)
                .Create();

            ViewAsymmetricKeyDefinitionResponse viewResponse = await azureKeyVault.ViewAsymmetricKeyDefinitionAsync(viewRequest, default);

            viewResponse.Should().NotBeNull();
            viewResponse.AsymmetricKeyDefinition.Should().NotBeNull();
            viewResponse.AsymmetricKeyDefinition.Id.Should().Be(createResponse.AsymmetricKeyDefinition.Id);
            viewResponse.AsymmetricKeyDefinition.Name.Should().Be(createResponse.AsymmetricKeyDefinition.Name);
            viewResponse.AsymmetricKeyDefinition.Version.Should().Be(createResponse.AsymmetricKeyDefinition.Version);
            viewResponse.AsymmetricKeyDefinition.IsEnabled.Should().Be(createResponse.AsymmetricKeyDefinition.IsEnabled);
            viewResponse.AsymmetricKeyDefinition.CreatedAt.Should().Be(createResponse.AsymmetricKeyDefinition.CreatedAt);

            RemoveAsymmetricKeyRequest removeRequest = fixture
                  .Build<RemoveAsymmetricKeyRequest>()
                  .With(x => x.Name, createRequest.Name)
                  .With(x => x.AwaitCompletion, false)
                  .Create();

            await azureKeyVault.RemoveAsymmetricKeyAsync(removeRequest, default);
        }

        [Fact]
        public async Task AzureKeyVault_GivenViewAsymmetricKeyDefinitionAsync_WhenInvalidName_ThenExceptionThrown()
        {
            IAsymmetricKeyVault azureKeyVault = m_AzureKeyVaultFixture.ServerServices.GetService<IAsymmetricKeyVault>();

            var fixture = new Fixture();
            CreateAsymmetricKeyRequest createRequest = fixture
                .Build<CreateAsymmetricKeyRequest>()
                .Create();

            CreateAsymmetricKeyResponse createResponse = await azureKeyVault.CreateAsymmetricKeyAsync(createRequest, default);

            createResponse.Should().NotBeNull();
            createResponse.AsymmetricKeyDefinition.Should().NotBeNull();
            createResponse.AsymmetricKeyDefinition.Id.Should().NotBeNullOrEmpty();
            createResponse.AsymmetricKeyDefinition.Name.Should().Be(createRequest.Name);
            createResponse.AsymmetricKeyDefinition.Version.Should().NotBeNullOrEmpty();
            createResponse.AsymmetricKeyDefinition.IsEnabled.Should().BeTrue();
            createResponse.AsymmetricKeyDefinition.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));

            ViewAsymmetricKeyDefinitionRequest viewRequest = fixture
                .Build<ViewAsymmetricKeyDefinitionRequest>()
                .With(x => x.Version, createResponse.AsymmetricKeyDefinition.Version)
                .Create();

            Func<Task> act = async () => await azureKeyVault.ViewAsymmetricKeyDefinitionAsync(viewRequest, default);

            await act.Should().ThrowAsync<Exception>();

            RemoveAsymmetricKeyRequest removeRequest = fixture
                  .Build<RemoveAsymmetricKeyRequest>()
                  .With(x => x.Name, createRequest.Name)
                  .With(x => x.AwaitCompletion, true)
                  .Create();

            await azureKeyVault.RemoveAsymmetricKeyAsync(removeRequest, default);
        }

        [Fact]
        public async Task AzureKeyVault_GivenViewAsymmetricKeyDefinitionAsync_WhenInvalidVersion_ThenExceptionThrown()
        {
            IAsymmetricKeyVault azureKeyVault = m_AzureKeyVaultFixture.ServerServices.GetService<IAsymmetricKeyVault>();

            var fixture = new Fixture();
            CreateAsymmetricKeyRequest createRequest = fixture
                .Build<CreateAsymmetricKeyRequest>()
                .Create();

            CreateAsymmetricKeyResponse createResponse = await azureKeyVault.CreateAsymmetricKeyAsync(createRequest, default);

            createResponse.Should().NotBeNull();
            createResponse.AsymmetricKeyDefinition.Should().NotBeNull();
            createResponse.AsymmetricKeyDefinition.Id.Should().NotBeNullOrEmpty();
            createResponse.AsymmetricKeyDefinition.Name.Should().Be(createRequest.Name);
            createResponse.AsymmetricKeyDefinition.Version.Should().NotBeNullOrEmpty();
            createResponse.AsymmetricKeyDefinition.IsEnabled.Should().BeTrue();
            createResponse.AsymmetricKeyDefinition.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));

            ViewAsymmetricKeyDefinitionRequest viewRequest = fixture
                .Build<ViewAsymmetricKeyDefinitionRequest>()
                .With(x => x.Name, createResponse.AsymmetricKeyDefinition.Name)
                .Create();

            Func<Task> act = async () => await azureKeyVault.ViewAsymmetricKeyDefinitionAsync(viewRequest, default);

            await act.Should().ThrowAsync<Exception>();

            RemoveAsymmetricKeyRequest removeRequest = fixture
                  .Build<RemoveAsymmetricKeyRequest>()
                  .With(x => x.Name, createRequest.Name)
                  .With(x => x.AwaitCompletion, true)
                  .Create();

            await azureKeyVault.RemoveAsymmetricKeyAsync(removeRequest, default);
        }

        [Fact]
        public async Task AzureKeyVault_GivenViewAsymmetricKeyDefinitionAsync_WhenKeyDeleted_ThenExceptionThrown()
        {
            IAsymmetricKeyVault azureKeyVault = m_AzureKeyVaultFixture.ServerServices.GetService<IAsymmetricKeyVault>();

            var fixture = new Fixture();
            CreateAsymmetricKeyRequest createRequest = fixture
                .Build<CreateAsymmetricKeyRequest>()
                .Create();

            CreateAsymmetricKeyResponse createResponse = await azureKeyVault.CreateAsymmetricKeyAsync(createRequest, default);

            createResponse.Should().NotBeNull();
            createResponse.AsymmetricKeyDefinition.Should().NotBeNull();
            createResponse.AsymmetricKeyDefinition.Id.Should().NotBeNullOrEmpty();
            createResponse.AsymmetricKeyDefinition.Name.Should().Be(createRequest.Name);
            createResponse.AsymmetricKeyDefinition.Version.Should().NotBeNullOrEmpty();
            createResponse.AsymmetricKeyDefinition.IsEnabled.Should().BeTrue();
            createResponse.AsymmetricKeyDefinition.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));

            RemoveAsymmetricKeyRequest removeRequest = fixture
                  .Build<RemoveAsymmetricKeyRequest>()
                  .With(x => x.Name, createRequest.Name)
                  .With(x => x.AwaitCompletion, true)
                  .Create();

            await azureKeyVault.RemoveAsymmetricKeyAsync(removeRequest, default);

            ViewAsymmetricKeyDefinitionRequest viewRequest = fixture
                .Build<ViewAsymmetricKeyDefinitionRequest>()
                .With(x => x.Name, createResponse.AsymmetricKeyDefinition.Name)
                .With(x => x.Version, createResponse.AsymmetricKeyDefinition.Version)
                .Create();

            Func<Task> act = async () => await azureKeyVault.ViewAsymmetricKeyDefinitionAsync(viewRequest, default);

            await act.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task AzureKeyVault_GivenViewAsymmetricKeyDefinitionAsync_WhenNoKeyExists_ThenExceptionThrown()
        {
            IAsymmetricKeyVault azureKeyVault = m_AzureKeyVaultFixture.ServerServices.GetService<IAsymmetricKeyVault>();

            var fixture = new Fixture();
            ViewAsymmetricKeyDefinitionRequest viewRequest = fixture
                .Build<ViewAsymmetricKeyDefinitionRequest>()
                .Create();

            Func<Task> act = async () => await azureKeyVault.ViewAsymmetricKeyDefinitionAsync(viewRequest, default);

            await act.Should().ThrowAsync<Exception>();
        }
    }
}
