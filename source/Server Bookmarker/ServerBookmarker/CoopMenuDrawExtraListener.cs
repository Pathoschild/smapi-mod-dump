/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/ServerBookmarker
**
*************************************************/

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
