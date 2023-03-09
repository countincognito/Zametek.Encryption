using System;

namespace Zametek.Utility.Encryption
{
    [Serializable]
    public class EnableAsymmetricKeyResponse
    {
        public AsymmetricKeyDefinition AsymmetricKeyDefinition { get; set; }
    }
}
