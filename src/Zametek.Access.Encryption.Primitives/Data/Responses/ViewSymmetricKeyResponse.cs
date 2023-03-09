using System;
using Zametek.Utility.Logging;

namespace Zametek.Access.Encryption
{
    [Serializable]
    public class ViewSymmetricKeyResponse
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

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset ModifiedAt { get; set; }

        public bool IsDisabled { get; set; }
    }
}
