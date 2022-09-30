/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyRoguelike
**
*************************************************/

using HarmonyLib;
using StardewValley;
using StardewValley.Menus;
using System;

namespace StardewRoguelike.Patches
{
    internal class FreeForgePatch : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(ForgeMenu), "GetForgeCost");

        public static bool Prefix(ref int __result)
        {
            __result = 0;
            return false;
        }
    }

    internal class FreeForgePatch2 : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(ForgeMenu), "GetForgeCostAtLevel");

        public static bool Prefix(ref int __result)
        {
            __result = 0;
            return false;
        }
    }

    [HarmonyPatch(typeof(Utility), "CollectOrDrop", new Type[] { typeof(Item), typeof(int) })]
    internal class UtilityCollectOrDrop
    {
        public static bool Prefix(ref bool __result, Item item, int direction)
        {
            if (item is not null && item.ParentSheetIndex == 848)
            {
                __result = false;
                return false;
            }

            return true;
        }
    }
}
