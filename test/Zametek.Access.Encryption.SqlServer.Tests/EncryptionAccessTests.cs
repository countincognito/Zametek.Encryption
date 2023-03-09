using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;
using Zametek.Utility;
using Zametek.Utility.Cache;

namespace Zametek.Access.Encryption.Tests
{
    public class EncryptionAccessTests
    {
        private readonly EncryptionAccessFixture m_EncryptionAccessFixture;

        public EncryptionAccessTests()
        {
            m_EncryptionAccessFixture = new EncryptionAccessFixture();
        }

        [Fact]
        public async Task EncryptionAccess_GivenViewSymmetricKeyAsync_WhenSymmetricKeyNotRegistered_ThenNullReturned()
        {
            IEncryptionAccess encryptionAccess = m_EncryptionAccessFixture.ServerServices.GetService<IEncryptionAccess>();

            var fixture = new Fixture();
            ViewSymmetricKeyRequest viewRequest = fixture
                .Build<ViewSymmetricKeyRequest>()
                .Create();

            ViewSymmetricKeyResponse viewResponse = await encryptionAccess.ViewSymmetricKeyAsync(viewRequest, default);

            viewResponse.Should().BeNull();
        }

        [Fact]
        public async Task EncryptionAccess_GivenViewSymmetricKeyAsync_WhenSymmetricKeyRegistered_ThenSymmetricKeyReturned()
        {
            IEncryptionAccess encryptionAccess = m_EncryptionAccessFixture.ServerServices.GetService<IEncryptionAccess>();

            var fixture = new Fixture();
            RegisterSymmetricKeyRequest registerRequest = fixture
                .Build<RegisterSymmetricKeyRequest>()
                .Create();

            RegisterSymmetricKeyResponse registerResponse = await encryptionAccess.RegisterSymmetricKeyAsync(registerRequest, default);

            var viewRequest = new ViewSymmetricKeyRequest
            {
                SymmetricKeyId = registerResponse.SymmetricKeyId,
                AsymmetricKeyId = registerResponse.AsymmetricKeyId,
            };

            ViewSymmetricKeyResponse viewResponse = await encryptionAccess.ViewSymmetricKeyAsync(viewRequest, default);

            viewResponse.Should().NotBeNull();
            viewResponse.SymmetricKeyId.Should().Be(registerResponse.SymmetricKeyId);
            viewResponse.SymmetricKeyName.Should().Be(registerResponse.SymmetricKeyName);
            viewResponse.AsymmetricKeyId.Should().Be(registerResponse.AsymmetricKeyId);
            viewResponse.AsymmetricKeyName.Should().Be(registerResponse.AsymmetricKeyName);
            viewResponse.AsymmetricKeyVersion.Should().Be(registerResponse.AsymmetricKeyVersion);
            viewResponse.WrappedSymmetricKey.Should().BeEquivalentTo(registerResponse.WrappedSymmetricKey);
            viewResponse.InitializationVector.Should().BeEquivalentTo(registerResponse.InitializationVector);
            viewResponse.CreatedAt.Should().Be(registerResponse.CreatedAt);
            viewResponse.ModifiedAt.Should().Be(registerResponse.ModifiedAt);
            viewResponse.IsDisabled.Should().Be(registerResponse.IsDisabled);
        }

        [Fact]
        public async Task EncryptionAccess_GivenViewLatestSymmetricKeyAsync_WhenSymmetricKeyNotRegistered_ThenNullReturned()
        {
            IEncryptionAccess encryptionAccess = m_EncryptionAccessFixture.ServerServices.GetService<IEncryptionAccess>();

            var fixture = new Fixture();
            ViewLatestSymmetricKeyRequest viewRequest = fixture
                .Build<ViewLatestSymmetricKeyRequest>()
                .Create();

            ViewSymmetricKeyResponse viewResponse = await encryptionAccess.ViewLatestSymmetricKeyAsync(viewRequest, default);

            viewResponse.Should().BeNull();
        }

