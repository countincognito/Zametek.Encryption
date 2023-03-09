using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Zametek.Utility.Logging;

namespace Zametek.Utility.Encryption
{
    /// <summary>
    /// Based on the work of Stephen Haunts
    /// https://github.com/stephenhaunts/AzureKeyVault
    /// https://github.com/stephenhaunts/SecureCodingWorkshop
    /// https://github.com/stephenhaunts/Building-Secure-Applications-with-Cryptography-in-.NET-Course-Source-Code
    /// and
    /// https://github.com/Azure/azure-sdk-for-net/tree/master/sdk/keyvault
    /// https://github.com/Azure/azure-sdk-for-net/tree/master/sdk/keyvault/Azure.Security.KeyVault.Keys
    /// </summary>
    [DiagnosticLogging(LogActive.Off)]
    public class AzureKeyVault
        : IAsymmetricKeyVault
    {
        #region Fields

        private readonly string m_AppName;
        private readonly TokenCredential m_Credential;
        private readonly KeyClient m_KeyClient;
        private static readonly KeyWrapAlgorithm s_KeyWrapAlgorithm = KeyWrapAlgorithm.RsaOaep;

        #endregion

        #region Ctors

        public AzureKeyVault(IOptions<AzureKeyVaultOptions> options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            AzureKeyVaultOptions keyVaultOptions = options.Value;
            AsymmetricKeyVaultOptionsValidator.ValidateAndThrow(keyVaultOptions);

            m_AppName = keyVaultOptions.AppName;
            m_Credential = new ClientSecretCredential(
                keyVaultOptions.TenantId,
                keyVaultOptions.ClientId,
                keyVaultOptions.ClientSecret);
            m_KeyClient = new KeyClient(
                keyVaultOptions.VaultUrl,
                m_Credential);
        }

        #endregion

        #region Private Members

        private Uri GetAsymmetricKeyId(
            string name,
            string version)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (string.IsNullOrWhiteSpace(version))
            {
                throw new ArgumentNullException(nameof(version));
            }
            return new Uri(m_KeyClient.VaultUri, $@"keys/{name}/{version}");
        }

        private IDictionary<string, string> GetKeyTags()
        {
            return new Dictionary<string, string>
            {
                { @"purpose", @"Encryption Key" },
                { @"app", m_AppName }
            };
        }

        private async Task<AsymmetricKeyDefinition> UpdateKeyAsync(
            KeyProperties keyProperties,
            CancellationToken ct)
        {
            if (keyProperties is null)
            {
                throw new ArgumentNullException(nameof(keyProperties));
            }

            KeyVaultKey response = await m_KeyClient
                .UpdateKeyPropertiesAsync(keyProperties, cancellationToken: ct)
                .ConfigureAwait(false);

            if (response is null)
            {
                return null;
            }

            return new AsymmetricKeyDefinition
            {
                Id = response?.Properties?.Id?.AbsoluteUri,
                Name = response?.Properties?.Name,
                Version = response?.Properties?.Version,
                IsEnabled = response?.Properties?.Enabled,
                CreatedAt = response?.Properties?.CreatedOn,
            };
        }

        #endregion

        #region IAsymmetricKeyVault Members

        public async Task<CreateAsymmetricKeyResponse> CreateAsymmetricKeyAsync(
            CreateAsymmetricKeyRequest request,
            CancellationToken ct)
        {
            await CreateAsymmetricKeyRequestValidator
                .ValidateAndThrowAsync(request, ct)
                .ConfigureAwait(false);

            var rsaKeyOptions = new CreateRsaKeyOptions(request.Name)
            {
                Enabled = true,
                ExpiresOn = null,//DateTimeOffset.FromUnixTimeSeconds(int.MaxValue).UtcDateTime,
                NotBefore = null,//DateTimeOffset.FromUnixTimeSeconds(0).UtcDateTime,
            };

            foreach (KeyValuePair<string, string> kvp in GetKeyTags())
            {
                rsaKeyOptions.Tags.Add(kvp);
            }

            KeyVaultKey response = await m_KeyClient
                .CreateRsaKeyAsync(rsaKeyOptions, ct)
                .ConfigureAwait(false);

            if (response is null)
            {
                return null;
            }

            var keyDefinition = new AsymmetricKeyDefinition
            {
                Id = response?.Properties?.Id?.AbsoluteUri,
                Name = response?.Properties?.Name,
                Version = response?.Properties?.Version,
                IsEnabled = response?.Properties?.Enabled,
                CreatedAt = response?.Properties?.CreatedOn,
            };

            return new CreateAsymmetricKeyResponse
            {
                AsymmetricKeyDefinition = keyDefinition,
            };
        }

        public async Task<DisableAsymmetricKeyResponse> DisableAsymmetricKeyAsync(
            DisableAsymmetricKeyRequest request,
            CancellationToken ct)
        {
            await DisableAsymmetricKeyRequestValidator
                .ValidateAndThrowAsync(request, ct)
                .ConfigureAwait(false);

            var asymmetricKeyId = GetAsymmetricKeyId(request.Name, request.Version);

            var keyProperties = new KeyProperties(asymmetricKeyId)
            {
                Enabled = false,
            };

            AsymmetricKeyDefinition keyDefinition =
                await UpdateKeyAsync(keyProperties, ct)
                .ConfigureAwait(false);

            return new DisableAsymmetricKeyResponse
            {
                AsymmetricKeyDefinition = keyDefinition,
            };
        }

