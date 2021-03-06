/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gzhynko/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

namespace EventBlackBars
{
    public class HarmonyPatches
    {
        /// <summary> Patch for the Game1.eventFinished method. </summary>
        public static void EventEnd()
        {
            ModEntry.Instance.StartMovingBars(false);
        }
        
        /// <summary> Patch for the GameLocation.startEvent method. </summary>
        public static void EventStart(Event evt)
        {
            if (evt.isFestival) return;
            
            ModEntry.Instance.StartMovingBars(true);
        }
    }
}