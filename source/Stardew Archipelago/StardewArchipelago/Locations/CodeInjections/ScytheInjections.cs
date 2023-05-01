/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewValley;
using xTile.Dimensions;

namespace StardewArchipelago.Locations.CodeInjections
{
    public static class ScytheInjections
    {
        private const string GRIM_REAPER_STATUE = "Grim Reaper statue";

        private static IMonitor _monitor;
        private static LocationChecker _locationChecker;

        public static void Initialize(IMonitor monitor, LocationChecker locationChecker)
        {
            _monitor = monitor;
            _locationChecker = locationChecker;
        }

        public static bool PerformAction_GoldenScythe_Prefix(GameLocation __instance, string action, Farmer who, Location tileLocation, ref bool __result)
        {
            try
            {
                if (action == null || !who.IsLocalPlayer)
                {
                    return true; // run original logic
                }

                var actionParts = action.Split(' ');
                var actionName = actionParts[0];
                if (actionName == "GoldenScythe")
                {
                    __result = true;

                    if (_locationChecker.IsLocationNotChecked(GRIM_REAPER_STATUE))
                    {
                        Game1.playSound("parry");
                        __instance.setMapTileIndex(29, 4, 245, "Front");
                        __instance.setMapTileIndex(30, 4, 246, "Front");
                        __instance.setMapTileIndex(29, 5, 261, "Front");
                        __instance.setMapTileIndex(30, 5, 262, "Front");
                        __instance.setMapTileIndex(29, 6, 277, "Buildings");
                        __instance.setMapTileIndex(30, 56, 278, "Buildings");
                        _locationChecker.AddCheckedLocation(GRIM_REAPER_STATUE);
                        return false; // don't run original logic
                    }

                    Game1.changeMusicTrack("none");
                    __instance.performTouchAction("MagicWarp Mine 67 10", Game1.player.getStandingPosition());
                    return false; // don't run original logic
                }

                return true; // run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(PerformAction_GoldenScythe_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    }
}
