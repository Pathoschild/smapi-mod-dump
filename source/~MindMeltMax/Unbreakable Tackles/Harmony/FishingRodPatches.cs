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
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unbreakable_Tackles.Harmony
{
    public class FishingRodPatches
    {
        public static bool doneFishing_prefix(FishingRod __instance, Farmer who, bool consumeBaitAndTackle)
        {
            try
            {
                if (consumeBaitAndTackle)
                {
                    NetEvent1Field<bool, NetBool> doneFishingEvent = ModEntry.IHelper.Reflection.GetField<NetEvent1Field<bool, NetBool>>(__instance, "doneFishingEvent").GetValue();
                    doneFishingEvent.Fire(false);
                    return false;
                }
                return true;
            }
            catch(Exception ex) { ModEntry.IMonitor.Log($"Failed patching FishingRod.doneFishing", LogLevel.Error); ModEntry.IMonitor.Log($"{ex} - {ex.Message}"); return true; }
        }

        public static void doDoneFishing_postfix(FishingRod __instance, bool consumeBaitAndTackle)
        {
            try
            {
                if (consumeBaitAndTackle)
                {
                    bool lastCatchWasJunk = ModEntry.IHelper.Reflection.GetField<bool>(__instance, "lastCatchWasJunk").GetValue();
                    if (__instance.attachments[1] != null && __instance.attachments[1].uses.Value > 0)
                        --__instance.attachments[1].uses.Value;
                }
            }
            catch (Exception ex) { ModEntry.IMonitor.Log($"Failed patching FishingRod.doDoneFishing", LogLevel.Error); ModEntry.IMonitor.Log($"{ex} - {ex.Message}"); return; }
        }
    }
}