        [Fact]
        public async Task EncryptionAccess_GivenViewLatestSymmetricKeyAsync_WhenSymmetricKeyRegistered_ThenSymmetricKeyReturned()
        {
            IEncryptionAccess encryptionAccess = m_EncryptionAccessFixture.ServerServices.GetService<IEncryptionAccess>();

            var fixture = new Fixture();
            RegisterSymmetricKeyRequest registerRequest = fixture
                .Build<RegisterSymmetricKeyRequest>()
                .Create();

            RegisterSymmetricKeyResponse registerResponse = await encryptionAccess.RegisterSymmetricKeyAsync(registerRequest, default);

            var viewRequest = new ViewLatestSymmetricKeyRequest
            {
                SymmetricKeyId = registerResponse.SymmetricKeyId,
            };

            ViewSymmetricKeyResponse viewResponse = await encryptionAccess.ViewLatestSymmetricKeyAsync(viewRequest, default);

            viewResponse.Should().NotBeNull();
            viewResponse.SymmetricKeyId.Should().Be(registerResponse.SymmetricKeyId);
            viewResponse.SymmetricKeyName.Should().Be(registerResponse.SymmetricKeyName);
            viewResponse.AsymmetricKeyId.Should().Be(registerResponse.AsymmetricKeyId);
            viewResponse.AsymmetricKeyName.Should().Be(registerResponse.AsymmetricKeyName);
            viewResponse.AsymmetricKeyVersion.Should().Be(registerResponse.AsymmetricKeyVersion);
            viewResponse.WrappedSymmetricKey.Should().BeEquivalentTo(registerResponse.WrappedSymmetricKey);
            viewResponse.InitializationVector.Should().BeEquivalentTo(registerResponse.InitializationVector);
            viewResponse.CreatedAt.Should().Be(registerResponse.CreatedAt);
            viewResponse.ModifiedAt.Should().Be(registerResponse.ModifiedAt);
            viewResponse.IsDisabled.Should().Be(registerResponse.IsDisabled);
        }

        [Fact]
        public async Task EncryptionAccess_GivenViewLatestSymmetricKeyAsync_WhenTwoSymmetricKeysRegistered_ThenLatestSymmetricKeyReturned()
        {
            IEncryptionAccess encryptionAccess = m_EncryptionAccessFixture.ServerServices.GetService<IEncryptionAccess>();

            var fixture = new Fixture();
            RegisterSymmetricKeyRequest registerRequest1 = fixture
                .Build<RegisterSymmetricKeyRequest>()
                .Create();

            RegisterSymmetricKeyResponse registerResponse1 = await encryptionAccess.RegisterSymmetricKeyAsync(registerRequest1, default);

            RegisterSymmetricKeyRequest registerRequest2 = fixture
                .Build<RegisterSymmetricKeyRequest>()
                .With(x => x.SymmetricKeyId, registerRequest1.SymmetricKeyId)
                .Create();

            RegisterSymmetricKeyResponse registerResponse2 = await encryptionAccess.RegisterSymmetricKeyAsync(registerRequest2, default);

            var viewRequest = new ViewLatestSymmetricKeyRequest
            {
                SymmetricKeyId = registerResponse1.SymmetricKeyId,
            };

            ViewSymmetricKeyResponse viewResponse = await encryptionAccess.ViewLatestSymmetricKeyAsync(viewRequest, default);

            viewResponse.Should().NotBeNull();
            viewResponse.SymmetricKeyId.Should().Be(registerResponse2.SymmetricKeyId);
            viewResponse.SymmetricKeyName.Should().Be(registerResponse2.SymmetricKeyName);
            viewResponse.AsymmetricKeyId.Should().Be(registerResponse2.AsymmetricKeyId);
            viewResponse.AsymmetricKeyName.Should().Be(registerResponse2.AsymmetricKeyName);
            viewResponse.AsymmetricKeyVersion.Should().Be(registerResponse2.AsymmetricKeyVersion);
            viewResponse.WrappedSymmetricKey.Should().BeEquivalentTo(registerResponse2.WrappedSymmetricKey);
            viewResponse.InitializationVector.Should().BeEquivalentTo(registerResponse2.InitializationVector);
            viewResponse.CreatedAt.Should().Be(registerResponse2.CreatedAt);
            viewResponse.ModifiedAt.Should().Be(registerResponse2.ModifiedAt);
            viewResponse.IsDisabled.Should().Be(registerResponse2.IsDisabled);
        }

