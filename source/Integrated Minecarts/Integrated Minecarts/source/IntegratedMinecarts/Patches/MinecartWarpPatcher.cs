/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Jibblestein/StardewMods
**
*************************************************/

using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using StardewModdingAPI;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System.Reflection.Emit;
using System.Reflection;
using StardewValley.Menus;
using StardewValley.GameData.Minecarts;
using System.Runtime.CompilerServices;
using xTile;
using static StardewValley.Minigames.TargetGame;

namespace IntegratedMinecarts.Patches
{
    public static class MinecartWarpPatcher
    {
        private static IMonitor? Monitor;
        // call this method from your Entry class
        public static void Patch(ModEntry mod)
        {
            Monitor = mod.Monitor;

            try
            {
                mod.Harmony!.Patch(
                   original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.MinecartWarp)),
                   prefix: new HarmonyMethod(typeof(MinecartWarpPatcher), nameof(MinecartWarp_Prefix))
                    );

            }
            catch (Exception ex)
            {
                Monitor.Log($"An error occurred while registering a harmony patch for GameLocation. \n{ex}", LogLevel.Warn);
            }
        }

        internal static bool MinecartWarp_Prefix(GameLocation __instance, MinecartDestinationData destination)
        {
            string currentlocation = Game1.player.currentLocation.Name;
            try
            {
                if (currentlocation == "BusStop" && destination.TargetLocation == "Desert")
                {
                    GameLocation target = Game1.getLocationFromName(destination.TargetLocation);
                    Monitor!.Log($"Changing LocationContext from Location:{target.Name}:{target.locationContextId}", LogLevel.Trace);
                    target.locationContextId = "Default";
                    Monitor.Log($"Changed to Location:{target.Name}:{target.locationContextId}", LogLevel.Trace);
                }

                if (destination.TargetLocation == "BusStop" && currentlocation == "Desert" | currentlocation == "EastScarp_DeepDark" | currentlocation == "EastScarp_Village" |
                    currentlocation == "Caldera" | currentlocation == "IslandNorthCave1")
                {
                    GameLocation target = Game1.getLocationFromName(currentlocation);
                    Monitor!.Log($"Changing LocationContext from Location:{target.Name}:{target.locationContextId}", LogLevel.Trace);
                    target.locationContextId = "Default";
                    Monitor.Log($"\"Changed to Location:{target.Name}:{target.locationContextId}", LogLevel.Trace);
                }
                return true;
            }
            catch (Exception ex)
            {
                Monitor?.Log($"An error occurred when applying patch to MinecartWarp. Running original", LogLevel.Warn);
                Monitor?.Log($"Details:\n{ex}", LogLevel.Warn);
                return true;
            }
        }

        public static void ResetlocationContexts(GameLocation previouslocation, GameLocation newlocation)
        {

            if (newlocation.Name == "BusStop" && previouslocation.Name == "Desert")
            {
                Monitor!.Log($"After Warping, Changing LocationContext back from Location:{previouslocation.Name}:{previouslocation.locationContextId}", LogLevel.Trace);
                previouslocation.locationContextId = "Desert";
                Monitor.Log($"Changed to Location:{previouslocation.Name}:{previouslocation.locationContextId}", LogLevel.Trace);
            }
            if (previouslocation.Name == "BusStop" && newlocation.Name == "Desert")
            {
                Monitor!.Log($"After Warping, Changing LocationContext back from Location:{newlocation.Name}:{newlocation.locationContextId}", LogLevel.Trace);
                newlocation.locationContextId = "Desert";
                Monitor.Log($"Changed to Location:{newlocation.Name}:{newlocation.locationContextId}", LogLevel.Trace);
            }
            if (newlocation.Name == "BusStop" && previouslocation.Name == "EastScarp_DeepDark" | previouslocation.Name == "EastScarp_Village")
            {
                Monitor!.Log($"After Warping, Changing LocationContext back from Location:{previouslocation.Name}:{previouslocation.locationContextId}", LogLevel.Trace);
                previouslocation.locationContextId = "Lemurkat_EastScarp";
                Monitor.Log($"Changed to Location:{previouslocation.Name}:{previouslocation.locationContextId}", LogLevel.Trace);
            }
            if (newlocation.Name == "BusStop" && previouslocation.Name == "Caldera" | previouslocation.Name == "IslandNorthCave1")
            {
                Monitor!.Log($"After Warping, Changing LocationContext back from Location:{previouslocation.Name}:{previouslocation.locationContextId}", LogLevel.Trace);
                previouslocation.locationContextId = "Island";
                Monitor.Log($"Changed to Location:{previouslocation.Name}:{previouslocation.locationContextId}", LogLevel.Trace);
            }
        }
    }
}
