using System;

namespace Zametek.Utility.Encryption
{
    [Serializable]
    public class DisableAsymmetricKeyResponse
    {
        public AsymmetricKeyDefinition AsymmetricKeyDefinition { get; set; }
    }
}
