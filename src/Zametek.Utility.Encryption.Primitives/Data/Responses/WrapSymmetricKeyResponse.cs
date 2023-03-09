using System;
using Zametek.Utility.Logging;

namespace Zametek.Utility.Encryption
{
    [Serializable]
    public class WrapSymmetricKeyResponse
    {
        [NoLogging]
        public byte[] WrappedSymmetricKey { get; set; }
    }
}
