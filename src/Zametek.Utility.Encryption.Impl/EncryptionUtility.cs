using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Zametek.Access.Encryption;
using Zametek.Utility.Cache;
using Zametek.Utility.Logging;

namespace Zametek.Utility.Encryption
{
    [DiagnosticLogging(LogActive.On)]
    public class EncryptionUtility
        : IEncryptionUtility
    {
        #region Fields

        private readonly IAsymmetricKeyVault m_AsymmetricKeyVault;
        private readonly ISymmetricKeyEncryption m_SymmetricKeyEncryption;
        private readonly IEncryptionAccess m_EncryptionAccess;
        private readonly ICacheUtility m_CacheUtility;
        private readonly ILogger m_Logger;

        private readonly DistributedCacheEntryOptions m_CacheOptions;

        private const string c_UnwrappedSymmetricKeyPrefix = @"UnwrappedSymmetricKey";

        #endregion

        #region Ctors

        public EncryptionUtility(
            IAsymmetricKeyVault asymmetricKeyVault,
            ISymmetricKeyEncryption symmetricKeyEncryption,
            IEncryptionAccess encryptionAccess,
            ICacheUtility cacheUtility,
            IOptions<CacheOptions> cacheOptions,
            ILogger logger)
        {
            m_AsymmetricKeyVault = asymmetricKeyVault ?? throw new ArgumentNullException(nameof(asymmetricKeyVault));
            m_SymmetricKeyEncryption = symmetricKeyEncryption ?? throw new ArgumentNullException(nameof(symmetricKeyEncryption));
            m_EncryptionAccess = encryptionAccess ?? throw new ArgumentNullException(nameof(encryptionAccess));
            m_CacheUtility = cacheUtility ?? throw new ArgumentNullException(nameof(cacheUtility));
            CacheOptions options = cacheOptions?.Value ?? throw new ArgumentNullException(nameof(cacheOptions));
            m_Logger = logger ?? throw new ArgumentNullException(nameof(logger));

            m_CacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(options.AbsoluteExpirationInMinutes),
            };
        }

        #endregion

        #region Private Members

        private async Task SetCachedSymmetricKeyAsync(
            SymmetricKey symmetricKey,
            CancellationToken ct)
        {
            if (symmetricKey is null)
            {
                throw new ArgumentNullException(nameof(symmetricKey));
            }

            try
            {
                var setCachedValueRequest = new SetCachedValueRequest
                {
                    Key = GetCacheKey(symmetricKey.SymmetricKeyId),
                    Data = symmetricKey.ObjectToByteArray(),
                    Options = m_CacheOptions,
                };

                m_Logger.Information(@"Attempting to cache SymmetricKey {@SymmetricKey} with request {@SetCachedValueRequest}.", symmetricKey, setCachedValueRequest);

                await m_CacheUtility
                    .SetCachedValueAsync(setCachedValueRequest, ct)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                m_Logger.Error(ex, @"Unable to cache SymmetricKey {@SymmetricKey}.", symmetricKey);
                throw;
            }
        }

        private async Task<SymmetricKey> GetCachedSymmetricKeyAsync(
            Guid symmetricKeyId,
            CancellationToken ct)
        {
            if (symmetricKeyId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(symmetricKeyId));
            }

            SymmetricKey symmetricKey;
            try
            {
                var getCachedValueRequest = new GetCachedValueRequest
                {
                    Key = GetCacheKey(symmetricKeyId),
                };

                m_Logger.Information(@"Attempting to retrieve cached SymmetricKey {@SymmetricKeyId} with request {@GetCachedValueRequest}.", symmetricKeyId, getCachedValueRequest);

                GetCachedValueResponse getCachedValueResponse = await m_CacheUtility
                    .GetCachedValueAsync(getCachedValueRequest, ct)
                    .ConfigureAwait(false);

                symmetricKey = getCachedValueResponse?.Data?.ByteArrayToObject<SymmetricKey>();
            }
            catch (Exception ex)
            {
                m_Logger.Error(ex, @"Unable to retrieve cached SymmetricKey {@SymmetricKeyId}.", symmetricKeyId);
                throw;
            }

