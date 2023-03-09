using System;

namespace Zametek.Utility.Encryption
{
    [Serializable]
    public class RotateAsymmetricKeyResponse
    {
        public SymmetricKeyDefinition SymmetricKeyDefinition { get; set; }

        public AsymmetricKeyDefinition AsymmetricKeyDefinition { get; set; }
    }
}
