using Microsoft.Xna.Framework;
using UltimateTool.Framework.Configuration;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using SFarmer = StardewValley.Farmer;
using SObject = StardewValley.Object;

namespace UltimateTool.Framework.Tools
{
    internal class WateringCanTool : BaseTool
    {
        private readonly WateringCanConfig Config;

        public WateringCanTool(WateringCanConfig config)
        {
            this.Config = config;
        }

        public override bool IsEnabled(SFarmer who, Tool tool, Item item, GameLocation location)
        {
            return this.Config.Enabled && tool is WateringCan;
        }

        public override bool Apply(Vector2 tile, SObject tileObj, TerrainFeature tileFeature, SFarmer who, Tool tool, Item item, GameLocation location)
        {
            if(!(tileFeature is HoeDirt dirt) || dirt.state == HoeDirt.watered)
            {
                return false;
            }
            WateringCan can = (WateringCan)tool;
            int previousWater = can.WaterLeft;
            can.WaterLeft = 100;
            this.UseToolOnTile(tool, tile);
            can.WaterLeft = previousWater;
            return true;
        }
    }
}