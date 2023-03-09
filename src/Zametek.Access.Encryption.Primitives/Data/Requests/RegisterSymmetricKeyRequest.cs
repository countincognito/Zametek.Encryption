using System;
using Zametek.Utility.Logging;

namespace Zametek.Access.Encryption
{
    [Serializable]
    public class RegisterSymmetricKeyRequest
    {
        public Guid SymmetricKeyId { get; set; }

        public string SymmetricKeyName { get; set; }

        public string AsymmetricKeyId { get; set; }

        public string AsymmetricKeyName { get; set; }

        public string AsymmetricKeyVersion { get; set; }

        [NoLogging]
        public byte[] WrappedSymmetricKey { get; set; }

        [NoLogging]
        public byte[] InitializationVector { get; set; }
    }
}