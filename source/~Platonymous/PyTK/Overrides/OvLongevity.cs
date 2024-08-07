/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using HarmonyLib;
using PyTK.CustomElementHandler;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Reflection;

namespace PyTK.Overrides
{
    internal class FakeLongevityPrice
    {
        public void UpdateItem(int itemIndex, int updateType, Chest chest = null)
        {

        }
    }

    [HarmonyPatch]
    internal class LongevityFix
    {
        internal static MethodInfo TargetMethod()
        {
            if (Type.GetType("Longevity.Price, Longevity") != null)
                return AccessTools.Method(Type.GetType("Longevity.Price, Longevity"), "UpdateItem");
            else
                return AccessTools.Method(typeof(FakeLongevityPrice), "UpdateItem");
        }

        internal static bool Prefix(int updateType, int itemIndex, Chest chest = null)
        {
            Item item = (updateType == 0) ? Game1.player.Items[itemIndex] : (updateType == 1 || chest == null) ? chest.items[itemIndex] : null;

            if(item == null || item.ParentSheetIndex < 0 || item is ISaveElement)
                return false;

            return true;
        }
    }
}
