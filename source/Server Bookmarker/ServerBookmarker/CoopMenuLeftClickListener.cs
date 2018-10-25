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
