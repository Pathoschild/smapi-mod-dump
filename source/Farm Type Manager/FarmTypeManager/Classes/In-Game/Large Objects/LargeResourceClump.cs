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
    public class LargeResourceClump : LargeTerrainFeature
    {
        [XmlElement("Clump")]
        public readonly NetRef<ResourceClump> Clump = new NetRef<ResourceClump>(new ResourceClump());

        public LargeResourceClump()
            : base(true)
        {
            NetFields.AddFields(Clump);

        }

        public LargeResourceClump(ResourceClump clump)
            : this()
        {
            Clump.Value = clump;
            tilePosition.Value = clump.tile.Value;
        }

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
