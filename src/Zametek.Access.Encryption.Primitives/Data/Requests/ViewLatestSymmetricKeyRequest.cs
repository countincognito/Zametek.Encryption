using System;

namespace Zametek.Access.Encryption
{
    [Serializable]
    public class ViewLatestSymmetricKeyRequest
    {
        public Guid SymmetricKeyId { get; set; }
    }
}