            return symmetricKey;
        }

        private async Task DeleteCachedSymmetricKeyAsync(
            Guid symmetricKeyId,
            CancellationToken ct)
        {
            if (symmetricKeyId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(symmetricKeyId));
            }

            try
            {
                var deleteCachedValueRequest = new DeleteCachedValueRequest
                {
                    Key = GetCacheKey(symmetricKeyId),
                };

                m_Logger.Information(@"Attempting to remove cached SymmetricKey with request {@DeleteCachedValueRequest}.", deleteCachedValueRequest);

                await m_CacheUtility
                    .DeleteCachedValueAsync(deleteCachedValueRequest, ct)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                m_Logger.Error(ex, @"Unable to remove cached SymmetricKey {@SymmetricKeyId}.", symmetricKeyId);
                throw;
            }
        }

        private async Task<SymmetricKey> WrapSymmetricWithNewAsymmetricKeyAsync(
            Guid symmtricKeyId,
            byte[] symmetricKey,
            byte[] iv,
            string symmetricKeyName,
            string asymmetricKeyName,
            CancellationToken ct)
        {
            if (symmtricKeyId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(symmtricKeyId));
            }
            if (symmetricKey is null)
            {
                throw new ArgumentNullException(nameof(symmetricKey));
            }
            if (iv is null)
            {
                throw new ArgumentNullException(nameof(iv));
            }
            if (string.IsNullOrWhiteSpace(symmetricKeyName))
            {
                throw new ArgumentNullException(nameof(symmetricKeyName));
            }
            if (string.IsNullOrWhiteSpace(asymmetricKeyName))
            {
                throw new ArgumentNullException(nameof(asymmetricKeyName));
            }

            // The name should be unique within the vault.

            var createAsymmetricKeyRequest = new CreateAsymmetricKeyRequest
            {
                Name = asymmetricKeyName,
            };

            // Generate asymmetric (e.g. RSA) key.

            m_Logger.Information(@"Attempting to generate asymmetric key with request {@CreateAsymmetricKeyRequest}.", createAsymmetricKeyRequest);

            CreateAsymmetricKeyResponse createAsymmetricKeyResponse = await m_AsymmetricKeyVault
                .CreateAsymmetricKeyAsync(createAsymmetricKeyRequest, ct)
                .ConfigureAwait(false);

            AsymmetricKeyDefinition asymmetricKeyDefinition = createAsymmetricKeyResponse?.AsymmetricKeyDefinition;

            if (asymmetricKeyDefinition is null)
            {
                m_Logger.Warning(@"Unable to generate asymmetric key with request {@CreateAsymmetricKeyRequest}.", createAsymmetricKeyRequest);
                return null;
            }

            Debug.Assert(string.Equals(asymmetricKeyDefinition.Name, asymmetricKeyName, StringComparison.InvariantCulture));

            // Wrap symmetric key with the asymmetric key.

            var wrapSymmetricKeyRequest = new WrapSymmetricKeyRequest
            {
                AsymmetricKeyName = asymmetricKeyDefinition.Name,
                AsymmetricKeyVersion = asymmetricKeyDefinition.Version,
                SymmetricKey = symmetricKey,
            };

            m_Logger.Information(@"Attempting to wrap symmetric key with request {@WrapSymmetricKeyRequest}.", wrapSymmetricKeyRequest);

            WrapSymmetricKeyResponse wrapSymmetricKeyResponse = await m_AsymmetricKeyVault
                .WrapSymmetricKeyAsync(wrapSymmetricKeyRequest, ct)
                .ConfigureAwait(false);

            if (wrapSymmetricKeyResponse is null)
            {
                m_Logger.Warning(@"Unable to wrap symmetric key with request {@WrapSymmetricKeyRequest}.", wrapSymmetricKeyRequest);
                return null;
            }

            return new SymmetricKey
            {
                SymmetricKeyId = symmtricKeyId,
                SymmetricKeyName = symmetricKeyName,
                AsymmetricKeyId = asymmetricKeyDefinition.Id,
                AsymmetricKeyName = asymmetricKeyDefinition.Name,
                AsymmetricKeyVersion = asymmetricKeyDefinition.Version,
                WrappedSymmetricKey = wrapSymmetricKeyResponse.WrappedSymmetricKey,
                UnwrappedSymmetricKey = symmetricKey,
                InitializationVector = iv,
            };
        }

        private async Task<SymmetricKey> GetKeyDataAsync(
            Guid id,
            CancellationToken ct)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(id));
            }

            // Retrieve symmetric key.

            var viewLatestSymmetricKeyRequest = new ViewLatestSymmetricKeyRequest
            {
                SymmetricKeyId = id,
            };

            m_Logger.Information(@"Attempting to view symmetric key with request {@ViewLatestSymmetricKeyRequest}.", viewLatestSymmetricKeyRequest);

            ViewSymmetricKeyResponse viewSymmetricKeyResponse = await m_EncryptionAccess
                .ViewLatestSymmetricKeyAsync(viewLatestSymmetricKeyRequest, ct)
                .ConfigureAwait(false);

            if (viewSymmetricKeyResponse is null)
            {
                m_Logger.Warning(@"Unable to view symmetric key with request {@ViewLatestSymmetricKeyRequest}.", viewLatestSymmetricKeyRequest);
                return null;
            }

            if (viewSymmetricKeyResponse.IsDisabled)
            {
                m_Logger.Warning(@"Symmetric key for request {@ViewLatestSymmetricKeyRequest} is disabled.", viewLatestSymmetricKeyRequest);
                return null;
            }

            // Unwrap symmetric key.

            var unwrapSymmetricKeyRequest = new UnwrapSymmetricKeyRequest
            {
                AsymmetricKeyName = viewSymmetricKeyResponse.AsymmetricKeyName,
                AsymmetricKeyVersion = viewSymmetricKeyResponse.AsymmetricKeyVersion,
                WrappedSymmetricKey = viewSymmetricKeyResponse.WrappedSymmetricKey,
            };

            m_Logger.Information(@"Attempting to unwrap symmetric key with request {@UnwrapSymmetricKeyRequest}.", unwrapSymmetricKeyRequest);

            UnwrapSymmetricKeyResponse unwrapSymmetricKeyResponse = await m_AsymmetricKeyVault
                .UnwrapSymmetricKeyAsync(unwrapSymmetricKeyRequest, ct)
                .ConfigureAwait(false);

            if (unwrapSymmetricKeyResponse is null)
            {
                m_Logger.Warning(@"Unable to unwrap symmetric key with request {@UnwrapSymmetricKeyRequest}.", unwrapSymmetricKeyRequest);
                return null;
            }

            return new SymmetricKey
            {
                SymmetricKeyId = viewSymmetricKeyResponse.SymmetricKeyId,
                SymmetricKeyName = viewSymmetricKeyResponse.SymmetricKeyName,
                AsymmetricKeyId = viewSymmetricKeyResponse.AsymmetricKeyId,
                AsymmetricKeyName = viewSymmetricKeyResponse.AsymmetricKeyName,
                AsymmetricKeyVersion = viewSymmetricKeyResponse.AsymmetricKeyVersion,
                WrappedSymmetricKey = viewSymmetricKeyResponse.WrappedSymmetricKey,
                UnwrappedSymmetricKey = unwrapSymmetricKeyResponse.SymmetricKey,
                InitializationVector = viewSymmetricKeyResponse.InitializationVector,
            };
        }

        #endregion

        #region Public Members

        public static string GetCacheKey(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return $@"{c_UnwrappedSymmetricKeyPrefix}_{id.ToFlatString()}";
        }

        #endregion

        #region IEncryptionUtility Members

        public async Task<CreateKeysResponse> CreateKeysAsync(
            CreateKeysRequest request,
            CancellationToken ct)
        {
            await CreateKeysRequestValidator
                .ValidateAndThrowAsync(request, ct)
                .ConfigureAwait(false);

            // Generate symmetric (AES) key and Initialization Vector.

            byte[] symmetrickey = EncryptionHelper.Generate256BitKey();
            byte[] iv = EncryptionHelper.Generate128BitInitializationVector();

            // Wrap the symmetric key in a new asymmetric key.

            m_Logger.Information(@"Attempting to wrap symmetric key for request {@CreateKeysRequest}.", request);

            SymmetricKey newSymmetricKey =
                await WrapSymmetricWithNewAsymmetricKeyAsync(
                    Guid.NewGuid(),
                    symmetrickey,
                    iv,
                    request.SymmetricKeyName,
                    request.AsymmetricKeyName,
                    ct)
                .ConfigureAwait(false);

            if (newSymmetricKey is null)
            {
                m_Logger.Warning(@"Unable to wrap symmetric key for request {@CreateKeysRequest}.", request);
                return null;
            }

            // Store the wrapped symmetric key and the asymmetric key details.

            var registerSymmetricKeyRequest = new RegisterSymmetricKeyRequest
            {
                SymmetricKeyId = newSymmetricKey.SymmetricKeyId,
                SymmetricKeyName = newSymmetricKey.SymmetricKeyName,
                AsymmetricKeyId = newSymmetricKey.AsymmetricKeyId,
                AsymmetricKeyName = newSymmetricKey.AsymmetricKeyName,
                AsymmetricKeyVersion = newSymmetricKey.AsymmetricKeyVersion,
                WrappedSymmetricKey = newSymmetricKey.WrappedSymmetricKey,
                InitializationVector = newSymmetricKey.InitializationVector,
            };

            m_Logger.Information(@"Attempting to register symmetric key for request {@RegisterSymmetricKeyRequest}.", registerSymmetricKeyRequest);

            RegisterSymmetricKeyResponse registerSymmetricKeyResponse = await m_EncryptionAccess
                .RegisterSymmetricKeyAsync(registerSymmetricKeyRequest, ct)
                .ConfigureAwait(false);

            if (registerSymmetricKeyResponse is null)
            {
                m_Logger.Warning(@"Unable to register symmetric key for request {@RegisterSymmetricKeyRequest}.", registerSymmetricKeyRequest);
                return null;
            }

            // Cache.

            await SetCachedSymmetricKeyAsync(newSymmetricKey, ct)
                .ConfigureAwait(false);

            var symmetricKeyDefinition = new SymmetricKeyDefinition
            {
                Id = registerSymmetricKeyResponse.SymmetricKeyId,
                Name = registerSymmetricKeyResponse.SymmetricKeyName,
                IsEnabled = !registerSymmetricKeyResponse.IsDisabled,
                CreatedAt = registerSymmetricKeyResponse.CreatedAt,
            };

            var asymmetricKeyDefinition = new AsymmetricKeyDefinition
            {
                Id = registerSymmetricKeyResponse.AsymmetricKeyId,
                Name = registerSymmetricKeyResponse.AsymmetricKeyName,
                Version = registerSymmetricKeyResponse.AsymmetricKeyVersion,
                IsEnabled = !registerSymmetricKeyResponse.IsDisabled,
                CreatedAt = registerSymmetricKeyResponse.CreatedAt,
            };

            return new CreateKeysResponse
            {
                SymmetricKeyDefinition = symmetricKeyDefinition,
                AsymmetricKeyDefinition = asymmetricKeyDefinition,
            };
        }

        public async Task<EncryptResponse> EncryptAsync(
            EncryptRequest request,
            CancellationToken ct)
        {
            await EncryptRequestValidator
                .ValidateAndThrowAsync(request, ct)
                .ConfigureAwait(false);

            // Get symmetric key.

            SymmetricKey symmetricKey = await GetCachedSymmetricKeyAsync(request.SymmetricKeyId, ct)
                .ConfigureAwait(false);

            if (symmetricKey is null)
            {
                m_Logger.Information(@"Attempting to retrieve symmetric key data for request {@EncryptRequest}.", request);

                symmetricKey = await GetKeyDataAsync(request.SymmetricKeyId, ct)
                    .ConfigureAwait(false);
            }

            if (symmetricKey is null)
            {
                m_Logger.Warning(@"Unable to retrieve symmetric key data for request {@EncryptRequest}.", request);
                return null;
            }

            // Encrypt and return the data.

            m_Logger.Information(@"Attempting to encrypt data for request {@EncryptRequest}.", request);

            byte[] encryptedData = await m_SymmetricKeyEncryption
                .EncryptAsync(request.Data, symmetricKey.UnwrappedSymmetricKey, symmetricKey.InitializationVector, ct)
                .ConfigureAwait(false);

            return new EncryptResponse
            {
                EncryptedData = encryptedData,
            };
        }

        public async Task<DecryptResponse> DecryptAsync(
            DecryptRequest request,
            CancellationToken ct)
        {
            await DecryptRequestValidator
                .ValidateAndThrowAsync(request, ct)
                .ConfigureAwait(false);

            // Get symmetric key.

            SymmetricKey symmetricKey = await GetCachedSymmetricKeyAsync(request.SymmetricKeyId, ct)
                .ConfigureAwait(false);

            if (symmetricKey is null)
            {
                m_Logger.Information(@"Attempting to retrieve symmetric key data for request {@DecryptRequest}.", request);

                symmetricKey = await GetKeyDataAsync(request.SymmetricKeyId, ct)
                   .ConfigureAwait(false);
            }

            if (symmetricKey is null)
            {
                m_Logger.Warning(@"Unable to retrieve symmetric key data for request {@DecryptRequest}.", request);
                return null;
            }

            // Decrypt and return the data.

            m_Logger.Information(@"Attempting to decrypt data for request {@DecryptRequest}.", request);

            byte[] data = await m_SymmetricKeyEncryption
                .DecryptAsync(request.EncryptedData, symmetricKey.UnwrappedSymmetricKey, symmetricKey.InitializationVector, ct)
                .ConfigureAwait(false);

            return new DecryptResponse
            {
                Data = data,
            };
        }

        public async Task<RotateAsymmetricKeyResponse> RotateAsymmetricKeyAsync(
            RotateAsymmetricKeyRequest request,
            CancellationToken ct)
        {
            await RotateAsymmetricKeyRequestValidator
                .ValidateAndThrowAsync(request, ct)
                .ConfigureAwait(false);

            // Get key data.

            SymmetricKey oldSymmetricKey = await GetCachedSymmetricKeyAsync(request.SymmetricKeyId, ct)
                .ConfigureAwait(false);

            if (oldSymmetricKey is null)
            {
                m_Logger.Information(@"Attempting to retrieve symmetric key data for request {@RotateAsymmetricKeyRequest}.", request);

                oldSymmetricKey = await GetKeyDataAsync(request.SymmetricKeyId, ct)
                   .ConfigureAwait(false);
            }

            if (oldSymmetricKey is null)
            {
                m_Logger.Warning(@"Unable to retrieve symmetric key data for request {@RotateAsymmetricKeyRequest}.", request);
                return null;
            }

            // Generate new asymmetric key.

            m_Logger.Information(@"Attempting to wrap symmetric key for request {@RotateAsymmetricKeyRequest}.", request);

            SymmetricKey newSymmetricKey =
                await WrapSymmetricWithNewAsymmetricKeyAsync(
                    oldSymmetricKey.SymmetricKeyId,
                    oldSymmetricKey.UnwrappedSymmetricKey,
                    oldSymmetricKey.InitializationVector,
                    oldSymmetricKey.SymmetricKeyName,
                    oldSymmetricKey.AsymmetricKeyName,
                    ct)
                .ConfigureAwait(false);

            if (newSymmetricKey is null)
            {
                m_Logger.Warning(@"Unable to wrap symmetric key for request {@RotateAsymmetricKeyRequest}.", request);
                return null;
            }

            // Only the version should change.

            Debug.Assert(oldSymmetricKey.SymmetricKeyId == newSymmetricKey.SymmetricKeyId);
            Debug.Assert(!string.Equals(oldSymmetricKey.AsymmetricKeyId, newSymmetricKey.AsymmetricKeyId, StringComparison.InvariantCulture));
            Debug.Assert(string.Equals(oldSymmetricKey.AsymmetricKeyName, newSymmetricKey.AsymmetricKeyName, StringComparison.InvariantCulture));

            // Register new symmetric/asymmetric key combination.

            var registerSymmetricKeyRequest = new RegisterSymmetricKeyRequest
            {
                SymmetricKeyId = newSymmetricKey.SymmetricKeyId,
                SymmetricKeyName = newSymmetricKey.SymmetricKeyName,
                AsymmetricKeyId = newSymmetricKey.AsymmetricKeyId,
                AsymmetricKeyName = newSymmetricKey.AsymmetricKeyName,
                AsymmetricKeyVersion = newSymmetricKey.AsymmetricKeyVersion,
                WrappedSymmetricKey = newSymmetricKey.WrappedSymmetricKey,
                InitializationVector = newSymmetricKey.InitializationVector,
            };

            m_Logger.Information(@"Attempting to register symmetric key for request {@RegisterSymmetricKeyRequest}.", registerSymmetricKeyRequest);

            RegisterSymmetricKeyResponse registerSymmetricKeyResponse = await m_EncryptionAccess
                .RegisterSymmetricKeyAsync(registerSymmetricKeyRequest, ct)
                .ConfigureAwait(false);

            if (registerSymmetricKeyResponse is null)
            {
                m_Logger.Warning(@"Unable to register symmetric key for request {@RegisterSymmetricKeyRequest}.", registerSymmetricKeyRequest);
                return null;
            }

            // Recache symmetric key.

            await DeleteCachedSymmetricKeyAsync(oldSymmetricKey.SymmetricKeyId, ct)
                .ConfigureAwait(false);

            await SetCachedSymmetricKeyAsync(newSymmetricKey, ct)
                .ConfigureAwait(false);

            // Disable old asymmetric key.

            var disableAsymmetricKeyRequest = new DisableAsymmetricKeyRequest
            {
                Name = oldSymmetricKey.AsymmetricKeyName,
                Version = oldSymmetricKey.AsymmetricKeyVersion,
            };

            m_Logger.Information(@"Attempting to disable asymmetric key with request {@DisableAsymmetricKeyRequest}.", disableAsymmetricKeyRequest);

            DisableAsymmetricKeyResponse disableAsymmetricKeyResponse = await m_AsymmetricKeyVault
                .DisableAsymmetricKeyAsync(disableAsymmetricKeyRequest, ct)
                .ConfigureAwait(false);

            if (disableAsymmetricKeyResponse is null)
            {
                m_Logger.Warning(@"Unable to disable asymmetric key with request {@DisableAsymmetricKeyRequest}.", disableAsymmetricKeyRequest);
                return null;
            }

            // Update old symmetric/asymmetric key combination to reflect disabling.

            var updateSymmetricKeyRequest = new UpdateSymmetricKeyRequest
            {
                SymmetricKeyId = oldSymmetricKey.SymmetricKeyId,
                SymmetricKeyName = oldSymmetricKey.SymmetricKeyName,
                AsymmetricKeyId = oldSymmetricKey.AsymmetricKeyId,
                AsymmetricKeyName = oldSymmetricKey.AsymmetricKeyName,
                AsymmetricKeyVersion = oldSymmetricKey.AsymmetricKeyVersion,
                WrappedSymmetricKey = oldSymmetricKey.WrappedSymmetricKey,
                InitializationVector = oldSymmetricKey.InitializationVector,
                IsDisabled = true,
            };

            m_Logger.Information(@"Attempting to update symmetric key with request {@UpdateSymmetricKeyRequest}.", updateSymmetricKeyRequest);

            UpdateSymmetricKeyResponse updateSymmetricKeyResponse = await m_EncryptionAccess
                .UpdateSymmetricKeyAsync(updateSymmetricKeyRequest, ct)
                .ConfigureAwait(false);

            if (updateSymmetricKeyResponse is null)
            {
                m_Logger.Warning(@"Unable to update symmetric key with request {@UpdateSymmetricKeyRequest}.", updateSymmetricKeyRequest);
                return null;
            }

            // Remove old symmetric/asymmetric key combination.

            var removeSymmetricKeyRequest = new RemoveSymmetricKeyRequest
            {
                SymmetricKeyId = request.SymmetricKeyId,
                AsymmetricKeyId = oldSymmetricKey.AsymmetricKeyId,
            };

            m_Logger.Information(@"Attempting to remove symmetric key with request {@RemoveSymmetricKeyRequest}.", removeSymmetricKeyRequest);

            bool removeSymmetricKeyResponse = await m_EncryptionAccess
                .RemoveSymmetricKeyAsync(removeSymmetricKeyRequest, ct)
                .ConfigureAwait(false);

            if (!removeSymmetricKeyResponse)
            {
                m_Logger.Warning(@"Unable to remove symmetric key with request {@RemoveSymmetricKeyRequest}.", removeSymmetricKeyRequest);

                return null;
            }

            // Return the new symmetric key definition.

            var symmetricKeyDefinition = new SymmetricKeyDefinition
            {
                Id = registerSymmetricKeyResponse.SymmetricKeyId,
                Name = registerSymmetricKeyResponse.SymmetricKeyName,
                IsEnabled = !registerSymmetricKeyResponse.IsDisabled,
                CreatedAt = registerSymmetricKeyResponse.CreatedAt,
            };

            var asymmetricKeyDefinition = new AsymmetricKeyDefinition
            {
                Id = registerSymmetricKeyResponse.AsymmetricKeyId,
                Name = registerSymmetricKeyResponse.AsymmetricKeyName,
                Version = registerSymmetricKeyResponse.AsymmetricKeyVersion,
                IsEnabled = !registerSymmetricKeyResponse.IsDisabled,
                CreatedAt = registerSymmetricKeyResponse.CreatedAt,
            };

            return new RotateAsymmetricKeyResponse
            {
                SymmetricKeyDefinition = symmetricKeyDefinition,
                AsymmetricKeyDefinition = asymmetricKeyDefinition,
            };
        }

        public async Task<ViewSymmetricKeyDefinitionResponse> ViewSymmetricKeyDefinitionAsync(
            ViewSymmetricKeyDefinitionRequest request,
            CancellationToken ct)
        {
            await ViewSymmetricKeyDefinitionRequestValidator
                .ValidateAndThrowAsync(request, ct)
                .ConfigureAwait(false);

            var viewLatestSymmetricKeyRequest = new ViewLatestSymmetricKeyRequest
            {
                SymmetricKeyId = request.SymmetricKeyId,
            };

            m_Logger.Information(@"Attempting to view symmetric key with request {@ViewLatestSymmetricKeyRequest}.", viewLatestSymmetricKeyRequest);

            ViewSymmetricKeyResponse viewSymmetricKeyResponse = await m_EncryptionAccess
                .ViewLatestSymmetricKeyAsync(viewLatestSymmetricKeyRequest, ct)
                .ConfigureAwait(false);

            if (viewSymmetricKeyResponse is null)
            {
                m_Logger.Warning(@"Unable to view symmetric key with request {@ViewLatestSymmetricKeyRequest}.", viewLatestSymmetricKeyRequest);
                return null;
            }

            return new ViewSymmetricKeyDefinitionResponse
            {
                SymmetricKeyDefinition = new SymmetricKeyDefinition
                {
                    Id = viewSymmetricKeyResponse.SymmetricKeyId,
                    Name = viewSymmetricKeyResponse.SymmetricKeyName,
                    IsEnabled = !viewSymmetricKeyResponse.IsDisabled,
                    CreatedAt = viewSymmetricKeyResponse.CreatedAt,
                },
            };
        }

        public async Task<ViewAsymmetricKeyDefinitionResponse> ViewAsymmetricKeyDefinitionAsync(
            ViewAsymmetricKeyDefinitionRequest request,
            CancellationToken ct)
        {
            await ViewAsymmetricKeyDefinitionRequestValidator
                .ValidateAndThrowAsync(request, ct)
                .ConfigureAwait(false);

            m_Logger.Information(@"Attempting to view asymmetric key with request {@ViewAsymmetricKeyDefinitionRequest}.", request);

            ViewAsymmetricKeyDefinitionResponse viewAsymmetricKeyDefinitionResponse = await m_AsymmetricKeyVault
                .ViewAsymmetricKeyDefinitionAsync(request, ct)
                .ConfigureAwait(false);

            AsymmetricKeyDefinition asymmetricKeyDefinition = viewAsymmetricKeyDefinitionResponse?.AsymmetricKeyDefinition;

            if (asymmetricKeyDefinition is null)
            {
                m_Logger.Warning(@"Unable to view asymmetric key with request {@ViewAsymmetricKeyDefinitionRequest}.", request);
                return null;
            }

            return new ViewAsymmetricKeyDefinitionResponse
            {
                AsymmetricKeyDefinition = asymmetricKeyDefinition,
            };
        }

        #endregion
    }
}
