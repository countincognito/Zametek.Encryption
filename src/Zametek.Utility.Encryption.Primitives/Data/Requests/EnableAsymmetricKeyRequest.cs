using System;

namespace Zametek.Utility.Encryption
{
    [Serializable]
    public class EnableAsymmetricKeyRequest
    {
        public string Name { get; set; }

        public string Version { get; set; }
    }
}
