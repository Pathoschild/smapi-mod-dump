using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using UltimateTool.Framework.Configuration;
using SFarmer = StardewValley.Farmer;
using SObject = StardewValley.Object;

namespace UltimateTool.Framework.Tools
{
    internal class GrassStarterTool : BaseTool
    {
        private readonly GrassStarterConfig _config;


        public GrassStarterTool(GrassStarterConfig config)
        {
            _config = config;
        }

        public override bool IsEnabled(SFarmer who, Tool tool, Item item, GameLocation location)
        {
            return _config.Enabled && item?.ParentSheetIndex == 297;
        }

        public override bool Apply(Vector2 tile, SObject tileObj, TerrainFeature tileFeature, SFarmer who, Tool tool, Item item, GameLocation location)
        {
            if(!(item is SObject @object) || @object.Stack <= 0)
            {
                return false;
            }

            if(@object.canBePlacedHere(location, tile) && @object.placementAction(location, (int)(tile.X * Game1.tileSize), (int)(tile.Y * Game1.tileSize), who))
            {
                RemoveItem(who, item);
                return true;
            }
            return false;
        }
    }
}
