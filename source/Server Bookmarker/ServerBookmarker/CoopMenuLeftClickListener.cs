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
    class CoopMenuLeftClickListener : Patch
    {
        public static event Action<CoopMenu, int, int> LeftClick;

        protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(CoopMenu), "receiveLeftClick");

        public static void Postfix(int x, int y, CoopMenu __instance)
        {
            LeftClick?.Invoke(__instance, x, y);
        }
    }
}
