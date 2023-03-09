using System;

namespace Zametek.Utility.Encryption
{
    [Serializable]
    public class AsymmetricKeyDefinition
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Version { get; set; }

        public bool? IsEnabled { get; set; }

        public DateTimeOffset? CreatedAt { get; set; }
    }
}
