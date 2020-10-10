/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Denifia/StardewMods
**
*************************************************/

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