        [Fact]
        public async Task EncryptionAccess_GivenRegisterSymmetricKeyAsync_WhenSymmetricKeyRegistered_ThenValueIsCached()
        {
            IEncryptionAccess encryptionAccess = m_EncryptionAccessFixture.ServerServices.GetService<IEncryptionAccess>();
            ICacheUtility cacheUtility = m_EncryptionAccessFixture.ServerServices.GetService<ICacheUtility>();

            var fixture = new Fixture();
            RegisterSymmetricKeyRequest registerRequest = fixture
                .Build<RegisterSymmetricKeyRequest>()
                .Create();

            RegisterSymmetricKeyResponse registerResponse = await encryptionAccess.RegisterSymmetricKeyAsync(registerRequest, default);

            string cacheKey = EncryptionAccess.GetCacheKey(registerResponse.SymmetricKeyId);

            SymmetricKey symmetricKey = await cacheUtility.GetAsync<SymmetricKey>(cacheKey, default);

            symmetricKey.WrappedSymmetricKey.Should().NotBeEmpty();
            symmetricKey.WrappedSymmetricKey.Should().BeEquivalentTo(registerRequest.WrappedSymmetricKey.ByteArrayToBase64String());
            symmetricKey.WrappedSymmetricKey.Should().BeEquivalentTo(registerResponse.WrappedSymmetricKey.ByteArrayToBase64String());
        }

        [Fact]
        public async Task EncryptionAccess_GivenViewSymmetricKeyAsync_WhenCacheIsCleared_ThenSymmetricKeyReturned()
        {
            IEncryptionAccess encryptionAccess = m_EncryptionAccessFixture.ServerServices.GetService<IEncryptionAccess>();
            ICacheUtility cacheUtility = m_EncryptionAccessFixture.ServerServices.GetService<ICacheUtility>();

            var fixture = new Fixture();
            RegisterSymmetricKeyRequest registerRequest = fixture
                .Build<RegisterSymmetricKeyRequest>()
                .Create();

            RegisterSymmetricKeyResponse registerResponse = await encryptionAccess.RegisterSymmetricKeyAsync(registerRequest, default);

            string cacheKey = EncryptionAccess.GetCacheKey(registerResponse.SymmetricKeyId);

            await cacheUtility.DeleteAsync(cacheKey, default);

            SymmetricKey symmetricKey = await cacheUtility.GetAsync<SymmetricKey>(cacheKey, default);

            symmetricKey.Should().BeNull();

            var viewRequest = new ViewSymmetricKeyRequest
            {
                SymmetricKeyId = registerResponse.SymmetricKeyId,
                AsymmetricKeyId = registerResponse.AsymmetricKeyId,
            };

            ViewSymmetricKeyResponse viewResponse = await encryptionAccess.ViewSymmetricKeyAsync(viewRequest, default);

            viewResponse.Should().NotBeNull();
            viewResponse.SymmetricKeyId.Should().Be(registerResponse.SymmetricKeyId);
            viewResponse.SymmetricKeyName.Should().Be(registerResponse.SymmetricKeyName);
            viewResponse.AsymmetricKeyId.Should().Be(registerResponse.AsymmetricKeyId);
            viewResponse.AsymmetricKeyName.Should().Be(registerResponse.AsymmetricKeyName);
            viewResponse.AsymmetricKeyVersion.Should().Be(registerResponse.AsymmetricKeyVersion);
            viewResponse.WrappedSymmetricKey.Should().BeEquivalentTo(registerResponse.WrappedSymmetricKey);
            viewResponse.InitializationVector.Should().BeEquivalentTo(registerResponse.InitializationVector);
            // Use BeCloseTo due to limitations of SQLite.
            viewResponse.CreatedAt.Should().BeCloseTo(registerResponse.CreatedAt, TimeSpan.FromSeconds(1));
            viewResponse.ModifiedAt.Should().BeCloseTo(registerResponse.ModifiedAt, TimeSpan.FromSeconds(1));
            viewResponse.IsDisabled.Should().Be(registerResponse.IsDisabled);
        }

