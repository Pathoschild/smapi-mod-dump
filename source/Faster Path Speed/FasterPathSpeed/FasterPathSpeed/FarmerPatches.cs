/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/alanperrow/StardewModding
**
*************************************************/

using StardewModdingAPI;
using System;
using StardewValley;

namespace FasterPathSpeed
{
    public class FarmerPatches
    {
        private static IMonitor Monitor;
        private static ModConfig Config;

        public static void Initialize(IMonitor monitor, ModConfig config)
        {
            Monitor = monitor;
            Config = config;
        }

        public static void GetMovementSpeed_Postfix(Farmer __instance, ref float __result)
        {
            try
            {
                if ((Game1.CurrentEvent == null || Game1.CurrentEvent.playerControlSequence) && (!(Game1.CurrentEvent == null && __instance.hasBuff(19))) &&
                    (!Config.IsPathSpeedBuffOnlyOnTheFarm || Game1.currentLocation.IsFarm))
                {
                    bool isOnFeature = Game1.currentLocation.terrainFeatures.TryGetValue(__instance.getTileLocation(), out StardewValley.TerrainFeatures.TerrainFeature terrainFeature);

                    if (isOnFeature && terrainFeature is StardewValley.TerrainFeatures.Flooring)
                    {
                        float pathSpeedBoost = ModEntry.GetPathSpeedBuffByFlooringType(terrainFeature as StardewValley.TerrainFeatures.Flooring);

                        float mult = __instance.movementMultiplier * Game1.currentGameTime.ElapsedGameTime.Milliseconds *
                            ((!Game1.eventUp && __instance.isRidingHorse()) ? Config.HorsePathSpeedBuffModifier : 1);

                        __result += (__instance.movementDirections.Count > 1) ? (0.7f * pathSpeedBoost * mult) : (pathSpeedBoost * mult);
                    }
                }
                // Don't mess with ELSE for now, don't want to make any unintentional errors dealing with Event movement
            }
            catch (Exception e)
            {
                Monitor.Log($"Failed in {nameof(GetMovementSpeed_Postfix)}:\n{e}", LogLevel.Error);
            }
        }
    }
}
