using Microsoft.Xna.Framework;
using UltimateTool.Framework.Configuration;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using SFarmer = StardewValley.Farmer;
using SObject = StardewValley.Object;

namespace UltimateTool.Framework.Tools
{
    internal class HoeTool : BaseTool
    {
        private readonly HoeConfig Config;

        public HoeTool(HoeConfig config)
        {
            this.Config = config;
        }

        public override bool IsEnabled(SFarmer who, Tool tool, Item item, GameLocation location)
        {
            return this.Config.TillDirt && tool is Hoe;
        }

        public override bool Apply(Vector2 tile, SObject tileObj, TerrainFeature tileFeature, SFarmer who, Tool tool, Item item, GameLocation location)
        {
            if(tileFeature == null && tileObj == null)
            {
                return this.UseToolOnTile(tool, tile);
            }
            if(tileObj?.Name == "Artifact Spot")
            {
                return this.UseToolOnTile(tool, tile);
            }
            return false;
        }
    }
}