        [Fact]
        public async Task EncryptionAccess_GivenViewLatestSymmetricKeyAsync_WhenCacheIsCleared_ThenSymmetricKeyReturned()
        {
            IEncryptionAccess encryptionAccess = m_EncryptionAccessFixture.ServerServices.GetService<IEncryptionAccess>();
            ICacheUtility cacheUtility = m_EncryptionAccessFixture.ServerServices.GetService<ICacheUtility>();

            var fixture = new Fixture();
            RegisterSymmetricKeyRequest registerRequest = fixture
                .Build<RegisterSymmetricKeyRequest>()
                .Create();

            RegisterSymmetricKeyResponse registerResponse = await encryptionAccess.RegisterSymmetricKeyAsync(registerRequest, default);

            string cacheKey = EncryptionAccess.GetCacheKey(registerResponse.SymmetricKeyId);

            await cacheUtility.DeleteAsync(cacheKey, default);

            SymmetricKey symmetricKey = await cacheUtility.GetAsync<SymmetricKey>(cacheKey, default);

            symmetricKey.Should().BeNull();

            var viewRequest = new ViewLatestSymmetricKeyRequest
            {
                SymmetricKeyId = registerResponse.SymmetricKeyId,
            };

            ViewSymmetricKeyResponse viewResponse = await encryptionAccess.ViewLatestSymmetricKeyAsync(viewRequest, default);

            viewResponse.Should().NotBeNull();
            viewResponse.SymmetricKeyId.Should().Be(registerResponse.SymmetricKeyId);
            viewResponse.SymmetricKeyName.Should().Be(registerResponse.SymmetricKeyName);
            viewResponse.AsymmetricKeyId.Should().Be(registerResponse.AsymmetricKeyId);
            viewResponse.AsymmetricKeyName.Should().Be(registerResponse.AsymmetricKeyName);
            viewResponse.AsymmetricKeyVersion.Should().Be(registerResponse.AsymmetricKeyVersion);
            viewResponse.WrappedSymmetricKey.Should().BeEquivalentTo(registerResponse.WrappedSymmetricKey);
            viewResponse.InitializationVector.Should().BeEquivalentTo(registerResponse.InitializationVector);
            // Use BeCloseTo due to limitations of SQLite.
            viewResponse.CreatedAt.Should().BeCloseTo(registerResponse.CreatedAt, TimeSpan.FromSeconds(1));
            viewResponse.ModifiedAt.Should().BeCloseTo(registerResponse.ModifiedAt, TimeSpan.FromSeconds(1));
            viewResponse.IsDisabled.Should().Be(registerResponse.IsDisabled);
        }

        [Fact]
        public async Task EncryptionAccess_GivenUpdateSymmetricKeyAsync_WhenSymmetricKeyNotRegistered_ThenNullReturned()
        {
            IEncryptionAccess encryptionAccess = m_EncryptionAccessFixture.ServerServices.GetService<IEncryptionAccess>();

            var fixture = new Fixture();
            UpdateSymmetricKeyRequest updateRequest = fixture
                .Build<UpdateSymmetricKeyRequest>()
                .Create();

            UpdateSymmetricKeyResponse updateResponse = await encryptionAccess.UpdateSymmetricKeyAsync(updateRequest, default);

            updateResponse.Should().BeNull();
        }

