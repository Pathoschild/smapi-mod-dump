/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using StardewModdingAPI;
using StardewValley.Tools;
using System;
using HarmonyLib;
using Object = StardewValley.Object;

namespace UnbreakableTackles
{
    internal static class Patches
    {
        internal static void Patch(string id)
        {
            Harmony harmony = new(id);

            harmony.Patch(
                original: AccessTools.Method(typeof(FishingRod), "doDoneFishing"),
                prefix: new(typeof(Patches), nameof(doDoneFishingPrefix)),
                postfix: new HarmonyMethod(typeof(Patches), nameof(doDoneFishingPostfix))
            );
        }

        internal static void doDoneFishingPrefix(FishingRod __instance, ref int __state)
        {
            try
            {
                if (__instance.GetBait() is Object bait && !ModEntry.IConfig.consumeBait)
                {
                    __state = bait.Stack;
                    __instance.GetBait().Stack++;
                }
            }
            catch (Exception ex)
            {
                ModEntry.IMonitor.Log($"Failed prefixing FishingRod.doDoneFishing", LogLevel.Error);
                ModEntry.IMonitor.Log($"{ex.GetType().FullName} - {ex.Message}\n{ex.StackTrace}");
            }
        }

        public static void doDoneFishingPostfix(FishingRod __instance, ref int __state)
        {
            try
            {
                if (!ModEntry.IConfig.consumeBait && __state > 0 && __instance.GetBait() is Object bait && bait.Stack < __state)
                    __instance.GetBait().Stack = __state;
                if (__instance.attachments[1] is not null && __instance.attachments[1].uses.Value > 0)
                    --__instance.attachments[1].uses.Value;
                if (__instance.attachments.Count > 2 && __instance.attachments[2] is not null && __instance.attachments[2].uses.Value > 0) //For advanced iridium rod tackle 2 index
                    --__instance.attachments[2].uses.Value;
            }
            catch (Exception ex) 
            { 
                ModEntry.IMonitor.Log($"Failed postfixing FishingRod.doDoneFishing", LogLevel.Error); 
                ModEntry.IMonitor.Log($"{ex.GetType().FullName} - {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}
