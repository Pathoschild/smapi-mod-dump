using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using MapUtilities.Slope;
using xTile.Layers;

namespace MapUtilities
{
    class Farmer_MovePosition_Patch
    {
        public static bool Prefix(GameTime time, xTile.Dimensions.Rectangle viewport, GameLocation currentLocation, Farmer __instance)
        {
            try
            {
                Layer slope = currentLocation.map.GetLayer("Slope" + Pseudo3D.LevelHandler.getLevelSuffixForCharacter(__instance));
                if (slope != null && slope.Tiles[__instance.getTileX(), __instance.getTileY()] != null)
                {
                    SlopeHandler.modifyVelocity(__instance, slope.Tiles[__instance.getTileX(), __instance.getTileY()]);
                }
            }
            catch (NullReferenceException)
            {

            }
            string currentLevel = Pseudo3D.LevelHandler.getLevelForCharacter(__instance);
            string backLayer = "Back" + Pseudo3D.LevelHandler.getLevelSuffixForCharacter(__instance);
            if (currentLocation.map.GetLayer(backLayer).Tiles[__instance.getTileX(), __instance.getTileY()] != null)
            {
                xTile.Tiles.Tile currentTile = currentLocation.map.GetLayer(backLayer).Tiles[__instance.getTileX(), __instance.getTileY()];
                if (currentTile.Properties.ContainsKey("Level"))
                {
                    Logger.log("Checking level...");
                    xTile.ObjectModel.PropertyValue layerSwitch = currentLocation.map.GetLayer(backLayer).Tiles[__instance.getTileX(), __instance.getTileY()].Properties["Level"];
                    string layer = layerSwitch.ToString();
                    if (layer.Equals("0"))
                        layer = "Base";
                    if (!currentLevel.Equals(layer))
                    {
                        Logger.log("Applying level " + layer + "...");
                        //Pseudo3D.MapHandler.setPassableTiles(currentLocation, layer);
                        Pseudo3D.LevelHandler.setLevelForCharacter(__instance, layer);
                    }
                }
            }
            //Logger.log("Colliding position? " + currentLocation.isCollidingPosition(__instance.nextPosition(__instance.facingDirection), viewport, true, 0, false, (Character)__instance).ToString());
            return true;
        }
    }

    class Farmer_Update_Patch
    {
        public static void Postfix(GameTime time, GameLocation location, Farmer __instance)
        {
            Contact.TileContactHandler.sustainedContact(__instance, location, __instance.getTileX(), __instance.getTileY());
        }
    }
}
