using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Serilog;
using Zametek.Utility;
using Zametek.Utility.Cache;
using Zametek.Utility.Logging;

namespace Zametek.Access.Encryption
{
    [DiagnosticLogging(LogActive.On)]
    public class EncryptionAccess
        : IEncryptionAccess
    {
        #region Fields

        private readonly ICacheUtility m_CacheUtility;
        private readonly IMapper m_Mapper;
        private readonly IDbContextFactory<EncryptionDbContext> m_CtxFactory;
        private readonly ILogger m_Logger;

        private readonly DistributedCacheEntryOptions m_CacheOptions;

        private const string c_SymmetricKeyPrefix = @"DbSymmetricKey";

        #endregion

        #region Ctors

        public EncryptionAccess(
            ICacheUtility cacheUtility,
            IMapper mapper,
            IDbContextFactory<EncryptionDbContext> ctxFactory,
            IOptions<CacheOptions> cacheOptions,
            ILogger logger)
        {
            m_CacheUtility = cacheUtility ?? throw new ArgumentNullException(nameof(cacheUtility));
            m_Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            m_CtxFactory = ctxFactory ?? throw new ArgumentNullException(nameof(ctxFactory));
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

        private async Task<SymmetricKey?> GetCachedSymmetricKeyAsync(
            Guid symmetricKeyId,
            CancellationToken ct)
        {
            if (symmetricKeyId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(symmetricKeyId));
            }

            SymmetricKey? symmetricKey;
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

        #endregion

        #region Public Members

        public static string GetCacheKey(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return $@"{c_SymmetricKeyPrefix}_{id.ToFlatString()}";
        }

        #endregion

        #region IEncryptionAccess Members

        public async Task<RegisterSymmetricKeyResponse> RegisterSymmetricKeyAsync(
            RegisterSymmetricKeyRequest request,
            CancellationToken ct)
        {
            await RegisterSymmetricKeyRequestValidator
                .ValidateAndThrowAsync(request, ct)
                .ConfigureAwait(false);

            SymmetricKey symmetricKey = m_Mapper.Map<RegisterSymmetricKeyRequest, SymmetricKey>(request);

            DateTimeOffset time = DateTimeOffset.UtcNow;

            symmetricKey.CreatedAt = time;
            symmetricKey.ModifiedAt = time;

            using var ctx = await m_CtxFactory.CreateDbContextAsync(ct).ConfigureAwait(false);
            using var transaction = await ctx.Database.BeginTransactionAsync(ct).ConfigureAwait(false);

            try
            {
                await ctx.SymmetricKeys
                    .AddAsync(symmetricKey, ct)
                    .ConfigureAwait(false);
                await ctx.SaveChangesAsync(true, ct)
                    .ConfigureAwait(false);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                m_Logger.Error(ex, @"Failed to register a new SymmetricKey {@SymmetricKey} for request {@RegisterSymmetricKeyRequest}.", symmetricKey, request);
                transaction.Rollback();
                throw;
            }

            await SetCachedSymmetricKeyAsync(symmetricKey, ct)
                .ConfigureAwait(false);

            return m_Mapper.Map<SymmetricKey, RegisterSymmetricKeyResponse>(symmetricKey);
        }

        public async Task<UpdateSymmetricKeyResponse?> UpdateSymmetricKeyAsync(
            UpdateSymmetricKeyRequest request,
            CancellationToken ct)
        {
            await UpdateSymmetricKeyRequestValidator
                .ValidateAndThrowAsync(request, ct)
                .ConfigureAwait(false);

            SymmetricKey? symmetricKey = null;

            using var ctx = await m_CtxFactory.CreateDbContextAsync(ct).ConfigureAwait(false);

            symmetricKey = await ctx.SymmetricKeys
                .Where(x => !x.IsDeleted
                    && x.SymmetricKeyId == request.SymmetricKeyId
                    && x.AsymmetricKeyId == request.AsymmetricKeyId)
                .SingleOrDefaultAsync(ct)
                .ConfigureAwait(false);

            if (symmetricKey is null)
            {
                return null;
            }

            var time = DateTimeOffset.UtcNow;

            using var transaction = await ctx.Database.BeginTransactionAsync(ct).ConfigureAwait(false);

            try
            {
                symmetricKey = m_Mapper.Map(request, symmetricKey);
                symmetricKey.ModifiedAt = time;
                await ctx.SaveChangesAsync(true, ct)
                    .ConfigureAwait(false);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                m_Logger.Error(ex, @"Failed to update SymmetricKey for request {@UpdateSymmetricKeyRequest}.", request);
                transaction.Rollback();
                throw;
            }

            await SetCachedSymmetricKeyAsync(symmetricKey, ct)
                .ConfigureAwait(false);

            return m_Mapper.Map<SymmetricKey, UpdateSymmetricKeyResponse>(symmetricKey);
        }

        public async Task<ViewSymmetricKeyResponse?> ViewSymmetricKeyAsync(
            ViewSymmetricKeyRequest request,
            CancellationToken ct)
        {
            await ViewSymmetricKeyRequestValidator
                .ValidateAndThrowAsync(request, ct)
                .ConfigureAwait(false);

            SymmetricKey? symmetricKey = await GetCachedSymmetricKeyAsync(request.SymmetricKeyId, ct)
                .ConfigureAwait(false);

            if (symmetricKey is null)
            {
                using var ctx = await m_CtxFactory.CreateDbContextAsync(ct).ConfigureAwait(false);

                symmetricKey = await ctx.SymmetricKeys
                    .Where(x => !x.IsDeleted
                        && x.SymmetricKeyId == request.SymmetricKeyId
                        && x.AsymmetricKeyId == request.AsymmetricKeyId)
                    .SingleOrDefaultAsync(ct)
                    .ConfigureAwait(false);
            }

            if (symmetricKey is null)
            {
                m_Logger.Warning(@"Failed to return SymmetricKey for request {@ViewSymmetricKeyRequest}.", request);
                return null;
            }

            await SetCachedSymmetricKeyAsync(symmetricKey, ct)
                .ConfigureAwait(false);

            return m_Mapper.Map<SymmetricKey, ViewSymmetricKeyResponse>(symmetricKey);
        }

        public async Task<ViewSymmetricKeyResponse?> ViewLatestSymmetricKeyAsync(
            ViewLatestSymmetricKeyRequest request,
            CancellationToken ct)
        {
            await ViewLatestSymmetricKeyRequestValidator
                .ValidateAndThrowAsync(request, ct)
                .ConfigureAwait(false);

            SymmetricKey? symmetricKey = await GetCachedSymmetricKeyAsync(request.SymmetricKeyId, ct)
                .ConfigureAwait(false);

            if (symmetricKey is null)
            {
                using var ctx = await m_CtxFactory.CreateDbContextAsync(ct).ConfigureAwait(false);

                symmetricKey = await ctx.SymmetricKeys
                    .OrderByDescending(x => x.CreatedAt)
                    .FirstOrDefaultAsync(x => !x.IsDeleted
                        && x.SymmetricKeyId == request.SymmetricKeyId, ct)
                    .ConfigureAwait(false);
            }

            if (symmetricKey is null)
            {
                m_Logger.Warning(@"Failed to return SymmetricKey for request {@ViewLatestSymmetricKeyRequest}.", request);
                return null;
            }

            await SetCachedSymmetricKeyAsync(symmetricKey, ct)
                .ConfigureAwait(false);

            return m_Mapper.Map<SymmetricKey, ViewSymmetricKeyResponse>(symmetricKey);
        }

        public async Task<bool> RemoveSymmetricKeyAsync(
            RemoveSymmetricKeyRequest request,
            CancellationToken ct)
        {
            await RemoveSymmetricKeyRequestValidator
                .ValidateAndThrowAsync(request, ct)
                .ConfigureAwait(false);

            await DeleteCachedSymmetricKeyAsync(request.SymmetricKeyId, ct)
                .ConfigureAwait(false);

            using var ctx = await m_CtxFactory.CreateDbContextAsync(ct).ConfigureAwait(false);

            SymmetricKey? symmetricKey = await ctx.SymmetricKeys
                .Where(x => !x.IsDeleted
                    && x.SymmetricKeyId == request.SymmetricKeyId
                    && x.AsymmetricKeyId == request.AsymmetricKeyId)
                .SingleOrDefaultAsync(ct)
                .ConfigureAwait(false);

            if (symmetricKey is null)
            {
                return false;
            }

            using var transaction = await ctx.Database.BeginTransactionAsync(ct).ConfigureAwait(false);

            try
            {
                symmetricKey.IsDeleted = true;
                symmetricKey.ModifiedAt = DateTimeOffset.UtcNow;
                await ctx.SaveChangesAsync(true, ct)
                    .ConfigureAwait(false);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                m_Logger.Error(ex, @"Failed to delete SymmetricKey for request {@RemoveSymmetricKeyRequest}.", request);
                transaction.Rollback();
                throw;
            }

            return true;
        }

        #endregion
    }
}
