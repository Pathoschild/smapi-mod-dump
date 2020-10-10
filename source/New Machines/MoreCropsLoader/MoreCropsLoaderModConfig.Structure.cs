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
using Igorious.StardewValley.DynamicAPI;
using Igorious.StardewValley.DynamicAPI.Data;

namespace Igorious.StardewValley.MoreCropsLoader
{
    public sealed partial class MoreCropsLoaderModConfig : DynamicConfiguration
    {
        public List<ItemInformation> Items { get; set; } = new List<ItemInformation>();
        public List<TreeInformation> Trees { get; set; } = new List<TreeInformation>();
        public List<CropInformation> Crops { get; set; } = new List<CropInformation>();
    }
}
