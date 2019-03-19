using Microsoft.Xna.Framework;
using UltimateTool.Framework.Configuration;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using SFarmer = StardewValley.Farmer;
using SObject = StardewValley.Object;

namespace UltimateTool.Framework.Tools
{
    internal class FertilizerTool : BaseTool
    {
        private readonly FertilizerConfig Config;

        public FertilizerTool(FertilizerConfig config)
        {
            this.Config = config;
        }

        public override bool IsEnabled(SFarmer who, Tool tool, Item item, GameLocation location)
        {
            return this.Config.Enabled && item?.category == SObject.fertilizerCategory;
        }

        public override bool Apply(Vector2 tile, SObject tileObj, TerrainFeature tileFeature, SFarmer who, Tool tool, Item item, GameLocation location)
        {
            if(item == null)
            {
                return false;
            }
            if(!(tileFeature is HoeDirt dirt) || dirt.fertilizer != HoeDirt.noFertilizer)
            {
                return false;
            }
            if(this.ResourceClumpCoveringTile(location, tile) != null)
            {
                return false;
            }
            dirt.fertilizer = item.parentSheetIndex;
            this.RemoveItem(who, item);
            return true;
        }
    }
}
