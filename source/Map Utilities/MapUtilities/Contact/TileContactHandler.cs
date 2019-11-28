using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using Microsoft.Xna.Framework;
using xTile;
using xTile.Layers;

namespace MapUtilities.Contact
{
    public static class TileContactHandler
    {
        //This method runs every frame, being passed the current coordinates of the player
        public static void sustainedContact(Farmer who, GameLocation location, int x, int y)
        {
            Layer velocity = location.map.GetLayer("Velocity" + Pseudo3D.LevelHandler.getLevelSuffixForCharacter(who));
            if (velocity != null && velocity.Tiles[x,y] != null)
            {
                Velocity.VelocityHandler.updateVelocity(who, velocity.Tiles[x,y]);
            }
            Layer back = location.map.GetLayer("Back" + Pseudo3D.LevelHandler.getLevelSuffixForCharacter(who));
            if (back != null && back.Tiles[x,y] != null)
            {
                xTile.Tiles.Tile currentTile = back.Tiles[x,y];
                if (currentTile.Properties.ContainsKey("Pain"))
                {
                    try
                    {
                        string painInfo = currentTile.Properties["Pain"].ToString();
                        string[] pain = painInfo.Split(' ');
                        int damage = Convert.ToInt32(pain[0]);
                        int cooldown = Convert.ToInt32(pain[1]);
                        Contact.PainTileHandler.damagePlayer(who, damage, cooldown);
                    }
                    catch (FormatException e)
                    {
                        Logger.log("Problem in pain tile at [" + x + ", " + y + "]!  Definition was not in the correct format: " + currentTile.Properties["Pain"].ToString(), StardewModdingAPI.LogLevel.Error);
                    }
                }
                if (currentTile.Properties.ContainsKey("Morph"))
                {
                    TileContactMorphHandler.performSustainedMorph(location, x, y, currentTile);
                }
            }
        }

        public static void makeContact(Farmer who, GameLocation location, int x, int y)
        {
            try
            {
                Layer slope = location.map.GetLayer("Slope" + Pseudo3D.LevelHandler.getLevelSuffixForCharacter(who));
                if (slope != null && slope.Tiles[x,y] != null)
                {
                    Slope.SlopeHandler.modifyVelocity(who, slope.Tiles[x,y]);
                }
            } catch (NullReferenceException){}
            string currentLevel = Pseudo3D.LevelHandler.getLevelForCharacter(who);
            string backLayer = "Back" + Pseudo3D.LevelHandler.getLevelSuffixForCharacter(who);
            if (location.map.GetLayer(backLayer).Tiles[x,y] != null)
            {
                xTile.Tiles.Tile currentTile = location.map.GetLayer(backLayer).Tiles[x,y];
                if (currentTile.Properties.ContainsKey("Level"))
                {
                    Logger.log("Checking level...");
                    string layer = location.map.GetLayer(backLayer).Tiles[x,y].Properties["Level"].ToString();
                    if (layer.Equals("0"))
                        layer = "Base";
                    if (!currentLevel.Equals(layer))
                    {
                        Logger.log("Applying level " + layer + "...");
                        //Pseudo3D.MapHandler.setPassableTiles(currentLocation, layer);
                        Pseudo3D.LevelHandler.setLevelForCharacter(who, layer);
                    }
                }
            }
        }


    }
}
