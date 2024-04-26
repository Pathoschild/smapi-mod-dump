/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/thimadera/StardewMods
**
*************************************************/

using StardewValley;
using StardewValley.Objects;

namespace StackEverythingRedux.Patches
{
    internal class CanStackWithPatches
    {
        public static bool Prefix(Item __instance, ref bool __result, ISalable other)
        {
            if ((__instance is StorageFurniture dresser1 && dresser1.heldItems.Count != 0) || (other is StorageFurniture dresser2 && dresser2.heldItems.Count != 0))
            {
                __result = false;
                return false;
            }

            return true;
        }
    }
}