        [Fact]
        public async Task EncryptionAccess_GivenUpdateSymmetricKeyAsync_WhenSymmetricKeyRegistered_ThenSymmetricKeyUpdated()
        {
            IEncryptionAccess encryptionAccess = m_EncryptionAccessFixture.ServerServices.GetService<IEncryptionAccess>();

            var fixture = new Fixture();
            RegisterSymmetricKeyRequest registerRequest = fixture
                .Build<RegisterSymmetricKeyRequest>()
                .Create();

            RegisterSymmetricKeyResponse registerResponse = await encryptionAccess.RegisterSymmetricKeyAsync(registerRequest, default);

            UpdateSymmetricKeyRequest updateRequest = fixture
                .Build<UpdateSymmetricKeyRequest>()
                .With(x => x.SymmetricKeyId, registerResponse.SymmetricKeyId)
                .With(x => x.AsymmetricKeyId, registerResponse.AsymmetricKeyId)
                .Create();

            UpdateSymmetricKeyResponse updateResponse = await encryptionAccess.UpdateSymmetricKeyAsync(updateRequest, default);

            updateResponse.Should().NotBeNull();
            updateResponse.SymmetricKeyId.Should().Be(registerResponse.SymmetricKeyId);
            updateResponse.SymmetricKeyName.Should().Be(updateRequest.SymmetricKeyName);
            updateResponse.AsymmetricKeyId.Should().Be(updateRequest.AsymmetricKeyId);
            updateResponse.AsymmetricKeyName.Should().Be(updateRequest.AsymmetricKeyName);
            updateResponse.AsymmetricKeyVersion.Should().Be(updateRequest.AsymmetricKeyVersion);
            updateResponse.WrappedSymmetricKey.Should().BeEquivalentTo(updateRequest.WrappedSymmetricKey);
            updateResponse.InitializationVector.Should().BeEquivalentTo(updateRequest.InitializationVector);
            // Use BeCloseTo due to limitations of SQLite.
            updateResponse.CreatedAt.Should().BeCloseTo(registerResponse.CreatedAt, TimeSpan.FromSeconds(1));
            updateResponse.ModifiedAt.Should().BeAfter(registerResponse.ModifiedAt);
            updateResponse.IsDisabled.Should().Be(updateRequest.IsDisabled);
        }

        [Fact]
        public async Task EncryptionAccess_GivenUpdateSymmetricKeyAsync_WhenSymmetricKeyRegistered_ThenCacheIsUpdated()
        {
            IEncryptionAccess encryptionAccess = m_EncryptionAccessFixture.ServerServices.GetService<IEncryptionAccess>();
            ICacheUtility cacheUtility = m_EncryptionAccessFixture.ServerServices.GetService<ICacheUtility>();

            var fixture = new Fixture();
            RegisterSymmetricKeyRequest registerRequest = fixture
                .Build<RegisterSymmetricKeyRequest>()
                .Create();

            RegisterSymmetricKeyResponse registerResponse = await encryptionAccess.RegisterSymmetricKeyAsync(registerRequest, default);

            UpdateSymmetricKeyRequest updateRequest = fixture
                .Build<UpdateSymmetricKeyRequest>()
                .With(x => x.SymmetricKeyId, registerResponse.SymmetricKeyId)
                .With(x => x.AsymmetricKeyId, registerResponse.AsymmetricKeyId)
                .Create();

            UpdateSymmetricKeyResponse updateResponse = await encryptionAccess.UpdateSymmetricKeyAsync(updateRequest, default);

            string cacheKey = EncryptionAccess.GetCacheKey(registerResponse.SymmetricKeyId);

            SymmetricKey symmetricKey = await cacheUtility.GetAsync<SymmetricKey>(cacheKey, default);

            symmetricKey.WrappedSymmetricKey.Should().NotBeEmpty();
            symmetricKey.WrappedSymmetricKey.Should().BeEquivalentTo(updateRequest.WrappedSymmetricKey.ByteArrayToBase64String());
            symmetricKey.WrappedSymmetricKey.Should().BeEquivalentTo(updateResponse.WrappedSymmetricKey.ByteArrayToBase64String());
        }

