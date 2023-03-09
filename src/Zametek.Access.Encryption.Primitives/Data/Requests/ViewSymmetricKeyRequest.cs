using System;

namespace Zametek.Access.Encryption
{
    [Serializable]
    public class ViewSymmetricKeyRequest
    {
        public Guid SymmetricKeyId { get; set; }

        public string AsymmetricKeyId { get; set; }
    }
}