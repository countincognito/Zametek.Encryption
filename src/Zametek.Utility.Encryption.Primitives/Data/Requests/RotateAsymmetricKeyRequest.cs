using System;

namespace Zametek.Utility.Encryption
{
    [Serializable]
    public class RotateAsymmetricKeyRequest
    {
        public Guid SymmetricKeyId { get; set; }
    }
}