        [Fact]
        public async Task EncryptionAccess_GivenUpdateSymmetricKeyAsync_WhenCacheIsCleared_ThenSymmetricKeyUpdated()
        {
            IEncryptionAccess encryptionAccess = m_EncryptionAccessFixture.ServerServices.GetService<IEncryptionAccess>();
            ICacheUtility cacheUtility = m_EncryptionAccessFixture.ServerServices.GetService<ICacheUtility>();

            var fixture = new Fixture();
            RegisterSymmetricKeyRequest registerRequest = fixture
                .Build<RegisterSymmetricKeyRequest>()
                .Create();

            RegisterSymmetricKeyResponse registerResponse = await encryptionAccess.RegisterSymmetricKeyAsync(registerRequest, default);

            string cacheKey = EncryptionAccess.GetCacheKey(registerResponse.SymmetricKeyId);

            await cacheUtility.DeleteAsync(cacheKey, default);

            SymmetricKey symmetricKey = await cacheUtility.GetAsync<SymmetricKey>(cacheKey, default);

            symmetricKey.Should().BeNull();

            UpdateSymmetricKeyRequest updateRequest = fixture
                .Build<UpdateSymmetricKeyRequest>()
                .With(x => x.SymmetricKeyId, registerResponse.SymmetricKeyId)
                .With(x => x.AsymmetricKeyId, registerResponse.AsymmetricKeyId)
                .Create();

            UpdateSymmetricKeyResponse updateResponse = await encryptionAccess.UpdateSymmetricKeyAsync(updateRequest, default);

            updateResponse.Should().NotBeNull();
            updateResponse.SymmetricKeyId.Should().Be(registerResponse.SymmetricKeyId);
            updateResponse.SymmetricKeyName.Should().Be(updateRequest.SymmetricKeyName);
            updateResponse.AsymmetricKeyId.Should().Be(updateRequest.AsymmetricKeyId);
            updateResponse.AsymmetricKeyName.Should().Be(updateRequest.AsymmetricKeyName);
            updateResponse.AsymmetricKeyVersion.Should().Be(updateRequest.AsymmetricKeyVersion);
            updateResponse.WrappedSymmetricKey.Should().BeEquivalentTo(updateRequest.WrappedSymmetricKey);
            updateResponse.InitializationVector.Should().BeEquivalentTo(updateRequest.InitializationVector);
            // Use BeCloseTo due to limitations of SQLite.
            updateResponse.CreatedAt.Should().BeCloseTo(registerResponse.CreatedAt, TimeSpan.FromSeconds(1));
            updateResponse.ModifiedAt.Should().BeAfter(registerResponse.ModifiedAt);
            updateResponse.IsDisabled.Should().Be(updateRequest.IsDisabled);
        }

        [Fact]
        public async Task EncryptionAccess_GivenRemoveSymmetricKeyAsync_WhenSymmetricKeyNotRegistered_ThenFalseReturned()
        {
            IEncryptionAccess encryptionAccess = m_EncryptionAccessFixture.ServerServices.GetService<IEncryptionAccess>();

            var fixture = new Fixture();
            RemoveSymmetricKeyRequest removeRequest = fixture
                .Build<RemoveSymmetricKeyRequest>()
                .Create();

            bool removeResponse = await encryptionAccess.RemoveSymmetricKeyAsync(removeRequest, default);

            removeResponse.Should().BeFalse();
        }

