using System;
using Zametek.Utility.Logging;

namespace Zametek.Utility.Encryption
{
    [Serializable]
    public class DecryptRequest
    {
        public Guid SymmetricKeyId { get; set; }

        [NoLogging]
        public byte[] EncryptedData { get; set; }
    }
}
