using UltimateTool.Framework.Configuration;
using SFarmer = StardewValley.Farmer;
using SObject = StardewValley.Object;

namespace UltimateTool.Framework.Tools
{
    internal class SeedTool : BaseTool
    {
        private readonly SeedsConfig Config;

        public SeedTool(SeedsConfig config)
        {
            this.Config = config;
        }

        public override bool IsEnabled(SFarmer who, Tool tool, Item item, GameLocation location)
        {
            return this.Config.Enabled && item?.category == SObject.SeedsCategory;
        }

        public override bool Apply(Vector2 tile, SObject tileObj, TerrainFeature tileFeature, SFarmer who, Tool tool, Item item, GameLocation location)
        {
            if(item == null || item.Stack <= 0)
            {
                return false;
            }

            if(!(tileFeature is HoeDirt dirt) || dirt.crop != null)
            {
                return false;
            }
            bool planted = dirt.plant(item.parentSheetIndex, (int)tile.X, (int)tile.Y, who);
            if (planted)
            {
                this.RemoveItem(who, item);
            }
            return planted;
        }
    }
}