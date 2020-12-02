/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Speshkitty/CustomiseChildBedroom
**
*************************************************/

using StardewValley;
using StardewValley.Locations;
using xTile.Tiles;

namespace CustomiseChildBedroom
{
    static class BuildingExtensions
    {
        internal static int GetLevel(this FarmHouse farmHouse)
        {
            if (farmHouse is Cabin)
            {
                return (farmHouse as Cabin).upgradeLevel;
            }
            else
            {
                return Game1.player.HouseUpgradeLevel;
            }
        }
        internal static string GetOwner(this FarmHouse farmHouse)
        {
            if(farmHouse is Cabin)
            {
                return (farmHouse as Cabin).owner.Name;
            }
            else
            {
                return Game1.player.Name;
            }
        }

        internal static void RemoveCrib(this FarmHouse building)
        {
            building.removeTiles("Buildings", 15, 3, 17, 5); //Get rid of the tiles from the building layer
            building.removeTiles("Front", 15, 2, 17, 4); //Get rid of the tiles from the front layer

            int wallpaper = building.getTileIndexAt(18, 3, "Buildings"); //get the wallpaper id so we can plaster it onto the tiles we're adding
            int flooring = building.getTileIndexAt(19, 3, "Back"); //get the flooring id too

            StaticTile WallpaperTile = new StaticTile(building.Map.GetLayer("Buildings"), building.Map.GetTileSheet("walls_and_floors"), BlendMode.Alpha, wallpaper);
            StaticTile FlooringTile = new StaticTile(building.Map.GetLayer("Back"), building.Map.GetTileSheet("walls_and_floors"), BlendMode.Alpha, flooring);

            //Add wall tiles to 15 3, 16 3, and 17 3 on the Buildings layer, which were previously occupied by the crib
            building.Map.GetLayer("Buildings").Tiles[15, 3] = WallpaperTile;
            building.Map.GetLayer("Buildings").Tiles[16, 3] = WallpaperTile;
            building.Map.GetLayer("Buildings").Tiles[17, 3] = WallpaperTile;

            //Add fooring tiles to the back layer where the crib was -- we'll have a weird black line there otherwise
            building.Map.GetLayer("Back").Tiles[15, 3] = FlooringTile;
            building.Map.GetLayer("Back").Tiles[16, 3] = FlooringTile;
            building.Map.GetLayer("Back").Tiles[17, 3] = FlooringTile;
        }

        internal static void RemoveLeftBed(this FarmHouse building)
        {
            removeTiles(building, "Buildings", 22, 4, 23, 6); //Get rid of the tiles from the building layer                    
            removeTiles(building, "Front", 22, 3, 23, 5); //Get rid of the tiles from the front layer
        }

        internal static void RemoveRightBed(this FarmHouse building)
        {
            removeTiles(building, "Buildings", 26, 4, 27, 6); //Get rid of the tiles from the building layer                    
            removeTiles(building, "Front", 26, 3, 27, 5); //Get rid of the tiles from the front layer
        }

        private static void removeTiles(this FarmHouse location, string layer, int startX, int startY, int endX, int endY)
        {
            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    location.removeTile(x, y, layer);
                }
            }
        }
    }
}
