/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JeanSebGwak/CustomFenceDecay
**
*************************************************/

using HarmonyLib;
using CustomFenceDecay.Configuration;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomFenceDecay
{
    public static class HarmonyPatch_CustomFenceDecay
    {
        /*****            *****/
        /***** Setup Code *****/
        /*****            *****/

        /// <summary>True if this patch is currently applied.</summary>
        public static bool _applied { get; private set; } = false;

        /// <summary>The monitor instance to use for log messages. Null if not provided.</summary>
        private static IMonitor _monitor { get; set; } = null;

        /// <summary>Applies this Harmony patch to the game.</summary>
        /// <param name="harmony">The <see cref="Harmony"/> created with this mod's ID.</param>
        /// /// <param name="helper">The <see cref="IModHelper"/> provided to this mod by SMAPI.</param>
        /// <param name="monitor">The <see cref="IMonitor"/> provided to this mod by SMAPI. Used for log messages.</param>
        public static void ApplyPatch(Harmony harmony, IMonitor monitor)
        {
            if (!_applied && monitor != null) //if NOT already enabled
            {
                _monitor = monitor; //store monitor

                if (ModEntry.Config != null)
                {
                    _monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_CustomFenceDecay)}\": postfixing SDV method \"Fence.minutesElapsed()\".", LogLevel.Trace);

                    harmony.Patch(
                        original: AccessTools.Method(typeof(Fence), "minutesElapsed", new[] { typeof(int) }),
                        postfix: new HarmonyMethod(typeof(HarmonyPatch_CustomFenceDecay), nameof(Fence_minutesElapsed))
                    );

                    _applied = true;
                }
                else
                {
                    _monitor.Log("Couldn't apply Harmony patch due to missing configuration.", LogLevel.Error);
                }
            }
        }

        private static void Fence_minutesElapsed(Fence __instance, int minutes, ref bool __result)
        {
            if (Game1.IsMasterGame && (!Game1.IsBuildingConstructed("Gold Clock") || Game1.netWorldState.Value.goldenClocksTurnedOff.Value))
            {
                // If IdenticalValue checkbox is checked
                if (ModEntry.Config.IdenticalValue)
                {
                    __instance.health.Value -= CalculateFenceDecaying(minutes, ModEntry.Config.FenceDecaySpeedInPercent);
                }
                else
                {
                    // Lower and trim to be sure
                    switch(__instance.Name.ToLowerInvariant().Trim())
                    {
                        case "wood fence":
                            __instance.health.Value -= CalculateFenceDecaying(minutes, ModEntry.Config.WoodFenceDecaySpeedInPercent);
                            break;
                        case "stone fence":
                            __instance.health.Value -= CalculateFenceDecaying(minutes, ModEntry.Config.StoneFenceDecaySpeedInPercent);
                            break;
                        case "iron fence":
                            __instance.health.Value -= CalculateFenceDecaying(minutes, ModEntry.Config.IronFenceDecaySpeedInPercent);
                            break;
                        case "hardwood fence":
                            __instance.health.Value -= CalculateFenceDecaying(minutes, ModEntry.Config.HardwoodFenceDecaySpeedInPercent);
                            break;
                        // Fallback in case of other fence
                        default:
                            __instance.health.Value -= CalculateFenceDecaying(minutes, ModEntry.Config.FenceDecaySpeedInPercent);
                            break;
                    }
                }

                // If the fence health value drop under (or equal) 1, we destroy it
                if (__instance.health.Value <= -1f && (Game1.timeOfDay <= 610 || Game1.timeOfDay > 1800))
                {
                    __result = true;
                }
            }

            __result = false;
        }

        private static float CalculateFenceDecaying(int minutes, int decaySetting)
        {
            // If decaying value is lesser or equal to 0, we return 0
            if (decaySetting > 0)
                // As decaySetting is a value in percent, we divide by 100
                // 1440 is a value from the base game
                return (float)((minutes / 1440f) * (decaySetting / 100));
            else
                return 0f;
        }
    }
}
