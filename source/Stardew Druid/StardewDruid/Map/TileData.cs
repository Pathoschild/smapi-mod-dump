/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Dimensions;
using xTile.Tiles;

namespace StardewDruid.Map
{
    internal class TileData
    {

        public static void LoadSheets()
        {

            //-------------------------- Effigy Tiles

            GameLocation farmCave = Game1.getLocationFromName("FarmCave");

            string craftableTiles = Path.Combine("TileSheets", "Craftables"); // preloading tilesheet

            if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "Content", craftableTiles + ".xnb")))
            {

                TileSheet craftableSheet = new("craftableTiles", farmCave.map, craftableTiles, new xTile.Dimensions.Size(8, 72), new xTile.Dimensions.Size(1, 1));

                farmCave.map.AddTileSheet(craftableSheet);


            }

            string witchTiles = Path.Combine("Maps", "WitchHutTiles");

            if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "Content", witchTiles + ".xnb")))
            {

                TileSheet witchSheet = new("witchHutTiles", farmCave.map, witchTiles, new xTile.Dimensions.Size(8, 16), new xTile.Dimensions.Size(1, 1));

                farmCave.map.AddTileSheet(witchSheet);


            }


            // ------------------------- Desert Tiles
            /*
            GameLocation desert;

            for (int i = 0; i < Game1.locations.Count; i++)
            {

                if (Game1.locations[i] is Desert)
                {

                    desert = Game1.locations[i];

                    string furnitureTiles = Path.Combine("TileSheets", "Furniture");

                    if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "Content", furnitureTiles + ".xnb")))
                    {

                        TileSheet furnitureSheet = new("furnitureTiles", desert.map, furnitureTiles, new xTile.Dimensions.Size(16, 16), new xTile.Dimensions.Size(1, 1));

                        desert.map.AddTileSheet(furnitureSheet);

                    }

                    break;

                }

            }*/

        }

    }
}
