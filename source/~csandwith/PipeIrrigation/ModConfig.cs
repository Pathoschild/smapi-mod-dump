/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/


using Microsoft.Xna.Framework;
using StardewModdingAPI;

namespace PipeIrrigation
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public bool WaterSurroundingTiles { get; set; } = true;
        public bool ShowSprinklerAnimations { get; set; } = true;
        public bool ShowWateredTilesLabelOnGrid { get; set; } = true;
        public int PercentWaterPerTile { get; set; } = 25;
    }
}
