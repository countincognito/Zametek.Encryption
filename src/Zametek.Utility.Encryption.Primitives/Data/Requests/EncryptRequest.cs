using System;
using Zametek.Utility.Logging;

namespace Zametek.Utility.Encryption
{
    [Serializable]
    public class EncryptRequest
    {
        public Guid SymmetricKeyId { get; set; }

        [NoLogging]
        public byte[] Data { get; set; }
    }
}
