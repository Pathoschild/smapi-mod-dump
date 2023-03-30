/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using Circuit.Events;
using HarmonyLib;
using StardewValley;

namespace Circuit.Patches
{
    [HarmonyPatch(typeof(Event), nameof(Event.command_rustyKey))]
    internal class EventRustyKeyPatch
    {
        public static void Postfix()
        {
            return;
            //if (!ModEntry.ShouldPatch(EventType.RepairedServices))
              //  return;

            RepairedServices evt = (RepairedServices)EventManager.GetCurrentEvent()!;
            evt.AlreadyHasRustyKey = true;
        }
    }
}
