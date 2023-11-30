/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mus-candidus/LittleNPCs
**
*************************************************/

using StardewValley;
using StardewValley.Characters;


namespace LittleNPCs.Framework.Patches {
    /// <summary>
    /// Prefix for <code>Child.checkAction</code>.
    /// Disables interaction with children.
    /// </summary>
    public class ChildCheckActionPatch {
        public static bool Prefix(Child __instance, Farmer who, GameLocation l, ref bool __result) {
            if (ModEntry.IsValidLittleNPCIndex(__instance.GetChildIndex()) && __instance.daysOld.Value >= ModEntry.config_.AgeWhenKidsAreModified) {
                __result = false;

                // Disable original method.
                return false;
            }
            
            // Enable original method.
            return true;
        }
    }
}
