/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace PortableBridges.TerrainFeatures
{
    [XmlType("Mods_DecidedlyHumanBridgeFloor")]
    public class Bridge : Flooring
    {
        // public readonly NetInt whichFloor = new NetInt();
        // public readonly NetInt whichView = new NetInt();
        // public readonly NetBool isPathway = new NetBool();
        // public readonly NetBool isSteppingStone = new NetBool();
        // public readonly NetBool drawContouredShadow = new NetBool();
        // public readonly NetBool cornerDecoratedBorders = new NetBool();

        public Bridge()
        {
            this.whichFloor.Value = 2;
        }

        //public override bool isPassable(Character c = null)
        //{
        //    if (c is Farmer)
        //        return true;

        //    return false;
        //}

        public override void draw(SpriteBatch spriteBatch, Vector2 tileLocation)
        {
            base.draw(spriteBatch, tileLocation);
        }

        public override void performPlayerEntryAction(Vector2 tileLocation)
        {
            Game1.player.ignoreCollisions = true;
        }

        //public override void dayUpdate(GameLocation environment, Vector2 tileLocation)
        //{
        //    UpdateNeighbours(environment, tileLocation);
        //}

        //private void UpdateNeighbours(GameLocation environment, Vector2 tileLocation)
        //{
        //    //foreach (Vector2 direction in Directions.vector2)
        //    //{
        //    //    Vector2 checkTile = tileLocation + direction;

        //    //    if (environment.terrainFeatures.ContainsKey(checkTile))
        //    //    {
        //    //        if (environment.terrainFeatures[checkTile] is Bridge)
        //    //        {
        //    //            environment.setTileProperty((int)checkTile.X, (int)checkTile.Y, "Buildings", "Passable", "T");
        //    //            environment.setTileProperty((int)checkTile.X, (int)checkTile.Y, "Back", "Water", "F");
        //    //        }
        //    //        else
        //    //        {
        //    //            //if (environment.doesTileHavePropertyNoNull((int)checkTile.X, (int)checkTile.Y, "Passable", "Buildings").Equals(""))
        //    //            //{
        //    //            //    environment.setTileProperty((int)checkTile.X, (int)checkTile.Y, "Buildings", "Passable", "F");
        //    //            //}
        //    //        }
        //    //    }
        //    //}
        //}

        public override void doCollisionAction(Rectangle positionOfCollider, int speedOfCollision, Vector2 tileLocation,
            Character who, GameLocation location)
        {
            Game1.player.ignoreCollisions = true;
        }
    }
}
