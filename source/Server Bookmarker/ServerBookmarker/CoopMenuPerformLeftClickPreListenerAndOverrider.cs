using StardewValley.Menus;
using System;
using StardewValley;

namespace ServerBookmarker
{
    class CoopMenuPerformLeftClickPreListenerAndOverrider : Patch
    {
        public static event Action<ShouldOverrideClick> PerformLeftClick;

        protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(TitleMenu), "receiveLeftClick");

        public static bool Prefix(TitleMenu __instance)
        {
            if (__instance == Game1.activeClickableMenu)
            {
                var o = new ShouldOverrideClick();
                PerformLeftClick?.Invoke(o);
                
                return !o.Override;
            }
            return true;
        }
    }

    class ShouldOverrideClick
    {
        public bool Override { get; set; } = false;
    }
}
