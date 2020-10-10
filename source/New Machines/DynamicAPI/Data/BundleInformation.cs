/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using Igorious.StardewValley.DynamicAPI.Extensions;
using Newtonsoft.Json;

namespace Igorious.StardewValley.DynamicAPI.Data
{
    public sealed class BundleInformation : OverridedBundleInformation
    {
        #region Properties

        [JsonProperty(Required = Required.Always)]
        public string Name { get; set; }

        [JsonIgnore]
        private string UnknownValue1 { get; set; }

        [JsonProperty(Required = Required.Always)]
        public DynamicID<ItemID> RewardID { get; set; }

        [JsonProperty, DefaultValue(1)]
        public int RewardAmount { get; set; } = 1;

        [JsonIgnore]
        private int UnknownValue2 { get; set; }

        [JsonProperty]
        public int Cells { get; set; }

        #endregion

        #region Serialization

        public static BundleInformation Parse(string bundleInformation, string key = null)
        {
            var info = new BundleInformation();
            var parts = bundleInformation.Split('/');
            info.Key = key;
            info.Name = parts[0];

            var rewardParts = parts[1].Split(' ');
            info.UnknownValue1 = rewardParts[0];
            info.RewardID = int.Parse(rewardParts[1]);
            info.RewardAmount = int.Parse(rewardParts[2]);

            var itemsParts = parts[2].Split(' ').Select(int.Parse).ToArray();
            info.Items = new List<BundleItemInformation>();
            for (var i = 0; i < itemsParts.Length; i += 3)
            {
                info.Items.Add(new BundleItemInformation(itemsParts[i], itemsParts[i + 1], itemsParts[i + 2]));
            }

            info.UnknownValue2 = int.Parse(parts[3]);
            if (parts.Length > 4) info.Cells = int.Parse(parts[4]);
            return info;
        }

        public override string ToString()
        {
            var s = $"{Name}/{UnknownValue1} {RewardID} {RewardAmount}/{Items.Serialize()}/{UnknownValue2}";
            if (Cells > 0) s = s + $"/{Cells}";
            return s;
        }

        #endregion
    }
}