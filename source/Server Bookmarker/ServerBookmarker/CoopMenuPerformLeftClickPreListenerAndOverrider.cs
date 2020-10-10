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
