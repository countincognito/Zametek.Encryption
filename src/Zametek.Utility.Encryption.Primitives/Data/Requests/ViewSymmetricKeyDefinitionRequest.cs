using System;

namespace Zametek.Utility.Encryption
{
    [Serializable]
    public class ViewSymmetricKeyDefinitionRequest
    {
        public Guid SymmetricKeyId { get; set; }
    }
}
