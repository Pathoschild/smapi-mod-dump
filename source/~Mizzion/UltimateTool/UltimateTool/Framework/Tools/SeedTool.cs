using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using UltimateTool.Framework.Configuration;
using SFarmer = StardewValley.Farmer;
using SObject = StardewValley.Object;

namespace UltimateTool.Framework.Tools
{
    internal class SeedTool : BaseTool
    {
        private readonly SeedsConfig _config;

        public SeedTool(SeedsConfig config)
        {
            _config = config;
        }

        public override bool IsEnabled(SFarmer who, Tool tool, Item item, GameLocation location)
        {
            return _config.Enabled && item?.Category == SObject.SeedsCategory;
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
            bool planted = dirt.plant(item.ParentSheetIndex, (int)tile.X, (int)tile.Y, who, false, location);
            if (planted)
            {
                RemoveItem(who, item);
            }
            return planted;
        }
    }
}