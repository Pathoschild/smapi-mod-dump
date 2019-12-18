using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.TerrainFeatures;
using Netcode;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Network;

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
            return false; //NOTE: Clump.performToolAction is called elsewhere, because SDV only implements LargeTerrainFeature actions for axes
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
