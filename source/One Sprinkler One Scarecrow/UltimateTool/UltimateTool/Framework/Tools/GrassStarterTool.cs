using Microsoft.Xna.Framework;
using UltimateTool.Framework.Configuration;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using SFarmer = StardewValley.Farmer;
using SObject = StardewValley.Object;

namespace UltimateTool.Framework.Tools
{
    internal class GrassStarterTool : BaseTool
    {
        private readonly GrassStarterConfig Config;


        public GrassStarterTool(GrassStarterConfig config)
        {
            this.Config = config;
        }

        public override bool IsEnabled(SFarmer who, Tool tool, Item item, GameLocation location)
        {
            return this.Config.Enabled && item?.parentSheetIndex == 297;
        }

        public override bool Apply(Vector2 tile, SObject tileObj, TerrainFeature tileFeature, SFarmer who, Tool tool, Item item, GameLocation location)
        {
            if(!(item is SObject @object) || @object.Stack <= 0)
            {
                return false;
            }

            if(@object.canBePlacedHere(location, tile) && @object.placementAction(location, (int)(tile.X * Game1.tileSize), (int)(tile.Y * Game1.tileSize), who))
            {
                this.RemoveItem(who, item);
                return true;
            }
            return false;
        }
    }
}
