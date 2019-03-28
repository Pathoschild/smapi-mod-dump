using System.Collections.Generic;
using UltimateTool.Framework.Tools;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using SFarmer = StardewValley.Farmer;
using SObject = StardewValley.Object;

namespace UltimateTool.Framework
{
    internal abstract class BaseTool : ITool
    {
        public abstract bool IsEnabled(SFarmer who, Tool tool, Item item, GameLocation location);

        public abstract bool Apply(Vector2 tile, SObject tileObj, TerrainFeature tileFeature, SFarmer who, Tool tool, Item item, GameLocation location);

        protected bool UseToolOnTile(Tool tool, Vector2 tile)
        {
            Vector2 at = (tile * Game1.tileSize) + new Vector2(Game1.tileSize / 2f);
            tool.DoFunction(Game1.currentLocation, (int)at.X, (int)at.Y, 0, Game1.player);            
            return true;
        }

        protected bool TileAction(GameLocation location, Vector2 tile, SFarmer who)
        {
            return location.checkAction(new Location((int)tile.X, (int)tile.Y), Game1.viewport, who);
        }

        protected void RemoveItem(SFarmer who, Item item, int number = 1)
        {
            item.Stack -= 1;
            if(item.Stack <= 0)
            {
                who.removeItemFromInventory(item);
            }
        }

        protected Rectangle AbsoluteArea(Vector2 tile)
        {
            Vector2 position = tile * Game1.tileSize;
            return new Rectangle((int)position.X, (int)position.Y, Game1.tileSize, Game1.tileSize);
        }

        protected IEnumerable<ResourceClump> ResourceClumps(GameLocation location)
        {
            if(location is Farm farm)
            {
                return farm.resourceClumps;
            }
            if(location is Woods woods)
            {
                return woods.stumps;
            }
            return new ResourceClump[0];
        }

        protected ResourceClump ResourceClumpCoveringTile(GameLocation location, Vector2 tile)
        {
            Rectangle tArea = this.AbsoluteArea(tile);
            foreach(ResourceClump rc in this.ResourceClumps(location))
            {
                if (rc.getBoundingBox(rc.tile).Intersects(tArea))
                {
                    return rc;
                }
            }
            return null;
        }
    }
}