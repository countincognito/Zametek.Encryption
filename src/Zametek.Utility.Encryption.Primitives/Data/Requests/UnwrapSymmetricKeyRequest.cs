using System;
using Zametek.Utility.Logging;

namespace Zametek.Utility.Encryption
{
    [Serializable]
    public class UnwrapSymmetricKeyRequest
    {
        public string AsymmetricKeyName { get; set; }

        public string AsymmetricKeyVersion { get; set; }

        [NoLogging]
        public byte[] WrappedSymmetricKey { get; set; }
    }
}
