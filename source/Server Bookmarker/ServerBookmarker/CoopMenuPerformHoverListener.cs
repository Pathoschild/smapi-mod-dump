/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/ServerBookmarker
**
*************************************************/

using StardewValley.Menus;
using System;

namespace ServerBookmarker
{
    class CoopMenuPerformHoverListener : Patch
    {
        public static event Action<CoopMenu, int, int> PerformHover;

        protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(CoopMenu), "performHoverAction");

        public static bool Prefix(int x, int y, CoopMenu __instance)
        {
            PerformHover?.Invoke(__instance, x, y);
            return true;
        }
    }
}
