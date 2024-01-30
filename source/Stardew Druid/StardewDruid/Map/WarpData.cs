/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using System.Collections.Generic;

namespace StardewDruid.Map
{
    static class WarpData
    {

        public static Vector2 WarpVectors(GameLocation location)
        {

            /*Point farmWarp = Game1.getFarm().GetMapPropertyPosition("WarpTotemEntry", default_x, default_y);
            
            Dictionary<string, Vector2> warpPoints = new()
            {
                ["Farm"] = new Vector2(farmWarp.X, farmWarp.Y),
                ["Mountain"] = new Vector2(31, 20),
                ["Beach"] = new Vector2(20, 4),
                ["Desert"] = new Vector2(35, 43),
                ["IslandSouth"] = new Vector2(11, 11)
            };

            return warpPoints;*/

            if (location is Farm)
            {

                int default_x = 48;
                int default_y = 7;

                if (Game1.whichFarm == 5)
                {
                    default_x = 48;
                    default_y = 39;
                }
                else if (Game1.whichFarm == 6)
                {
                    default_x = 82;
                    default_y = 29;
                }

                Point farmWarp = Game1.getFarm().GetMapPropertyPosition("WarpTotemEntry", default_x, default_y);

                return new Vector2(farmWarp.X, farmWarp.Y);

            }
            else if (location is Mountain)
            {

                return new Vector2(31, 20);

            }
            else if (location is Beach)
            {

                return new Vector2(20, 4);

            }
            else if (location is Desert)
            {

                return new Vector2(35, 43);

            }
            else if (location is IslandSouth)
            {

                return new Vector2(11, 11);

            }

            return Vector2.Zero;

        }

        public static bool WarpExclusions(GameLocation location, Warp warp)
        {

            Dictionary<string, List<string>> exclusionWarps = new()
            {

                ["Forest"] = new() { "Beach", },
                ["Beach"] = new() { "Town", "Forest", },
                ["Town"] = new() { "Beach", "Mountain", },
                ["Backwoods"] = new() { "BusStop", },
                ["Mountain"] = new() { "Town" },

            };

            if (exclusionWarps.ContainsKey(location.Name))
            {
                if (exclusionWarps[location.Name].Contains(warp.TargetName))
                {

                    return true;

                }

            }

            return false;

        }

        public static Vector2 WarpReverse(GameLocation location, Warp warp)
        {

            if(location is Shed || location is AnimalHouse)
            {

                return new Vector2(warp.X, warp.Y);

            }

            GameLocation target = Game1.getLocationFromName(warp.TargetName);

            foreach (Warp reverse in target.warps)
            {
                
                if (reverse.TargetName == location.Name)
                {
                    
                    return new Vector2(reverse.TargetX, reverse.TargetY);

                }
                
            }

            return new Vector2(0);

        }

        public static int WarpTotems(GameLocation location)
        {
            /*Dictionary<string, int> warpTotems = new()
            {
                ["Farm"] = 688,
                ["Mountain"] = 689,
                ["Beach"] = 690,
                ["Desert"] = 261,
                ["IslandSouth"] = 886,
            };

            return warpTotems;*/

            if (location is Farm)
            {

                return 688;

            }
            else if (location is Mountain)
            {

                return 689;

            }
            else if (location is Beach)
            {

                return 690;

            }
            else if (location is Desert)
            {

                return 261;

            }

            return 886;


        }

    }
}
