 using System;
using Zametek.Utility.Logging;

namespace Zametek.Utility.Encryption
{
    [Serializable]
    public class EncryptResponse
    {
        [NoLogging]
        public byte[] EncryptedData { get; set; }
    }
}
