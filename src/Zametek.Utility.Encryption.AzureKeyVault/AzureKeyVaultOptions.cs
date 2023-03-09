using System;

namespace Zametek.Utility.Encryption
{
    [Serializable]
    public class AzureKeyVaultOptions
    {
        public string AppName { get; set; }

        public Uri VaultUrl { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string TenantId { get; set; }
    }
}
