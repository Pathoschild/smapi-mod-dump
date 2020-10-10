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
    internal class WateringCanTool : BaseTool
    {
        private readonly WateringCanConfig _config;

        public WateringCanTool(WateringCanConfig config)
        {
            _config = config;
        }

        public override bool IsEnabled(SFarmer who, Tool tool, Item item, GameLocation location)
        {
            return _config.Enabled && tool is WateringCan;
        }

        public override bool Apply(Vector2 tile, SObject tileObj, TerrainFeature tileFeature, SFarmer who, Tool tool, Item item, GameLocation location)
        {
            if(!(tileFeature is HoeDirt dirt) || dirt.state.Value == HoeDirt.watered)
            {
                return false;
            }
            WateringCan can = (WateringCan)tool;
            int previousWater = can.WaterLeft;
            can.WaterLeft = 100;
            UseToolOnTile(tool, tile);
            can.WaterLeft = previousWater;
            return true;
        }
    }
}