using System;

namespace Zametek.Utility.Encryption
{
    [Serializable]
    public class DisableAsymmetricKeyRequest
    {
        public string Name { get; set; }

        public string Version { get; set; }
    }
}
