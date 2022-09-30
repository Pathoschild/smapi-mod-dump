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

namespace StardewRoguelike.Patches
{
    [HarmonyPatch(typeof(BuffsDisplay), "updatePosition")]
    internal class BuffDisplayUpdatePosition
    {
        public static void Postfix(BuffsDisplay __instance)
        {
            __instance.width = Game1.uiViewport.Width;
            __instance.xPositionOnScreen += 64;
            __instance.syncIcons();
        }
    }
}
