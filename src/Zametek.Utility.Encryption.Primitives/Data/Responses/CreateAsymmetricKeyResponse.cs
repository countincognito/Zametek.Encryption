using System;

namespace Zametek.Utility.Encryption
{
    [Serializable]
    public class CreateAsymmetricKeyResponse
    {
        public AsymmetricKeyDefinition AsymmetricKeyDefinition { get; set; }
    }
}
