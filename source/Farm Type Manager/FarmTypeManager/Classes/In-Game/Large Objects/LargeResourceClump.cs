/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using StardewValley.TerrainFeatures;
using System.Xml.Serialization;

namespace FarmTypeManager
{
    /// <summary>A large terrain feature that wraps a resource clump for use in GameLocations that don't support resource clumps.</summary>
    /// <remarks>This class is not currently designed to be saved by the game's native processes. All instances should be removed from the game before saving (i.e. end of day).</remarks>
    public class LargeResourceClump : LargeTerrainFeature
    {
        /// <summary>The resource clump wrapped by this large terrain feature.</summary>
        [XmlElement("Clump")]
        public readonly NetRef<ResourceClump> Clump = new NetRef<ResourceClump>(new ResourceClump());

        public LargeResourceClump()
            : base(true)
        {
            NetFields.AddFields(Clump);

        }

        /// <summary>Create a new large terrain feature wrapping the provided resouce clump.</summary>
        /// <param name="clump">The resource clump to be wrapped. The clump's tile value will be used as this feature's tile position.</param>
        public LargeResourceClump(ResourceClump clump)
            : this()
        {
            Clump.Value = clump;
            tilePosition.Value = clump.tile.Value;
        }

        /// <summary>Create a new large terrain feature wrapping a new resource clump with the provided settings.</summary>
        /// <param name="parentSheetIndex">The resource clump's index value. Refer to the ResourceClump class for valid options.</param>
        /// <param name="width">The resource clump's width.</param>
        /// <param name="height">The resource clump's height.</param>
        /// <param name="tile">The resource clump's X/Y tile location.</param>
        public LargeResourceClump(int parentSheetIndex, int width, int height, Vector2 tile)
            : this()
        {
            Clump.Value = new ResourceClump(parentSheetIndex, width, height, tile);
            tilePosition.Value = tile;
        }

        public override bool isPassable(Character c = null)
        {
            return Clump.Value.isPassable(c);
        }

        public override bool performToolAction(Tool t, int damage, Vector2 tileLocation, GameLocation location)
        {
            bool result = Clump.Value.performToolAction(t, damage, this.tilePosition, location); //hit the clump and get the result (note: passing tileLocation would displace the destruction animation)

            if (result) //if the clump's method returned true, it was harvested (i.e. broken)
                location.largeTerrainFeatures.Remove(this); //remove this from the location

            return false; //return false (to discourage tool code from trying to remove this)
        }

        new public Rectangle getBoundingBox() //this overrides accidental use of LargeTerrainFeature.getBoundingBox() where possible
        {
            return getBoundingBox(tilePosition);
        }

        public override Rectangle getBoundingBox(Vector2 tileLocation)
        {
            return Clump.Value.getBoundingBox(tileLocation);
        }

        public override void draw(SpriteBatch spriteBatch, Vector2 tileLocation)
        {
            Clump.Value.draw(spriteBatch, tileLocation);
        }

        public override bool performUseAction(Vector2 tileLocation, GameLocation location)
        {
            return Clump.Value.performUseAction(tileLocation, location);
        }

        public override bool tickUpdate(GameTime time, Vector2 tileLocation, GameLocation location)
        {
            return Clump.Value.tickUpdate(time, tileLocation, location);
        }

        public override bool isActionable()
        {
            return Clump.Value.isActionable();
        }
    }
}
