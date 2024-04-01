/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TheMightyAmondee/SkullCavernToggle
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using xTile.Layers;
using xTile.Tiles;

namespace SkullCavernToggle
{
    public class Shrine 
        : GameLocation
    {
        // Apply shrine tiles to map
        public void ApplyTiles(IModHelper helper, bool multiplayerpatch = false)
        {
            // Get tilesheet pathway
            string tilesheetPath = helper.ModContent.GetInternalAssetName("assets\\snake_shrine.png").Name;

            // Get skullcave location
            GameLocation location = Game1.getLocationFromName("SkullCave");

            // Get tilesheet from pathway
            TileSheet tilesheet = new TileSheet(
                  id: "z_shrine_tilesheet",
                  map: location.map,
                  imageSource: tilesheetPath,
                  sheetSize: new xTile.Dimensions.Size(16, 48),
                  tileSize: new xTile.Dimensions.Size(16, 16)
               );

            // Load tilesheet
            location.map.AddTileSheet(tilesheet);
            location.map.LoadTileSheets(Game1.mapDisplayDevice);

            // Get required layers
            Layer frontlayer = location.map.GetLayer("Front");
            Layer buildingslayer = location.map.GetLayer("Buildings");

            // Which snake head to use
            if (Game1.netWorldState.Value.SkullCavesDifficulty == 0)
            {
                if (multiplayerpatch == true)
                {
                    // Dangerous state, red eyes
                    frontlayer.Tiles[2, 2] = new StaticTile(frontlayer, tilesheet, BlendMode.Alpha, 48);
                }
                else
                {
                    // Normal state, yellow eyes
                    frontlayer.Tiles[2, 2] = new StaticTile(frontlayer, tilesheet, BlendMode.Alpha, 0);
                }

            }
            else
            {
                if (multiplayerpatch == true)
                {
                    // Normal state, yellow eyes
                    frontlayer.Tiles[2, 2] = new StaticTile(frontlayer, tilesheet, BlendMode.Alpha, 0);
                }
                else
                {
                    // Dangerous state, red eyes 
                    frontlayer.Tiles[2, 2] = new StaticTile(frontlayer, tilesheet, BlendMode.Alpha, 48);
                }
            }
            
            // Apply other tiles for shrine
            frontlayer.Tiles[2, 3] = new StaticTile(frontlayer, tilesheet, BlendMode.Alpha, 16);
            buildingslayer.Tiles[2, 4] = new StaticTile(buildingslayer, tilesheet, BlendMode.Alpha, 32);

            // Apply properties to tiles            
            location.setTileProperty(2, 3, "Buildings", "Action", "SnakeShrine");
            location.setTileProperty(2, 4, "Buildings", "Action", "SnakeShrine");
        }
    }
}
