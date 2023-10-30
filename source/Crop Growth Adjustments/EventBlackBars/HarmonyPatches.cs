/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gzhynko/stardew-mods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;

//ReSharper disable InconsistentNaming

namespace EventBlackBars
{
    public class HarmonyPatches
    {
        /// <summary> Patch for the Event.exitEvent method. </summary>
        public static void EventEnd(Event __instance)
        {
            if (__instance.isFestival || __instance.isWedding) return;
            
            ModEntry.Instance.StartMovingBars(Direction.MoveOut);
        }
        
        /// <summary> Patch for the GameLocation.startEvent method. </summary>
        public static void EventStart(Event evt)
        {
            if (evt.isFestival || evt.isWedding) return;
            
            ModEntry.Instance.StartMovingBars(Direction.MoveIn);
        }
    }
}