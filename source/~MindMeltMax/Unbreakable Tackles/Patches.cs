/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Netcode;
using StardewModdingAPI;
using StardewValley.Tools;
using StardewValley;
using System;
using HarmonyLib;

namespace UnbreakableTackles
{
    internal static class Patches
    {
        internal static void Patch(string id)
        {
            Harmony harmony = new(id);
            if (ModEntry.IConfig.consumeBait)
            {
                harmony.Patch(
                    original: AccessTools.Method(typeof(FishingRod), "doDoneFishing"),
                    postfix: new HarmonyMethod(typeof(Patches), nameof(doDoneFishingPostfix))
                );
            }
            else
            {
                harmony.Patch(
                    original: AccessTools.Method(typeof(FishingRod), nameof(FishingRod.doneFishing)),
                    prefix: new HarmonyMethod(typeof(Patches), nameof(doneFishingPrefix))
                );
            }
        }

        public static bool doneFishingPrefix(FishingRod __instance, bool consumeBaitAndTackle)
        {
            try
            {
                if (consumeBaitAndTackle)
                {
                    NetEvent1Field<bool, NetBool> doneFishingEvent = ModEntry.IHelper.Reflection.GetField<NetEvent1Field<bool, NetBool>>(__instance, "doneFishingEvent").GetValue();
                    doneFishingEvent.Fire(false);
                    return false;
                }
            }
            catch (Exception ex) 
            { 
                ModEntry.IMonitor.Log($"Failed patching FishingRod.doneFishing", LogLevel.Error); 
                ModEntry.IMonitor.Log($"{ex.GetType().FullName} - {ex.Message}\n{ex.StackTrace}"); 
            }
            return true;
        }

        public static void doDoneFishingPostfix(FishingRod __instance, bool consumeBaitAndTackle)
        {
            try
            {
                if (consumeBaitAndTackle && __instance.attachments[1] is not null && __instance.attachments[1].uses.Value > 0)
                    --__instance.attachments[1].uses.Value;
            }
            catch (Exception ex) 
            { 
                ModEntry.IMonitor.Log($"Failed patching FishingRod.doDoneFishing", LogLevel.Error); 
                ModEntry.IMonitor.Log($"{ex.GetType().FullName} - {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}
