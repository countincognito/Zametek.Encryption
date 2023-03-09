using System;

namespace Zametek.Access.Encryption
{
    [Serializable]
    public class RemoveSymmetricKeyRequest
    {
        public Guid SymmetricKeyId { get; set; }

        public string AsymmetricKeyId { get; set; }
    }
}