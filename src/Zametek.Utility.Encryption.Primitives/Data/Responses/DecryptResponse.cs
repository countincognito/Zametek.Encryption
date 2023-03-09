using System;
using Zametek.Utility.Logging;

namespace Zametek.Utility.Encryption
{
    [Serializable]
    public class DecryptResponse
    {
        [NoLogging]
        public byte[] Data { get; set; }
    }
}
