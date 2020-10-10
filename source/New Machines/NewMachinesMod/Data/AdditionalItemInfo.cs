/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using Newtonsoft.Json;

namespace Igorious.StardewValley.NewMachinesMod.Data
{
    public sealed class AdditionalItemInfo
    {
        [JsonConstructor]
        public AdditionalItemInfo() { }

        public AdditionalItemInfo(DynamicID<ItemID, CategoryID> id, int count)
        {
            ID = id;
            Count = count;
        }

        [JsonProperty(Required = Required.Always)]
        public DynamicID<ItemID, CategoryID> ID { get; set; }

        [JsonProperty(Required = Required.Always)]
        public int Count { get; set; }
    }
}