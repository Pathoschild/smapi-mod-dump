namespace Denifia.Stardew.SendItemsApi.Domain
{
    public class AzureStorageConfig
    {
        public string AccountName { get; set; }
        public string AccountKey { get; set; }
        public string EndpointSuffix { get; set; }
        public bool UseHttps { get; set; } = true;
        public string TableName { get; set; }
    }
}
