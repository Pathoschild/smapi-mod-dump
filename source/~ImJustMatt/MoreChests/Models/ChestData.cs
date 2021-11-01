/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace MoreChests.Models
{
    using System.Collections.Generic;

    internal class ChestData : ChestConfig
    {
        public int Depth { get; set; }
        public string Description { get; set; }
        public HashSet<string> DisabledFeatures { get; set; }
        public string DisplayName { get; set; }
        public Dictionary<string, bool> FilterItems { get; set; }
        public int Frames { get; set; }
        public string Image { get; set; }
        public float OpenNearby { get; set; }
        public bool PlayerColor { get; set; }
        public bool PlayerConfig { get; set; }
    }
}