        public async Task<EnableAsymmetricKeyResponse> EnableAsymmetricKeyAsync(
            EnableAsymmetricKeyRequest request,
            CancellationToken ct)
        {
            await EnableAsymmetricKeyRequestValidator
                .ValidateAndThrowAsync(request, ct)
                .ConfigureAwait(false);

            var asymmetricKeyId = GetAsymmetricKeyId(request.Name, request.Version);

            var keyProperties = new KeyProperties(asymmetricKeyId)
            {
                Enabled = true,
            };

            AsymmetricKeyDefinition keyDefinition =
                await UpdateKeyAsync(keyProperties, ct)
                .ConfigureAwait(false);

            return new EnableAsymmetricKeyResponse
            {
                AsymmetricKeyDefinition = keyDefinition,
            };
        }

        public async Task<bool> RemoveAsymmetricKeyAsync(
            RemoveAsymmetricKeyRequest request,
            CancellationToken ct)
        {
            await RemoveAsymmetricKeyRequestValidator
                .ValidateAndThrowAsync(request, ct)
                .ConfigureAwait(false);

            DeleteKeyOperation deleteKeyOperation = await m_KeyClient
                .StartDeleteKeyAsync(request.Name, ct)
                .ConfigureAwait(false);

            if (deleteKeyOperation is null)
            {
                return false;
            }

            if (request.AwaitCompletion)
            {
                DeletedKey deletedKey = await deleteKeyOperation
                    .WaitForCompletionAsync(ct)
                    .ConfigureAwait(false);

                if (deletedKey is null)
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<WrapSymmetricKeyResponse> WrapSymmetricKeyAsync(
            WrapSymmetricKeyRequest request,
            CancellationToken ct)
        {
            await WrapSymmetricKeyRequestValidator
                .ValidateAndThrowAsync(request, ct)
                .ConfigureAwait(false);

            KeyVaultKey key = await m_KeyClient
                .GetKeyAsync(request.AsymmetricKeyName, request.AsymmetricKeyVersion, ct)
                .ConfigureAwait(false);

            if (key is null)
            {
                return null;
            }

            if (key?.Properties?.Enabled != true)
            {
                throw new InvalidOperationException($@"{Properties.Resources.AsymmetricKeyIsNotEnabled}: {key?.Id}");
            }

            var cryptoClient = new CryptographyClient(key.Id, m_Credential);

            WrapResult result = await cryptoClient
                .WrapKeyAsync(s_KeyWrapAlgorithm, request.SymmetricKey, ct)
                .ConfigureAwait(false);

            if (result is null)
            {
                return null;
            }

            return new WrapSymmetricKeyResponse
            {
                WrappedSymmetricKey = result.EncryptedKey,
            };
        }

        public async Task<UnwrapSymmetricKeyResponse> UnwrapSymmetricKeyAsync(
            UnwrapSymmetricKeyRequest request,
            CancellationToken ct)
        {
            await UnwrapSymmetricKeyRequestValidator
                .ValidateAndThrowAsync(request, ct)
                .ConfigureAwait(false);

            KeyVaultKey key = await m_KeyClient
                .GetKeyAsync(request.AsymmetricKeyName, request.AsymmetricKeyVersion, ct)
                .ConfigureAwait(false);

            if (key is null)
            {
                return null;
            }

            if (key?.Properties?.Enabled != true)
            {
                throw new InvalidOperationException($@"{Properties.Resources.AsymmetricKeyIsNotEnabled}: {key?.Id}");
            }

            var cryptoClient = new CryptographyClient(key.Id, m_Credential);

            UnwrapResult result = await cryptoClient
                .UnwrapKeyAsync(s_KeyWrapAlgorithm, request.WrappedSymmetricKey, ct)
                .ConfigureAwait(false);

            if (result is null)
            {
                return null;
            }

            return new UnwrapSymmetricKeyResponse
            {
                SymmetricKey = result.Key,
            };
        }

        public async Task<ViewAsymmetricKeyDefinitionResponse> ViewAsymmetricKeyDefinitionAsync(
            ViewAsymmetricKeyDefinitionRequest request,
            CancellationToken ct)
        {
            await ViewAsymmetricKeyDefinitionRequestValidator
                .ValidateAndThrowAsync(request, ct)
                .ConfigureAwait(false);

            KeyVaultKey key = await m_KeyClient
                .GetKeyAsync(request.Name, request.Version, ct)
                .ConfigureAwait(false);

            if (key is null)
            {
                return null;
            }

            var keyDefinition = new AsymmetricKeyDefinition
            {
                Id = key?.Properties?.Id?.AbsoluteUri,
                Name = key?.Properties?.Name,
                Version = key?.Properties?.Version,
                IsEnabled = key?.Properties?.Enabled,
                CreatedAt = key?.Properties?.CreatedOn,
            };

            return new ViewAsymmetricKeyDefinitionResponse
            {
                AsymmetricKeyDefinition = keyDefinition,
            };
        }

        #endregion
    }
}
