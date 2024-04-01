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
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using static StardewValley.Minigames.TargetGame;

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

                if (!Game1.getFarm().TryGetMapPropertyAs("WarpTotemEntry", out Point farmWarp, required: false))
                {
                    farmWarp = new Point(default_x, default_y);
                }

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

            return new Vector2(-1);

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

            if(target == null)
            {

                return Vector2.Zero;

            }

            foreach (Warp reverse in target.warps)
            {
                
                if (reverse.TargetName == location.Name)
                {
                    
                    return new Vector2(reverse.TargetX, reverse.TargetY);

                }
                
            }

            return Vector2.Zero;

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

        public static Vector2 WarpStart(string defaultMap = "FarmCave")
        {

            if (defaultMap == "Follow")
            {

                defaultMap = "Farm";

            }

            switch (defaultMap)
            {

                case "Mountain":

                    return new Vector2(6176, 1728);

                case "18465_Crypt":

                    return new Vector2(1280, 448);

                case "Farm":
                case "FarmCave":


                    Dictionary<string, Vector2> farmPositions = new() { ["FarmCave"] = new Vector2(6, 6) * 64, ["Farm"] = Vector2.One * 64 };

                    foreach (Warp warp in Game1.getFarm().warps)
                    {

                        if (warp.TargetName == "FarmCave")
                        {

                            Vector2 cavePosition = new Vector2(warp.TargetX, warp.TargetY - 2) * 64;

                            Vector2 farmPosition = new Vector2(warp.X, warp.Y + 5) * 64;

                            farmPositions = new() { ["FarmCave"] = cavePosition, ["Farm"] = farmPosition, };

                        }

                    }

                    return farmPositions[defaultMap];

            }

            return new Vector2(-1);

        }

        public static Vector2 WarpEntrance(GameLocation targetLocation, Vector2 targetVector)
        {

            if (targetLocation == null)
            {

                return new Vector2(-1);

            }

            List<Vector2> destinations = new();

            float newDistance;

            float furthestDistance = 0f;

            List<string> surveyed = new();

            foreach (Warp warp in targetLocation.warps)
            {

                if (surveyed.Contains(warp.TargetName))
                {

                    continue;

                }

                surveyed.Add(warp.TargetName);

                Vector2 destination;

                if (WarpExclusions(targetLocation, warp))
                {

                    destination = WarpVectors(targetLocation);

                    if (destination == Vector2.Zero)
                    {

                        continue;

                    }

                }
                else
                {

                    destination = WarpReverse(targetLocation, warp);

                    if (destination == Vector2.Zero)
                    {

                        continue;

                    }

                }

                Vector2 possibility = destination * 64;

                if (destinations.Count == 0)
                {

                    destinations.Add(possibility);

                    furthestDistance = Vector2.Distance(targetVector, possibility);

                }
                else
                {

                    newDistance = Vector2.Distance(targetVector, possibility);

                    if (Mod.instance.rite.caster.getGeneralDirectionTowards(possibility, 0, false, false) == Mod.instance.rite.caster.facingDirection.Value && newDistance > furthestDistance)
                    {

                        destinations.Clear();

                        destinations.Add(possibility);

                    }

                }

            }

            if (destinations.Count > 0)
            {

                return destinations[0];

            }

            return new Vector2(-1);

        }

        public static Vector2 WarpXZone(GameLocation targetLocation, Vector2 targetVector)
        {

            if (targetLocation == null)
            {

                return new Vector2(-1);

            }

            Type reflectType = typeof(MineShaft);

            FieldInfo reflectField = reflectType.GetField("netTileBeneathLadder", BindingFlags.NonPublic | BindingFlags.Instance);

            var tile = reflectField.GetValue(targetLocation as MineShaft);

            if (tile == null)
            {

                return new Vector2(-1);
            }

            string tileString = tile.ToString();

            Match m = Regex.Match(tileString, @"\{*X\:(\d+)\sY\:(\d+)\}", RegexOptions.IgnoreCase);

            if (!m.Success)
            {

                return new Vector2(-1);

            }

            int tileX = Convert.ToInt32(m.Groups[1].Value);

            int tileY = Convert.ToInt32(m.Groups[2].Value);

            Vector2 destination = new Vector2(tileX, tileY) * 64;

            //ModUtility.AnimateQuickWarp(targetLocation, destination);

            return destination;

        }

    }

}
