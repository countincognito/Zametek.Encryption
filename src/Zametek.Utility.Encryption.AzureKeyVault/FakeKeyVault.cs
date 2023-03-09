using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Zametek.Utility.Encryption
{
    /// <summary>
    /// Based on the work of Stephen Haunts
    /// https://github.com/stephenhaunts/AzureKeyVault
    /// https://github.com/stephenhaunts/SecureCodingWorkshop
    /// https://github.com/stephenhaunts/Building-Secure-Applications-with-Cryptography-in-.NET-Course-Source-Code
    /// </summary>
    public class FakeKeyVault
        : IAsymmetricKeyVault
    {
        #region Fields

        private const int c_KeySize = 2048;

        private readonly IDictionary<string, RSAParameters> _PublicKeys;
        private readonly IDictionary<string, RSAParameters> _PrivateKeys;
        private readonly IDictionary<string, byte[]> _Keys;

        #endregion

        #region Ctors

        public FakeKeyVault()
        {
            _PublicKeys = new Dictionary<string, RSAParameters>();
            _PrivateKeys = new Dictionary<string, RSAParameters>();
            _Keys = new Dictionary<string, byte[]>();
        }

        #endregion

        #region IAsymmetricKeyVault Members

        public async Task<CreateAsymmetricKeyResponse> CreateAsymmetricKeyAsync(
            CreateAsymmetricKeyRequest request,
            CancellationToken ct)
        {
            string keyVersion = Guid.NewGuid().ToString();

            string keyId = $@"{request.Name}/{keyVersion}";

            using (var rsa = new RSACryptoServiceProvider(c_KeySize))
            {
                rsa.PersistKeyInCsp = false;
                _PublicKeys.Add(keyId, rsa.ExportParameters(false));
                _PrivateKeys.Add(keyId, rsa.ExportParameters(true));
            }

            var keyDefinition = new AsymmetricKeyDefinition
            {
                Id = keyId,
                Name = request.Name,
                Version = keyVersion,
                IsEnabled = true,
                CreatedAt = DateTimeOffset.UtcNow,
            };

            _Keys.Add(keyId, keyDefinition.ObjectToByteArray());

            return await Task.FromResult(new CreateAsymmetricKeyResponse
            {
                AsymmetricKeyDefinition = keyDefinition,
            }).ConfigureAwait(false);
        }

        public async Task<DisableAsymmetricKeyResponse> DisableAsymmetricKeyAsync(
            DisableAsymmetricKeyRequest request,
            CancellationToken ct)
        {
            string keyId = $@"{request.Name}/{request.Version}";

            var key = _Keys[keyId].ByteArrayToObject<AsymmetricKeyDefinition>();
            key.IsEnabled = false;
            _Keys[keyId] = key.ObjectToByteArray();

            return await Task.FromResult(new DisableAsymmetricKeyResponse
            {
                AsymmetricKeyDefinition = key,
            }).ConfigureAwait(false);
        }

        public async Task<EnableAsymmetricKeyResponse> EnableAsymmetricKeyAsync(
            EnableAsymmetricKeyRequest request,
            CancellationToken ct)
        {
            string keyId = $@"{request.Name}/{request.Version}";

            var key = _Keys[keyId].ByteArrayToObject<AsymmetricKeyDefinition>();
            key.IsEnabled = true;
            _Keys[keyId] = key.ObjectToByteArray();

            return await Task.FromResult(new EnableAsymmetricKeyResponse
            {
                AsymmetricKeyDefinition = key,
            }).ConfigureAwait(false);
        }

        public async Task<bool> RemoveAsymmetricKeyAsync(
            RemoveAsymmetricKeyRequest request,
            CancellationToken ct)
        {
            var keys = _Keys.Keys.Where(x => Regex.Match(x, $@"^{request.Name}/.+").Success);

            if (!keys.Any())
            {
                throw new InvalidOperationException($@"Asymmetric key {request.Name} not present");
            }

            foreach (string key in keys)
            {
                _Keys.Remove(key);
                _PublicKeys.Remove(key);
                _PrivateKeys.Remove(key);
            }

            return await Task.FromResult(true);
        }

        public async Task<WrapSymmetricKeyResponse> WrapSymmetricKeyAsync(
            WrapSymmetricKeyRequest request,
            CancellationToken ct)
        {
            string keyId = $@"{request.AsymmetricKeyName}/{request.AsymmetricKeyVersion}";

            if (!_Keys[keyId].ByteArrayToObject<AsymmetricKeyDefinition>().IsEnabled.GetValueOrDefault())
            {
                throw new InvalidOperationException($@"RSA key {keyId} is disabled.");
            }

            using (var rsa = new RSACryptoServiceProvider(c_KeySize))
            {
                rsa.PersistKeyInCsp = false;
                rsa.ImportParameters(_PublicKeys[keyId]);
                return new WrapSymmetricKeyResponse
                {
                    WrappedSymmetricKey = await Task.FromResult(rsa.Encrypt(request.SymmetricKey, true)).ConfigureAwait(false),
                };
            }
        }

        public async Task<UnwrapSymmetricKeyResponse> UnwrapSymmetricKeyAsync(
            UnwrapSymmetricKeyRequest request,
            CancellationToken ct)
        {
            string keyId = $@"{request.AsymmetricKeyName}/{request.AsymmetricKeyVersion}";

            if (!_Keys[keyId].ByteArrayToObject<AsymmetricKeyDefinition>().IsEnabled.GetValueOrDefault())
            {
                throw new InvalidOperationException($@"RSA key {keyId} is disabled.");
            }

            using (var rsa = new RSACryptoServiceProvider(c_KeySize))
            {
                rsa.PersistKeyInCsp = false;
                rsa.ImportParameters(_PrivateKeys[keyId]);
                return new UnwrapSymmetricKeyResponse
                {
                    SymmetricKey = await Task.FromResult(rsa.Decrypt(request.WrappedSymmetricKey, true)).ConfigureAwait(false),
                };
            }
        }

        public async Task<ViewAsymmetricKeyDefinitionResponse> ViewAsymmetricKeyDefinitionAsync(
            ViewAsymmetricKeyDefinitionRequest request,
            CancellationToken ct)
        {
            string keyId = $@"{request.Name}/{request.Version}";

            return await Task.FromResult(new ViewAsymmetricKeyDefinitionResponse
            {
                AsymmetricKeyDefinition = _Keys[keyId].ByteArrayToObject<AsymmetricKeyDefinition>(),
            }).ConfigureAwait(false);
        }

        #endregion
    }
}
