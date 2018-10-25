using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using System;

namespace ServerBookmarker
{
    class CoopMenuDrawExtraListener : Patch
    {
        public static event Action<CoopMenu, SpriteBatch> DrawExtra;

        protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(CoopMenu), "drawExtra");

        public static void Postfix(SpriteBatch b, CoopMenu __instance)
        {
            DrawExtra?.Invoke(__instance, b);
        }
    }
}
