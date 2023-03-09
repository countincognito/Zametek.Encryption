using System;

namespace Zametek.Utility.Encryption
{
    [Serializable]
    public class RemoveAsymmetricKeyRequest
    {
        public string Name { get; set; }

        public bool AwaitCompletion { get; set; }
    }
}
