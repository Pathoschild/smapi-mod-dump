/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using StardewValley.Objects;

namespace CarryChest.Overrides
{
    public static class ObjectDescriptionHook
    {
        public static void Postfix(StardewValley.Object __instance, ref string __result)
        {
            if ( __instance.ParentSheetIndex == 130 )
            {
                var chest = __instance as Chest;
                __result += "\n" + $"Contains {chest?.items?.Count ?? 0} items.";
            }
        }
    }
}