        [Fact]
        public async Task EncryptionAccess_GivenRemoveSymmetricKeyAsync_WhenSymmetricKeyRegistered_ThenTrueReturned()
        {
            IEncryptionAccess encryptionAccess = m_EncryptionAccessFixture.ServerServices.GetService<IEncryptionAccess>();

            var fixture = new Fixture();
            RegisterSymmetricKeyRequest registerRequest = fixture
                .Build<RegisterSymmetricKeyRequest>()
                .Create();

            RegisterSymmetricKeyResponse registerResponse = await encryptionAccess.RegisterSymmetricKeyAsync(registerRequest, default);

            var removeRequest = new RemoveSymmetricKeyRequest
            {
                SymmetricKeyId = registerResponse.SymmetricKeyId,
                AsymmetricKeyId = registerResponse.AsymmetricKeyId,
            };

            bool removeResponse = await encryptionAccess.RemoveSymmetricKeyAsync(removeRequest, default);

            removeResponse.Should().BeTrue();

            var viewRequest = new ViewSymmetricKeyRequest
            {
                SymmetricKeyId = registerResponse.SymmetricKeyId,
                AsymmetricKeyId = registerResponse.AsymmetricKeyId,
            };

            ViewSymmetricKeyResponse viewResponse = await encryptionAccess.ViewSymmetricKeyAsync(viewRequest, default);

            viewResponse.Should().BeNull();
        }

        [Fact]
        public async Task EncryptionAccess_GivenRemoveSymmetricKeyAsync_WhenSymmetricKeyRegistered_ThenCacheIsCleared()
        {
            IEncryptionAccess encryptionAccess = m_EncryptionAccessFixture.ServerServices.GetService<IEncryptionAccess>();
            ICacheUtility cacheUtility = m_EncryptionAccessFixture.ServerServices.GetService<ICacheUtility>();

            var fixture = new Fixture();
            RegisterSymmetricKeyRequest registerRequest = fixture
                .Build<RegisterSymmetricKeyRequest>()
                .Create();

            RegisterSymmetricKeyResponse registerResponse = await encryptionAccess.RegisterSymmetricKeyAsync(registerRequest, default);

            var removeRequest = new RemoveSymmetricKeyRequest
            {
                SymmetricKeyId = registerResponse.SymmetricKeyId,
                AsymmetricKeyId = registerResponse.AsymmetricKeyId,
            };

            bool removeResponse = await encryptionAccess.RemoveSymmetricKeyAsync(removeRequest, default);

            removeResponse.Should().BeTrue();

            string cacheKey = EncryptionAccess.GetCacheKey(registerResponse.SymmetricKeyId);

            SymmetricKey symmetricKey = await cacheUtility.GetAsync<SymmetricKey>(cacheKey, default);

            symmetricKey.Should().BeNull();
        }

        [Fact]
        public async Task EncryptionAccess_GivenRemoveSymmetricKeyAsync_WhenCacheIsCleared_ThenTrueReturned()
        {
            IEncryptionAccess encryptionAccess = m_EncryptionAccessFixture.ServerServices.GetService<IEncryptionAccess>();
            ICacheUtility cacheUtility = m_EncryptionAccessFixture.ServerServices.GetService<ICacheUtility>();

            var fixture = new Fixture();
            RegisterSymmetricKeyRequest registerRequest = fixture
                .Build<RegisterSymmetricKeyRequest>()
                .Create();

            RegisterSymmetricKeyResponse registerResponse = await encryptionAccess.RegisterSymmetricKeyAsync(registerRequest, default);

            string cacheKey = EncryptionAccess.GetCacheKey(registerResponse.SymmetricKeyId);

            await cacheUtility.DeleteAsync(cacheKey, default);

            var removeRequest = new RemoveSymmetricKeyRequest
            {
                SymmetricKeyId = registerResponse.SymmetricKeyId,
                AsymmetricKeyId = registerResponse.AsymmetricKeyId,
            };

            bool removeResponse = await encryptionAccess.RemoveSymmetricKeyAsync(removeRequest, default);

            removeResponse.Should().BeTrue();

            var viewRequest = new ViewSymmetricKeyRequest
            {
                SymmetricKeyId = registerResponse.SymmetricKeyId,
                AsymmetricKeyId = registerResponse.AsymmetricKeyId,
            };

            ViewSymmetricKeyResponse viewResponse = await encryptionAccess.ViewSymmetricKeyAsync(viewRequest, default);

            viewResponse.Should().BeNull();
        }
    }
}
