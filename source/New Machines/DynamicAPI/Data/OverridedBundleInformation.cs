using System.Collections.Generic;
using System.Linq;
using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using Newtonsoft.Json;

namespace Igorious.StardewValley.DynamicAPI.Data
{
    public class OverridedBundleInformation
    {
        [JsonConstructor]
        public OverridedBundleInformation() { }

        public OverridedBundleInformation(string key, params BundleItemInformation[] items)
        {
            Key = key;
            Items = items.ToList();
        }

        [JsonProperty(Required = Required.Always)]
        public string Key { get; set; }

        [JsonProperty(Required = Required.Always)]
        public List<BundleItemInformation> Items { get; set; }
    }
}