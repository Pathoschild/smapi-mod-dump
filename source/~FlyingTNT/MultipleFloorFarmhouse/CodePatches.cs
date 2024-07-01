/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FlyingTNT/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiStoryFarmhouse
{
    public class CodePatches
    {
        public static void FarmHouse_resetLocalState_Prefix(ref Vector2 __state)
        {
            __state = new Vector2(-1, -1);
            if (Game1.isWarping && Game1.player.previousLocationName == "MultipleFloors0")
            {
                __state = new Vector2(Game1.xLocationAfterWarp, Game1.yLocationAfterWarp);
            }
        }

        public static void FarmHouse_resetLocalState_Postfix(Vector2 __state)
        {
            if (__state.X >= 0)
            {
                Game1.player.Position = __state * 64f;
                Game1.xLocationAfterWarp = Game1.player.TilePoint.X;
                Game1.yLocationAfterWarp = Game1.player.TilePoint.Y;
            }
        }

        public static void SaveGame_loadDataToLocations_Prefix()
        {
            try
            {
                ModEntry.SMonitor.Log($"Checking save for multiple floors");

                List<string> possibleFloors = ModEntry.GetPossibleFloors();

                for (int i = 0; i < possibleFloors.Count; i++)
                {
                    string floorName = possibleFloors[i];
                    DecoratableLocation location = (DecoratableLocation)Game1.locations.FirstOrDefault(l => l.Name == $"MultipleFloors{i}");
                    if (location == null)
                    {
                        ModEntry.SMonitor.Log($"adding floor MultipleFloors{i}");
                        location = new DecoratableLocation($"Maps/MultipleFloorsMap{i}", $"MultipleFloors{i}");

                        Game1.locations.Add(location);
                    }
                    else
                        ModEntry.SMonitor.Log($"Game already has floor MultipleFloors{i}");
                }
            }
            catch(Exception ex)
            {
                ModEntry.SMonitor.Log($"Failed in {nameof(SaveGame_loadDataToLocations_Prefix)} {ex}", StardewModdingAPI.LogLevel.Error);
            }
        }

        public static bool DecorableLocation_getFloors_Prefix(DecoratableLocation __instance, ref List<Rectangle> __result)
        {
            if (!__instance.Name.StartsWith("MultipleFloors"))
                return true;

            if(!ModEntry.TryGetFloor(__instance.Name, out Floor floor))
            {
                ModEntry.SMonitor.Log($"Could not get floor {__instance.Name} for flooring!", StardewModdingAPI.LogLevel.Debug);
                return true;
            }

            __result = floor.floors;

            return false;
        }

        public static bool GameLocation_getWalls_Prefix(DecoratableLocation __instance, ref List<Rectangle> __result)
        {
            if (!__instance.Name.StartsWith("MultipleFloors"))
                return true;

            if (!ModEntry.TryGetFloor(__instance.Name, out Floor floor))
            {
                ModEntry.SMonitor.Log($"Could not get floor {__instance.Name} for walls!", StardewModdingAPI.LogLevel.Debug);
                return true;
            }

            __result = floor.walls;

            return false;
        }
        
        public static bool GameLocation_CanPlaceThisFurnitureHere_Prefix(GameLocation __instance, ref bool __result, Furniture furniture)
        {
            if (!__instance.Name.StartsWith("MultipleFloors") || furniture is null)
                return true;

            __result = true;
            return false;
        }
    }
}