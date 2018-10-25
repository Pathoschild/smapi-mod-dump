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
