using System;

namespace Zametek.Utility.Encryption
{
    [Serializable]
    public class CreateKeysRequest
    {
        public string SymmetricKeyName { get; set; }

        public string AsymmetricKeyName { get; set; }
    }
}
