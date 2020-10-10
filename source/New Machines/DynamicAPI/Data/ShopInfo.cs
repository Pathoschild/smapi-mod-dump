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
using System.Linq;
using Newtonsoft.Json;
using StardewValley;

namespace Igorious.StardewValley.DynamicAPI.Data
{
    public sealed class ShopInfo
    {
        [JsonConstructor]
        public ShopInfo() {}

        public ShopInfo(string location, params Item[] items)
        {
            Location = location;
            Items = items.ToList();
        }

        [JsonProperty(Required = Required.Always)]
        public string Location { get; set; }

        [JsonProperty(Required = Required.Always)]
        public List<Item> Items { get; set; }
    }
}