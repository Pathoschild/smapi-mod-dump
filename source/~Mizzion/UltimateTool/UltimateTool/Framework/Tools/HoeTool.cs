/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using UltimateTool.Framework.Configuration;
using SFarmer = StardewValley.Farmer;
using SObject = StardewValley.Object;

namespace UltimateTool.Framework.Tools
{
    internal class HoeTool : BaseTool
    {
        private readonly HoeConfig _config;

        public HoeTool(HoeConfig config)
        {
            _config = config;
        }

        public override bool IsEnabled(SFarmer who, Tool tool, Item item, GameLocation location)
        {
            return _config.TillDirt && tool is Hoe;
        }

        public override bool Apply(Vector2 tile, SObject tileObj, TerrainFeature tileFeature, SFarmer who, Tool tool, Item item, GameLocation location)
        {
            if(tileFeature == null && tileObj == null)
            {
                return UseToolOnTile(tool, tile);
            }
            if(tileObj?.Name == "Artifact Spot")
            {
                return UseToolOnTile(tool, tile);
            }
            return false;
        }
    }
}
