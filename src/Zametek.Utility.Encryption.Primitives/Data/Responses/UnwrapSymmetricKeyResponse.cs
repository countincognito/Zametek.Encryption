using System;
using Zametek.Utility.Logging;

namespace Zametek.Utility.Encryption
{
    [Serializable]
    public class UnwrapSymmetricKeyResponse
    {
        [NoLogging]
        public byte[] SymmetricKey { get; set; }
    }
}
