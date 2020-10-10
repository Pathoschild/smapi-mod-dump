/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using xTile.Dimensions;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using SObject = StardewValley.Object;

namespace AllMightyTool.Framework.Tools
{
    internal abstract class BaseTool : ITool
    {
        protected IReflectionHelper reflection { get; }

        /// <summary>Apply the tool to the given tile.</summary>
        /// <param name="tile">The tile to modify.</param>
        /// <param name="obj">The object on the tile.</param>
        /// <param name="terrain">The feature on the tile.</param>
        /// <param name="player">The current player.</param>
        /// <param name="tool">The tool selected by the player (if any).</param>
        /// <param name="item">The item selected by the player (if any).</param>
        /// <param name="location">The current location.</param>
        public abstract bool Activate(Vector2 tile, SObject obj, TerrainFeature terrain, Farmer player, Tool tool, Item item, GameLocation location);

        protected BaseTool(IReflectionHelper _reflection)
        {
            reflection = _reflection;
        }

        protected bool UseTool(Tool tool, Vector2 tileLoc)
        {
            Vector2 use = (tileLoc * Game1.tileSize) + new Vector2(Game1.tileSize / 2f);
            Game1.player.lastClick = use;
            tool.DoFunction(Game1.currentLocation, (int)use.X, (int)use.Y, 0, Game1.player);
            return true;
        }

        protected bool CheckAction(GameLocation loc, Vector2 tile, Farmer who)
        {
            return loc.checkAction(new Location((int) tile.X, (int) tile.Y), Game1.viewport, who);
        }

        protected bool weed(SObject obj)
        {
            return !(obj is Chest) && obj?.Name == "Weeds";
        }

        protected void UseItem(Farmer who, Item item, int count = 1)
        {
            item.Stack -= 1;
            if(item.Stack <= 0)
                who.removeItemFromInventory(item);
        }

        protected Rectangle AbsoluteTileArea(Vector2 tile)
        {
            Vector2 pos = tile * Game1.tileSize;
            return new Rectangle((int) pos.X, (int) pos.Y, Game1.tileSize, Game1.tileSize);
        }

        protected IEnumerable<ResourceClump> ResourceClumps(GameLocation loc)
        {
            switch (loc)
            {
                case Farm farm:
                    return farm.resourceClumps;
                case Forest forest:
                    return forest.log != null ? new[] { forest.log } : new ResourceClump[0];
                case Woods woods:
                    return woods.stumps;
                case MineShaft mshaft:
                    return mshaft.resourceClumps;
                default:
                    if (loc.Name == "DeepWoods")
                        return this.reflection.GetField<IList<ResourceClump>>(loc, "resourceClumps", required: false)?.GetValue() ?? new ResourceClump[0];
                    return new ResourceClump[0];
            }
        }

        protected ResourceClump ResourceClumpConveringTile(GameLocation loc, Vector2 tile)
        {
            Rectangle tArea = this.AbsoluteTileArea(tile);
            foreach (ResourceClump clump in this.ResourceClumps(loc))
            {
                if (clump.getBoundingBox(clump.tile.Value).Intersects(tArea))
                    return clump;
            }

            return null;
        }

        protected bool TryGetHoeDirt(TerrainFeature terrain, SObject obj, out HoeDirt dirt, out bool isCoveredByObj)
        {
            //Check to see if it's a garden pot
            if (obj is IndoorPot pot)
            {
                dirt = pot.hoeDirt.Value;
                isCoveredByObj = false;
                return true;
            }
            //Check to see if it's normal dirt
            if ((dirt = terrain as HoeDirt) != null)
            {
                isCoveredByObj = obj != null;
                return true;
            }

            //Found nothing
            dirt = null;
            isCoveredByObj = false;
            return false;
        }
    }
}
