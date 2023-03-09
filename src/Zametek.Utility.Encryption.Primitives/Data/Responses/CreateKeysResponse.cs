using System;

namespace Zametek.Utility.Encryption
{
    [Serializable]
    public class CreateKeysResponse
    {
        public SymmetricKeyDefinition SymmetricKeyDefinition { get; set; }

        public AsymmetricKeyDefinition AsymmetricKeyDefinition { get; set; }
    }
}
