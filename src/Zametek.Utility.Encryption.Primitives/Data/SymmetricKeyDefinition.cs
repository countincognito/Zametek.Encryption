using System;

namespace Zametek.Utility.Encryption
{
    [Serializable]
    public class SymmetricKeyDefinition
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool IsEnabled { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}
