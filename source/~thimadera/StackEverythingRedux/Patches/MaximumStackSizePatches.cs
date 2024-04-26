/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/thimadera/StardewMods
**
*************************************************/

namespace StackEverythingRedux.Patches
{
    internal class MaximumStackSizePatches
    {
        public static bool Prefix(ref int __result)
        {
            __result = StackEverythingRedux.Config.MaxStackingNumber;

            return false;
        }
    }
}
