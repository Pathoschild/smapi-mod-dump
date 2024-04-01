/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dunc4nNT/StardewMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using SObject = StardewValley.Object;

namespace NeverToxic.StardewMods.YetAnotherFishingMod.Framework
{
    internal class Patches()
    {
        private static Harmony s_harmony;
        private static IMonitor s_monitor;
        private static Func<ModConfig> s_config;
        private static IReflectionHelper s_reflectionHelper;

        internal static void Initialise(Harmony harmony, IMonitor monitor, Func<ModConfig> config, IReflectionHelper reflectionHelper)
        {
            s_harmony = harmony;
            s_monitor = monitor;
            s_config = config;
            s_reflectionHelper = reflectionHelper;

            ApplyPatches();
        }

        private static void ApplyPatches()
        {
            s_harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.getFish)),
                postfix: new HarmonyMethod(typeof(Patches), nameof(Patches.GetFishPatch))
            );
        }

        private static void GetFishPatch(ref GameLocation __instance, ref Item __result, Farmer who, int waterDepth, Vector2 bobberTile)
        {
            try
            {
                ModConfig config = s_config();

                if (__result.Category == SObject.FishCategory || !(config.CatchFishRetries >= 0))
                    return;

                bool isTutorialCatch = who.fishCaught.Length == 0;

                for (int i = 0; i < config.CatchFishRetries; i++)
                {
                    __result = GameLocation.GetFishFromLocationData(__instance.Name, bobberTile, waterDepth, who, isTutorialCatch, isInherited: false, __instance);

                    if (__result.Category == SObject.FishCategory)
                        return;
                }
            }
            catch (Exception e)
            {
                s_monitor.Log($"Failed in {nameof(GetFishPatch)}:\n{e}", LogLevel.Error);
            }
        }
    }
}
